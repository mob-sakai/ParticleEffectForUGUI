using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    /// <summary>
    /// Render maskable and sortable particle effect ,without Camera, RenderTexture or Canvas.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIParticle : MaskableGraphic
    {
        //################################
        // Serialize Members.
        //################################
        [Tooltip("The ParticleSystem rendered by CanvasRenderer")] [SerializeField]
        ParticleSystem m_ParticleSystem;

        [Tooltip("The UIParticle to render trail effect")] [SerializeField]
        internal UIParticle m_TrailParticle;

        [HideInInspector] [SerializeField] bool m_IsTrail = false;

        [Tooltip("Ignore canvas scaler")] [SerializeField]
        bool m_IgnoreCanvasScaler = false;

        [Tooltip("Ignore parent scale")] [SerializeField]
        bool m_IgnoreParent = false;

        [Tooltip("Particle effect scale")] [SerializeField]
        float m_Scale = 0;

        [Tooltip("Animatable material properties. If you want to change the material properties of the ParticleSystem in Animation, enable it.")] [SerializeField]
        internal AnimatableProperty[] m_AnimatableProperties = new AnimatableProperty[0];

        [Tooltip("Particle effect scale")] [SerializeField]
        internal Vector3 m_Scale3D = Vector3.one;

        private readonly Material[] _maskMaterials = new Material[2];
        private DrivenRectTransformTracker _tracker;
        private Mesh _bakedMesh;
        private ParticleSystemRenderer _renderer;
        private int _cachedSharedMaterialId;
        private int _cachedTrailMaterialId;
        private bool _cachedSpritesModeAndHasTrail;

        //################################
        // Public/Protected Members.
        //################################
        /// <summary>
        /// Should this graphic be considered a target for raycasting?
        /// </summary>
        public override bool raycastTarget
        {
            get { return false; }
            set { base.raycastTarget = value; }
        }

        /// <summary>
        /// Cached ParticleSystem.
        /// </summary>
        public ParticleSystem cachedParticleSystem
        {
            get { return m_ParticleSystem ? m_ParticleSystem : (m_ParticleSystem = GetComponent<ParticleSystem>()); }
        }

        /// <summary>
        /// Cached ParticleSystem.
        /// </summary>
        internal ParticleSystemRenderer cachedRenderer
        {
            get { return _renderer; }
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
            get { return m_Scale3D.x; }
            set { m_Scale3D.Set(value, value, value); }
        }

        /// <summary>
        /// Particle effect scale.
        /// </summary>
        public Vector3 scale3D
        {
            get { return m_Scale3D; }
            set { m_Scale3D = value; }
        }

        internal bool isTrailParticle
        {
            get { return m_IsTrail; }
        }

        internal bool isSpritesMode
        {
            get { return textureSheetAnimationModule.enabled && textureSheetAnimationModule.mode == ParticleSystemAnimationMode.Sprites; }
        }

        private bool isSpritesModeAndHasTrail
        {
            get { return isSpritesMode && trailModule.enabled; }
        }

        private ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule
        {
            get { return cachedParticleSystem.textureSheetAnimation; }
        }

        internal ParticleSystem.TrailModule trailModule
        {
            get { return cachedParticleSystem.trails; }
        }

        internal ParticleSystem.MainModule mainModule
        {
            get { return cachedParticleSystem.main; }
        }

        public bool isValid
        {
            get { return m_ParticleSystem && _renderer && canvas; }
        }

        public Mesh bakedMesh
        {
            get { return _bakedMesh; }
        }

        protected override void UpdateMaterial()
        {
            if (!_renderer) return;

            if (!isSpritesMode) // Non sprite mode: canvas renderer has main and trail materials.
            {
                canvasRenderer.materialCount = trailModule.enabled ? 2 : 1;
                canvasRenderer.SetMaterial(GetModifiedMaterial(_renderer.sharedMaterial, 0), 0);
                if (trailModule.enabled)
                    canvasRenderer.SetMaterial(GetModifiedMaterial(_renderer.trailMaterial, 1), 1);
            }
            else if (isTrailParticle) // Sprite mode (Trail): canvas renderer has trail material.
            {
                canvasRenderer.materialCount = 1;
                canvasRenderer.SetMaterial(GetModifiedMaterial(_renderer.trailMaterial, 0), 0);
            }
            else // Sprite mode (Main): canvas renderer has main material.
            {
                canvasRenderer.materialCount = 1;
                canvasRenderer.SetMaterial(GetModifiedMaterial(_renderer.sharedMaterial, 0), 0);
            }
        }

        private Material GetModifiedMaterial(Material baseMaterial, int index)
        {
            if (!baseMaterial || 1 < index || !isActiveAndEnabled) return null;

            var hasAnimatableProperties = 0 < m_AnimatableProperties.Length && index == 0;
            if (hasAnimatableProperties || isTrailParticle)
                baseMaterial = new Material(baseMaterial);

            var baseMat = baseMaterial;
            if (m_ShouldRecalculateStencil)
            {
                m_ShouldRecalculateStencil = false;

                if (maskable)
                {
                    var sortOverrideCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
                    m_StencilValue = MaskUtilities.GetStencilDepth(transform, sortOverrideCanvas) + index;
                }
                else
                {
                    m_StencilValue = 0;
                }
            }

            var component = GetComponent<Mask>();
            if (m_StencilValue <= 0 || (component != null && component.IsActive())) return baseMat;

            var stencilId = (1 << m_StencilValue) - 1;
            var maskMaterial = StencilMaterial.Add(baseMat, stencilId, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, stencilId, 0);
            StencilMaterial.Remove(_maskMaterials[index]);
            _maskMaterials[index] = maskMaterial;
            baseMat = _maskMaterials[index];

            return baseMat;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            UpdateVersionIfNeeded();

            _tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);

            // Initialize.
            _renderer = cachedParticleSystem ? cachedParticleSystem.GetComponent<ParticleSystemRenderer>() : null;
            if (_renderer != null)
                _renderer.enabled = false;

            CheckMaterials();

            // Create objects.
            _bakedMesh = new Mesh();
            _bakedMesh.MarkDynamic();

            MeshHelper.Register();
            BakingCamera.Register();
            UIParticleUpdater.Register(this);

            base.OnEnable();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            _tracker.Clear();

            // Destroy object.
            DestroyImmediate(_bakedMesh);
            _bakedMesh = null;

            MeshHelper.Unregister();
            BakingCamera.Unregister();
            UIParticleUpdater.Unregister(this);

            CheckMaterials();

            // Remove mask materials.
            for (var i = 0; i < _maskMaterials.Length; i++)
            {
                StencilMaterial.Remove(_maskMaterials[i]);
                _maskMaterials[i] = null;
            }

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


        //################################
        // Private Members.
        //################################
        private static bool HasMaterialChanged(Material material, ref int current)
        {
            var old = current;
            current = material ? material.GetInstanceID() : 0;
            return current != old;
        }

        internal void UpdateTrailParticle()
        {
            // Should have a UIParticle for trail particle?
            if (isActiveAndEnabled && isValid && !isTrailParticle && isSpritesModeAndHasTrail)
            {
                if (!m_TrailParticle)
                {
                    // Create new UIParticle for trail particle
                    m_TrailParticle = new GameObject("[UIParticle] Trail").AddComponent<UIParticle>();
                    var trans = m_TrailParticle.transform;
                    trans.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    trans.localScale = Vector3.one;
                    trans.SetParent(transform, false);

                    m_TrailParticle._renderer = _renderer;
                    m_TrailParticle.m_ParticleSystem = m_ParticleSystem;
                    m_TrailParticle.m_IsTrail = true;
                }

                m_TrailParticle.gameObject.hideFlags = HideFlags.DontSave;
            }
            else if (m_TrailParticle)
            {
                // Destroy a UIParticle for trail particle.
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(m_TrailParticle.gameObject);
                else
#endif
                    Destroy(m_TrailParticle.gameObject);

                m_TrailParticle = null;
            }
        }

        internal void CheckMaterials()
        {
            if (!_renderer) return;

            var matChanged = HasMaterialChanged(_renderer.sharedMaterial, ref _cachedSharedMaterialId);
            var matChanged2 = HasMaterialChanged(_renderer.trailMaterial, ref _cachedTrailMaterialId);
            var modeChanged = _cachedSpritesModeAndHasTrail != isSpritesModeAndHasTrail;
            _cachedSpritesModeAndHasTrail = isSpritesModeAndHasTrail;

            if (matChanged || matChanged2 || modeChanged)
                SetMaterialDirty();
        }

        private void UpdateVersionIfNeeded()
        {
            if (Mathf.Approximately(m_Scale, 0)) return;

            var parent = GetComponentInParent<UIParticle>();
            if (m_IgnoreParent || !parent)
                scale3D = m_Scale * transform.localScale;
            else
                scale3D = transform.localScale;
            m_Scale = 0;
        }
    }
}
