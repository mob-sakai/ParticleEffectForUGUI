#if UNITY_2019_3_11 || UNITY_2019_3_12 || UNITY_2019_3_13 || UNITY_2019_3_14 || UNITY_2019_3_15 || UNITY_2019_4_OR_NEWER
#define SERIALIZE_FIELD_MASKABLE
#endif
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Coffee.UIParticleExtensions;
using UnityEngine;
using UnityEngine.Rendering;
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
    {
        [HideInInspector] [SerializeField] internal bool m_IsTrail = false;

        [Tooltip("Particle effect scale")] [SerializeField]
        private Vector3 m_Scale3D = new Vector3(1, 1, 1);

        [Tooltip("Animatable material properties. If you want to change the material properties of the ParticleSystem in Animation, enable it.")] [SerializeField]
        internal AnimatableProperty[] m_AnimatableProperties = new AnimatableProperty[0];

        [Tooltip("Particles")] [SerializeField]
        private List<ParticleSystem> m_Particles = new List<ParticleSystem>();

        [Tooltip("Shrink rendering by material on refresh.\nNOTE: This option will improve canvas batching and performance, but in some cases the rendering is not correct.")] [SerializeField]
        private bool m_ShrinkByMaterial = true;

#if !SERIALIZE_FIELD_MASKABLE
        [SerializeField] private bool m_Maskable = true;
#endif

        private bool _shouldBeRemoved;
        private Mesh _bakedMesh;
        private List<Material> _modifiedMaterials;
        private List<Material> _maskMaterials;
        private List<bool> _activeMeshIndices;
        private static MaterialPropertyBlock s_Mpb;
        private static readonly List<Material> s_TempMaterials = new List<Material>(2);
        private static readonly List<Material> s_PrevMaskMaterials = new List<Material>();
        private static readonly List<Material> s_PrevModifiedMaterials = new List<Material>();
        private static readonly List<Component> s_Components = new List<Component>();
        private static readonly List<ParticleSystem> s_ParticleSystems = new List<ParticleSystem>();


        /// <summary>
        /// Should this graphic be considered a target for raycasting?
        /// </summary>
        public override bool raycastTarget
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Shrink rendering by material on refresh.
        /// NOTE: This option will improve canvas batching and performance, but in some cases the rendering is not correct.
        /// </summary>
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
            set { m_Scale3D = new Vector3(value, value, value); }
        }

        /// <summary>
        /// Particle effect scale.
        /// </summary>
        public Vector3 scale3D
        {
            get { return m_Scale3D; }
            set { m_Scale3D = value; }
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

        public void Play()
        {
            particles.Exec(p =>
            {
                p.Simulate(0, false, true);
                Debug.Log(p.particleCount);
            });
        }

        public void Pause()
        {
            particles.Exec(p => p.Pause());
        }

        public void Stop()
        {
            particles.Exec(p => p.Stop());
        }

        public void Clear()
        {
            particles.Exec(p => p.Clear());
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
                if (tsa.mode == ParticleSystemAnimationMode.Sprites && tsa.uvChannelMask == 0)
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
            var materialCount = Mathf.Min(8, count);
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
                        mat = ((IMaterialModifier)s_Components[k]).GetModifiedMaterial(mat);
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
                        mat = ((IMaterialModifier)s_Components[k]).GetModifiedMaterial(mat);
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
            var count = activeMeshIndices.CountFast();
            if (m_AnimatableProperties.Length == 0 || count == 0) return;

            //
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

            _modifiedMaterials = ListPool.Rent<Material>();
            _maskMaterials = ListPool.Rent<Material>();
            _activeMeshIndices = ListPool.Rent<bool>();

            UIParticleUpdater.Register(this);
            particles.Exec(p => p.GetComponent<ParticleSystemRenderer>().enabled = false);

            // Create objects.
            _bakedMesh = MeshPool.Rent();

            base.OnEnable();

            InitializeIfNeeded();
        }

        private new IEnumerator Start()
        {
            // #147: ParticleSystem creates Particles in wrong position during prewarm
            // #148: Particle Sub Emitter not showing when start game
            var delayToPlay = particles.AnyFast(ps =>
            {
                ps.GetComponentsInChildren(false, s_ParticleSystems);
                return s_ParticleSystems.AnyFast(p => p.isPlaying); //&& (p.subEmitters.enabled || p.main.prewarm));
            });
            s_ParticleSystems.Clear();
            if (!delayToPlay) yield break;

            Stop();
            Clear();
            yield return null;

            Play();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            UIParticleUpdater.Unregister(this);
            if (!_shouldBeRemoved)
                particles.Exec(p => p.GetComponent<ParticleSystemRenderer>().enabled = true);

            // Destroy object.
            MeshPool.Return(_bakedMesh);
            _bakedMesh = null;

            activeMeshIndices.Clear();
            UpdateMaterial();

            ListPool.Return(_modifiedMaterials);
            ListPool.Return(_maskMaterials);
            ListPool.Return(_activeMeshIndices);
            _modifiedMaterials = null;
            _maskMaterials = null;
            _activeMeshIndices = null;

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
        protected override void Reset()
        {
            InitializeIfNeeded();
            base.Reset();
        }

        protected override void OnValidate()
        {
#if !SERIALIZE_FIELD_MASKABLE
            maskable = m_Maskable;
#endif
            SetLayoutDirty();
            SetVerticesDirty();
            m_ShouldRecalculateStencil = true;
            RecalculateClipping();
        }
#endif
    }
}
