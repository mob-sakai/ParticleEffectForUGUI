#if UNITY_2019_3_11 || UNITY_2019_3_12 || UNITY_2019_3_13 || UNITY_2019_3_14 || UNITY_2019_3_15 || UNITY_2019_4_OR_NEWER
#define SERIALIZE_FIELD_MASKABLE
#endif
using System.Collections.Generic;
using System.Linq;
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
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIParticle : MaskableGraphic
    {
        public enum MeshSharing
        {
            None,
            Auto,
            Primary,
            PrimarySimulator,
            Reprica,
        }

        [HideInInspector][SerializeField] internal bool m_IsTrail = false;

        [Tooltip("Particle effect scale")]
        [SerializeField]
        private Vector3 m_Scale3D = new Vector3(10, 10, 10);

        [Tooltip("Animatable material properties. If you want to change the material properties of the ParticleSystem in Animation, enable it.")]
        [SerializeField]
        internal AnimatableProperty[] m_AnimatableProperties = new AnimatableProperty[0];

        [Tooltip("Particles")]
        [SerializeField]
        private List<ParticleSystem> m_Particles = new List<ParticleSystem>();

        [Tooltip("Mesh sharing.None: disable mesh sharing.\nAuto: automatically select Primary/Reprica.\nPrimary: provides particle simulation results to the same group.\nPrimary Simulator: Primary, but do not render the particle (simulation only).\nReprica: render simulation results provided by the primary.")]
        [SerializeField]
        private MeshSharing m_MeshSharing = MeshSharing.None;

        [Tooltip("Mesh sharing group ID. If non-zero is specified, particle simulation results are shared within the group.")]
        [SerializeField]
        private int m_GroupId = 0;

        [SerializeField]
        private int m_GroupMaxId = 0;

        [SerializeField]
        [Tooltip("The particles will be emitted at the ParticleSystem position.\nMove the UIParticle/ParticleSystem to move the particle.")]
        private bool m_AbsoluteMode = false;

        private List<UIParticleRenderer> m_Renderers = new List<UIParticleRenderer>();

#if !SERIALIZE_FIELD_MASKABLE
        [SerializeField] private bool m_Maskable = true;
#endif

        private DrivenRectTransformTracker _tracker;
        private Camera _orthoCamera;
        private int _groupId;

        /// <summary>
        /// Should this graphic be considered a target for raycasting?
        /// </summary>
        public override bool raycastTarget
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Mesh sharing.None: disable mesh sharing.
        /// Auto: automatically select Primary/Reprica.
        /// Primary: provides particle simulation results to the same group.
        /// Primary Simulator: Primary, but do not render the particle (simulation only).
        /// Reprica: render simulation results provided by the primary.
        /// </summary>
        public MeshSharing meshSharing
        {
            get { return m_MeshSharing; }
            set { m_MeshSharing = value; }
        }

        /// <summary>
        /// Mesh sharing group ID. If non-zero is specified, particle simulation results are shared within the group.
        /// </summary>
        public int groupId
        {
            get { return _groupId; }
            set
            {
                if (m_GroupId == value) return;
                m_GroupId = value;
                if (m_GroupId != m_GroupMaxId)
                    ResetGroupId();
            }
        }

        public int groupMaxId
        {
            get { return m_GroupMaxId; }
            set
            {
                if (m_GroupMaxId == value) return;
                m_GroupMaxId = value;
                ResetGroupId();
            }
        }

        /// <summary>
        /// Absolute particle position mode.
        /// The particles will be emitted at the ParticleSystem position.
        /// Move the UIParticle/ParticleSystem to move the particle.
        /// </summary>
        public bool absoluteMode
        {
            get { return m_AbsoluteMode; }
            set { m_AbsoluteMode = value; }
        }

        internal bool useMeshSharing
        {
            get { return m_MeshSharing != MeshSharing.None; }
        }

        internal bool isPrimary
        {
            get { return m_MeshSharing == MeshSharing.Primary || m_MeshSharing == MeshSharing.PrimarySimulator; }
        }

        internal bool canSimulate
        {
            get { return m_MeshSharing == MeshSharing.None || m_MeshSharing == MeshSharing.Auto || m_MeshSharing == MeshSharing.Primary || m_MeshSharing == MeshSharing.PrimarySimulator; }
        }

        internal bool canRender
        {
            get { return m_MeshSharing == MeshSharing.None || m_MeshSharing == MeshSharing.Auto || m_MeshSharing == MeshSharing.Primary || m_MeshSharing == MeshSharing.Reprica; }
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

        public List<ParticleSystem> particles
        {
            get { return m_Particles; }
        }

        /// <summary>
        /// Get all base materials to render.
        /// </summary>
        public IEnumerable<Material> materials
        {
            get
            {
                for (var i = 0; i < m_Renderers.Count; i++)
                {
                    if (!m_Renderers[i] || !m_Renderers[i].material) continue;
                    yield return m_Renderers[i].material;
                }
                yield break;
            }
        }

        public override Material materialForRendering
        {
            get { return null; }
        }

        /// <summary>
        /// Paused.
        /// </summary>
        public bool isPaused { get; internal set; }

        public void Play()
        {
            particles.Exec(p => p.Simulate(0, false, true));
            isPaused = false;
        }

        public void Pause()
        {
            particles.Exec(p => p.Pause());
            isPaused = true;
        }

        public void Resume()
        {
            isPaused = false;
        }

        public void Stop()
        {
            particles.Exec(p => p.Stop());
            isPaused = true;
        }

        public void Clear()
        {
            particles.Exec(p => p.Clear());
            isPaused = true;
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

        private void RefreshParticles(GameObject root)
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

            RefreshParticles(particles);
        }

        public void RefreshParticles(List<ParticleSystem> particles)
        {
            GetComponentsInChildren(m_Renderers);

            var j = 0;
            for (var i = 0; i < particles.Count; i++)
            {
                if (!particles[i]) continue;
                GetRenderer(j++).Set(this, particles[i], false);
                if (particles[i].trails.enabled)
                {
                    GetRenderer(j++).Set(this, particles[i], true);
                }
            }

            for (; j < m_Renderers.Count; j++)
            {
                GetRenderer(j).Clear(j);
            }
        }

        internal void UpdateTransformScale()
        {
            //var newScale = Vector3.one;
            //if (uiScaling)
            //{
            //    newScale = transform.parent.lossyScale.Inverse();
            //}
            var newScale = transform.parent.lossyScale.Inverse();
            if (transform.localScale != newScale)
            {
                transform.localScale = newScale;
            }
        }

        internal void UpdateRenderers()
        {
            if (!isActiveAndEnabled) return;

            if (m_Renderers.Any(x => !x))
            {
                RefreshParticles(particles);
            }

            var bakeCamera = GetBakeCamera();
            for (var i = 0; i < m_Renderers.Count; i++)
            {
                if (!m_Renderers[i]) continue;
                m_Renderers[i].UpdateMesh(bakeCamera);
            }
        }

        internal void UpdateParticleCount()
        {
            for (var i = 0; i < m_Renderers.Count; i++)
            {
                if (!m_Renderers[i]) continue;
                m_Renderers[i].UpdateParticleCount();
            }
        }

        protected override void OnEnable()
        {
#if !SERIALIZE_FIELD_MASKABLE
            maskable = m_Maskable;
#endif
            ResetGroupId();
            _tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
            UIParticleUpdater.Register(this);
            RegisterDirtyMaterialCallback(UpdateRendererMaterial);

            if (0 < particles.Count)
            {
                RefreshParticles(particles);
            }
            else
            {
                RefreshParticles();
            }

            base.OnEnable();
        }

        internal void ResetGroupId()
        {
            if (m_GroupId == m_GroupMaxId)
            {
                _groupId = m_GroupId;
            }
            else
            {
                _groupId = Random.Range(m_GroupId, m_GroupMaxId + 1);
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            _tracker.Clear();
            UIParticleUpdater.Unregister(this);
            m_Renderers.ForEach(r => r.Clear());
            UnregisterDirtyMaterialCallback(UpdateRendererMaterial);

            base.OnDisable();
        }

        protected override void UpdateMaterial()
        {
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

        private void UpdateRendererMaterial()
        {
            for (var i = 0; i < m_Renderers.Count; i++)
            {
                if (!m_Renderers[i]) continue;
                m_Renderers[i].maskable = maskable;
                m_Renderers[i].SetMaterialDirty();
            }
        }

        internal UIParticleRenderer GetRenderer(int index)
        {
            if (m_Renderers.Count <= index)
            {
                m_Renderers.Add(UIParticleRenderer.AddRenderer(this, index));
            }
            if (!m_Renderers[index])
            {
                m_Renderers[index] = UIParticleRenderer.AddRenderer(this, index);
            }
            return m_Renderers[index];
        }

        private Camera GetBakeCamera()
        {
            if (!canvas) return Camera.main;

            // World camera.
            var root = canvas.rootCanvas;
            if (root.renderMode != RenderMode.ScreenSpaceOverlay) return root.worldCamera ? root.worldCamera : Camera.main;

            // Create ortho-camera.
            if (!_orthoCamera)
            {
                _orthoCamera = GetComponentInChildren<Camera>();
                if (!_orthoCamera)
                {
                    var go = new GameObject("UIParticleOverlayCamera")
                    {
                        hideFlags = HideFlags.DontSave,
                    };
                    go.SetActive(false);
                    go.transform.SetParent(transform, false);
                    _orthoCamera = go.AddComponent<Camera>();
                    _orthoCamera.enabled = false;
                }
            }

            // 
            var size = ((RectTransform)root.transform).rect.size;
            _orthoCamera.orthographicSize = Mathf.Max(size.x, size.y) * root.scaleFactor;
            _orthoCamera.transform.SetPositionAndRotation(new Vector3(0, 0, -1000), Quaternion.identity);
            _orthoCamera.orthographic = true;
            _orthoCamera.farClipPlane = 2000f;

            return _orthoCamera;
        }
    }
}
