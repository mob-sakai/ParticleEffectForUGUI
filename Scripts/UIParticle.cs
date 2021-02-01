#if UNITY_2019_3_11 || UNITY_2019_3_12 || UNITY_2019_3_13 || UNITY_2019_3_14 || UNITY_2019_3_15 || UNITY_2019_4_OR_NEWER
#define SERIALIZE_FIELD_MASKABLE
#endif
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Coffee.UIParticleExtensions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

[assembly: InternalsVisibleTo("Coffee.UIParticle.Editor")]

namespace Coffee.UIExtensions
{
    /// <summary>
    /// Render maskable and sortable particle effect ,without Camera, RenderTexture or Canvas.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIParticle : MaskableGraphic
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
        [HideInInspector] [SerializeField] internal bool m_IsTrail = false;

        [Tooltip("Ignore canvas scaler")] [SerializeField] [FormerlySerializedAs("m_IgnoreParent")]
        bool m_IgnoreCanvasScaler = true;

        [Tooltip("Particle effect scale")] [SerializeField]
        float m_Scale = 100;

        [Tooltip("Particle effect scale")] [SerializeField]
        private Vector3 m_Scale3D;

        [Tooltip("Animatable material properties. If you want to change the material properties of the ParticleSystem in Animation, enable it.")] [SerializeField]
        internal AnimatableProperty[] m_AnimatableProperties = new AnimatableProperty[0];

        [Tooltip("Particles")] [SerializeField]
        private List<ParticleSystem> m_Particles = new List<ParticleSystem>();

        [Tooltip("Shrink rendering by material on refresh.\nNOTE: Performance will be improved, but in some cases the rendering is not correct.")] [SerializeField]
        bool m_ShrinkByMaterial = false;

#if !SERIALIZE_FIELD_MASKABLE
        [SerializeField] private bool m_Maskable = true;
#endif

        private bool _shouldBeRemoved;
        private DrivenRectTransformTracker _tracker;
        private Mesh _bakedMesh;
        private readonly List<Material> _modifiedMaterials = new List<Material>();
        private readonly List<Material> _maskMaterials = new List<Material>();
        private readonly List<bool> _activeMeshIndices = new List<bool>();
        private Vector3 _cachedPosition;
        private static readonly List<Material> s_TempMaterials = new List<Material>(2);
        private static MaterialPropertyBlock s_Mpb;
        private static readonly List<Material> s_PrevMaskMaterials = new List<Material>();
        private static readonly List<Material> s_PrevModifiedMaterials = new List<Material>();
        private static readonly List<Component> s_Components = new List<Component>();


        /// <summary>
        /// Should this graphic be considered a target for raycasting?
        /// </summary>
        public override bool raycastTarget
        {
            get { return false; }
            set { }
        }

        public bool ignoreCanvasScaler
        {
            get { return m_IgnoreCanvasScaler; }
            set
            {
                // if (m_IgnoreCanvasScaler == value) return;
                m_IgnoreCanvasScaler = value;
                _tracker.Clear();
                if (isActiveAndEnabled && m_IgnoreCanvasScaler)
                    _tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
            }
        }

        public bool shrinkByMaterial
        {
            get { return m_ShrinkByMaterial; }
            set
            {
                if (m_ShrinkByMaterial == value) return;
                m_ShrinkByMaterial = value;
                RefreshParticles();
            }
        }

        /// <summary>
        /// Particle effect scale.
        /// </summary>
        public float scale
        {
            get { return m_Scale3D.x; }
            set
            {
                m_Scale = Mathf.Max(0.001f, value);
                m_Scale3D = new Vector3(m_Scale, m_Scale, m_Scale);
            }
        }

        /// <summary>
        /// Particle effect scale.
        /// </summary>
        public Vector3 scale3D
        {
            get { return m_Scale3D; }
            set
            {
                if (m_Scale3D == value) return;
                m_Scale3D.x = Mathf.Max(0.001f, value.x);
                m_Scale3D.y = Mathf.Max(0.001f, value.y);
                m_Scale3D.z = Mathf.Max(0.001f, value.z);
            }
        }

        internal Mesh bakedMesh
        {
            get { return _bakedMesh; }
        }

        public List<ParticleSystem> particles
        {
            get { return m_Particles; }
        }

        public IEnumerable<Material> materials
        {
            get { return _modifiedMaterials; }
        }

        public override Material materialForRendering
        {
            get { return canvasRenderer.GetMaterial(0); }
        }

        public List<bool> activeMeshIndices
        {
            get { return _activeMeshIndices; }
            set
            {
                if (_activeMeshIndices.SequenceEqualFast(value)) return;
                _activeMeshIndices.Clear();
                _activeMeshIndices.AddRange(value);
                UpdateMaterial();
            }
        }

        internal Vector3 cachedPosition
        {
            get { return _cachedPosition; }
            set { _cachedPosition = value; }
        }

        public void Play()
        {
            particles.Exec(p => p.Play());
        }

        public void Pause()
        {
            particles.Exec(p => p.Pause());
        }

        public void Stop()
        {
            particles.Exec(p => p.Stop());
        }

        public void SetParticleSystemInstance(GameObject instance)
        {
            SetParticleSystemInstance(instance, true);
        }

        public void SetParticleSystemInstance(GameObject instance, bool destroyOldParticles)
        {
            if (!instance) return;

            foreach (Transform child in transform)
            {
                var go = child.gameObject;
                go.SetActive(false);
                if (!destroyOldParticles) continue;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(go);
                else
#endif
                    Destroy(go);
            }

            var tr = instance.transform;
            tr.SetParent(transform, false);
            tr.localPosition = Vector3.zero;

            RefreshParticles(instance);
        }

        public void SetParticleSystemPrefab(GameObject prefab)
        {
            if (!prefab) return;

            SetParticleSystemInstance(Instantiate(prefab.gameObject), true);
        }

        public void RefreshParticles()
        {
            RefreshParticles(gameObject);
        }

        public void RefreshParticles(GameObject root)
        {
            if (!root) return;
            root.GetComponentsInChildren(particles);
            particles.RemoveAll(x => x.GetComponentInParent<UIParticle>() != this);

            foreach (var ps in particles)
            {
                var tsa = ps.textureSheetAnimation;
                if (tsa.mode == ParticleSystemAnimationMode.Sprites && tsa.uvChannelMask == (UVChannelFlags) 0)
                    tsa.uvChannelMask = UVChannelFlags.UV0;
            }

            particles.Exec(p => p.GetComponent<ParticleSystemRenderer>().enabled = !enabled);
            particles.SortForRendering(transform, m_ShrinkByMaterial);

            SetMaterialDirty();
        }

        protected override void UpdateMaterial()
        {
            // Clear mask materials.
            s_PrevMaskMaterials.AddRange(_maskMaterials);
            _maskMaterials.Clear();

            // Clear modified materials.
            s_PrevModifiedMaterials.AddRange(_modifiedMaterials);
            _modifiedMaterials.Clear();

            // Recalculate stencil value.
            if (m_ShouldRecalculateStencil)
            {
                var rootCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
                m_StencilValue = maskable ? MaskUtilities.GetStencilDepth(transform, rootCanvas) : 0;
                m_ShouldRecalculateStencil = false;
            }

            // No mesh to render.
            var count = activeMeshIndices.CountFast();
            if (count == 0 || !isActiveAndEnabled || particles.Count == 0)
            {
                canvasRenderer.Clear();
                ClearPreviousMaterials();
                return;
            }

            //
            GetComponents(typeof(IMaterialModifier), s_Components);
            var materialCount = Mathf.Max(8, count);
            canvasRenderer.materialCount = materialCount;
            var j = 0;
            for (var i = 0; i < particles.Count; i++)
            {
                if (materialCount <= j) break;
                var ps = particles[i];
                if (!ps) continue;

                var r = ps.GetComponent<ParticleSystemRenderer>();
                r.GetSharedMaterials(s_TempMaterials);

                // Main
                var index = i * 2;
                if (activeMeshIndices.Count <= index) break;
                if (activeMeshIndices[index] && 0 < s_TempMaterials.Count)
                {
                    var mat = GetModifiedMaterial(s_TempMaterials[0], ps.GetTextureForSprite());
                    for (var k = 1; k < s_Components.Count; k++)
                        mat = (s_Components[k] as IMaterialModifier).GetModifiedMaterial(mat);
                    canvasRenderer.SetMaterial(mat, j);
                    UpdateMaterialProperties(r, j);
                    j++;
                }

                // Trails
                index++;
                if (activeMeshIndices.Count <= index || materialCount <= j) break;
                if (activeMeshIndices[index] && 1 < s_TempMaterials.Count)
                {
                    var mat = GetModifiedMaterial(s_TempMaterials[1], null);
                    for (var k = 1; k < s_Components.Count; k++)
                        mat = (s_Components[k] as IMaterialModifier).GetModifiedMaterial(mat);
                    canvasRenderer.SetMaterial(mat, j++);
                }
            }

            ClearPreviousMaterials();
        }

        private void ClearPreviousMaterials()
        {
            foreach (var m in s_PrevMaskMaterials)
                StencilMaterial.Remove(m);
            s_PrevMaskMaterials.Clear();

            foreach (var m in s_PrevModifiedMaterials)
                ModifiedMaterial.Remove(m);
            s_PrevModifiedMaterials.Clear();
        }

        private Material GetModifiedMaterial(Material baseMaterial, Texture2D texture)
        {
            if (0 < m_StencilValue)
            {
                baseMaterial = StencilMaterial.Add(baseMaterial, (1 << m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
                _maskMaterials.Add(baseMaterial);
            }

            if (texture == null && m_AnimatableProperties.Length == 0) return baseMaterial;

            var id = m_AnimatableProperties.Length == 0 ? 0 : GetInstanceID();
            baseMaterial = ModifiedMaterial.Add(baseMaterial, texture, id);
            _modifiedMaterials.Add(baseMaterial);

            return baseMaterial;
        }

        internal void UpdateMaterialProperties()
        {
            if (m_AnimatableProperties.Length == 0) return;

            //
            var count = activeMeshIndices.CountFast();
            var materialCount = Mathf.Max(8, count);
            canvasRenderer.materialCount = materialCount;
            var j = 0;
            for (var i = 0; i < particles.Count; i++)
            {
                if (materialCount <= j) break;
                var ps = particles[i];
                if (!ps) continue;

                var r = ps.GetComponent<ParticleSystemRenderer>();
                r.GetSharedMaterials(s_TempMaterials);

                // Main
                if (activeMeshIndices[i * 2] && 0 < s_TempMaterials.Count)
                {
                    UpdateMaterialProperties(r, j);
                    j++;
                }
            }
        }

        internal void UpdateMaterialProperties(Renderer r, int index)
        {
            if (m_AnimatableProperties.Length == 0 || canvasRenderer.materialCount <= index) return;

            r.GetPropertyBlock(s_Mpb ?? (s_Mpb = new MaterialPropertyBlock()));
            if (s_Mpb.isEmpty) return;

            // #41: Copy the value from MaterialPropertyBlock to CanvasRenderer
            var mat = canvasRenderer.GetMaterial(index);
            if (!mat) return;

            foreach (var ap in m_AnimatableProperties)
            {
                ap.UpdateMaterialProperties(mat, s_Mpb);
            }

            s_Mpb.Clear();
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
#if !SERIALIZE_FIELD_MASKABLE
            maskable = m_Maskable;
#endif
            _cachedPosition = transform.position;
            activeMeshIndices.Clear();

            UIParticleUpdater.Register(this);
            particles.Exec(p => p.GetComponent<ParticleSystemRenderer>().enabled = false);

            if (isActiveAndEnabled && m_IgnoreCanvasScaler)
            {
                _tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
            }

            // Create objects.
            _bakedMesh = MeshPool.Rent();

            base.OnEnable();

            InitializeIfNeeded();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            UIParticleUpdater.Unregister(this);
            if (!_shouldBeRemoved)
                particles.Exec(p => p.GetComponent<ParticleSystemRenderer>().enabled = true);
            _tracker.Clear();

            // Destroy object.
            MeshPool.Return(_bakedMesh);
            _bakedMesh = null;

            base.OnDisable();
        }

        /// <summary>
        /// Call to update the geometry of the Graphic onto the CanvasRenderer.
        /// </summary>
        protected override void UpdateGeometry()
        {
        }

        /// <summary>
        /// Callback for when properties have been changed by animation.
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
        }

        private void InitializeIfNeeded()
        {
            if (enabled && m_IsTrail)
            {
                UnityEngine.Debug.LogWarningFormat(this, "[UIParticle] The UIParticle component should be removed: {0}\nReason: UIParticle for trails is no longer needed.", name);
                gameObject.hideFlags = HideFlags.None;
                _shouldBeRemoved = true;
                enabled = false;
                return;
            }

            if (!this || particles.AnyFast()) return;

            // refresh.
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this) RefreshParticles();
                };
            else
#endif
                RefreshParticles();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetLayoutDirty();
            SetVerticesDirty();
            m_ShouldRecalculateStencil = true;
            RecalculateClipping();
#if !SERIALIZE_FIELD_MASKABLE
            maskable = m_Maskable;
#endif
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (Application.isPlaying) return;
            InitializeIfNeeded();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_Scale3D == Vector3.zero)
            {
                scale = m_Scale;
            }

            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (Application.isPlaying || !this) return;
                InitializeIfNeeded();
            };
        }
#endif
    }
}
