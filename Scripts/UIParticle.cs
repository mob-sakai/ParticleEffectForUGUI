using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    {
        [HideInInspector] [SerializeField] bool m_IsTrail = false;

        [Tooltip("Ignore canvas scaler")] [SerializeField] [FormerlySerializedAs("m_IgnoreParent")]
        bool m_IgnoreCanvasScaler = true;

        [Tooltip("Particle effect scale")] [SerializeField]
        float m_Scale = 100;

        [Tooltip("Animatable material properties. If you want to change the material properties of the ParticleSystem in Animation, enable it.")] [SerializeField]
        internal AnimatableProperty[] m_AnimatableProperties = new AnimatableProperty[0];

        [Tooltip("Particles")] [SerializeField]
        private List<ParticleSystem> m_Particles = new List<ParticleSystem>();

        private DrivenRectTransformTracker _tracker;
        private Mesh _bakedMesh;
        private readonly List<Material> _modifiedMaterials = new List<Material>();
        private uint _activeMeshIndices;
        private Vector3 _cachedPosition;
        private static readonly List<Material> s_TempMaterials = new List<Material>(2);


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
            set { m_IgnoreCanvasScaler = value; }
        }

        /// <summary>
        /// Particle effect scale.
        /// </summary>
        public float scale
        {
            get { return m_Scale; }
            set { m_Scale = Mathf.Max(0.001f, value); }
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

        internal uint activeMeshIndices
        {
            get { return _activeMeshIndices; }
            set
            {
                if (_activeMeshIndices == value) return;
                _activeMeshIndices = value;
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

        public void RefreshParticles()
        {
            GetComponentsInChildren(particles);

            particles.Exec(p => p.GetComponent<ParticleSystemRenderer>().enabled = !enabled);
            particles.SortForRendering(transform);

            SetMaterialDirty();
        }

        protected override void UpdateMaterial()
        {
            // Clear modified materials.
            for (var i = 0; i < _modifiedMaterials.Count; i++)
            {
                StencilMaterial.Remove(_modifiedMaterials[i]);
                DestroyImmediate(_modifiedMaterials[i]);
                _modifiedMaterials[i] = null;
            }

            _modifiedMaterials.Clear();

            // Recalculate stencil value.
            if (m_ShouldRecalculateStencil)
            {
                var rootCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
                m_StencilValue = maskable ? MaskUtilities.GetStencilDepth(transform, rootCanvas) : 0;
                m_ShouldRecalculateStencil = false;
            }

            // No mesh to render.
            if (activeMeshIndices == 0 || !isActiveAndEnabled || particles.Count == 0)
            {
                _activeMeshIndices = 0;
                canvasRenderer.Clear();
                return;
            }

            //
            var materialCount = Mathf.Max(8, activeMeshIndices.BitCount());
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
                var bit = 1 << (i * 2);
                if (0 < (activeMeshIndices & bit) && 0 < s_TempMaterials.Count)
                {
                    var mat = GetModifiedMaterial(s_TempMaterials[0], ps.GetTextureForSprite());
                    canvasRenderer.SetMaterial(mat, j++);
                }

                // Trails
                if (materialCount <= j) break;
                bit <<= 1;
                if (0 < (activeMeshIndices & bit) && 1 < s_TempMaterials.Count)
                {
                    var mat = GetModifiedMaterial(s_TempMaterials[1], null);
                    canvasRenderer.SetMaterial(mat, j++);
                }
            }
        }

        private Material GetModifiedMaterial(Material baseMaterial, Texture2D texture)
        {
            if (0 < m_StencilValue)
            {
                baseMaterial = StencilMaterial.Add(baseMaterial, (1 << m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
                _modifiedMaterials.Add(baseMaterial);
            }

            if (texture == null && m_AnimatableProperties.Length == 0) return baseMaterial;

            baseMaterial = new Material(baseMaterial);
            _modifiedMaterials.Add(baseMaterial);
            if (texture)
                baseMaterial.mainTexture = texture;

            return baseMaterial;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            InitializeIfNeeded();

            _cachedPosition = transform.localPosition;
            _activeMeshIndices = 0;

            UIParticleUpdater.Register(this);
            particles.Exec(p => p.GetComponent<ParticleSystemRenderer>().enabled = false);
            _tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);

            // Create objects.
            _bakedMesh = new Mesh();
            _bakedMesh.MarkDynamic();

            base.OnEnable();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            UIParticleUpdater.Unregister(this);
            particles.Exec(p => p.GetComponent<ParticleSystemRenderer>().enabled = true);
            _tracker.Clear();

            // Destroy object.
            DestroyImmediate(_bakedMesh);
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
        /// This function is called when the parent property of the transform of the GameObject has changed.
        /// </summary>
        protected override void OnTransformParentChanged()
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
            if (0 < particles.Count) return;

            if (m_IsTrail
                || transform.parent && transform.parent.GetComponentInParent<UIParticle>())
            {
                gameObject.SetActive(false);
                if (Application.isPlaying)
                    Destroy(gameObject);
                else
                    DestroyImmediate(gameObject);
                return;
            }

            // refresh.
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.delayCall += RefreshParticles;
            else
#endif
                RefreshParticles();
        }
    }
}
