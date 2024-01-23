using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Coffee.UIParticleExtensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[assembly: InternalsVisibleTo("Coffee.UIParticle.Editor")]

namespace Coffee.UIExtensions
{
    /// <summary>
    /// Render maskable and sortable particle effect ,without Camera, RenderTexture or Canvas.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIParticle : MaskableGraphic, ISerializationCallbackReceiver
    {
        public enum AutoScalingMode
        {
            None,
            UIParticle,
            Transform
        }

        public enum MeshSharing
        {
            None,
            Auto,
            Primary,
            PrimarySimulator,
            Replica
        }

        public enum PositionMode
        {
            Relative,
            Absolute
        }

        [HideInInspector]
        [SerializeField]
        internal bool m_IsTrail;

        [HideInInspector]
        [FormerlySerializedAs("m_IgnoreParent")]
        [SerializeField]
        private bool m_IgnoreCanvasScaler;

        [HideInInspector]
        [SerializeField]
        private bool m_AbsoluteMode;

        [Tooltip("Particle effect scale")]
        [SerializeField]
        private Vector3 m_Scale3D = new Vector3(10, 10, 10);

        [Tooltip("Animatable material properties.\n" +
                 "If you want to change the material properties of the ParticleSystem in Animation, enable it.")]
        [SerializeField]
        internal AnimatableProperty[] m_AnimatableProperties = new AnimatableProperty[0];

        [Tooltip("Particles")]
        [SerializeField]
        private List<ParticleSystem> m_Particles = new List<ParticleSystem>();

        [Tooltip("Mesh sharing.\n" +
                 "None: disable mesh sharing.\n" +
                 "Auto: automatically select Primary/Replica.\n" +
                 "Primary: provides particle simulation results to the same group.\n" +
                 "Primary Simulator: Primary, but do not render the particle (simulation only).\n" +
                 "Replica: render simulation results provided by the primary.")]
        [SerializeField]
        private MeshSharing m_MeshSharing = MeshSharing.None;

        [Tooltip("Mesh sharing group ID.\n" +
                 "If non-zero is specified, particle simulation results are shared within the group.")]
        [SerializeField]
        private int m_GroupId;

        [SerializeField]
        private int m_GroupMaxId;

        [Tooltip("Relative: The particles will be emitted from the scaled position of ParticleSystem.\n" +
                 "Absolute: The particles will be emitted from the world position of ParticleSystem.")]
        [SerializeField]
        private PositionMode m_PositionMode = PositionMode.Relative;

        [SerializeField]
        [Tooltip("Prevent the root-Canvas scale from affecting the hierarchy-scaled ParticleSystem.")]
        private bool m_AutoScaling = true;

        [SerializeField]
        [Tooltip("Transform: Transform.lossyScale (=world scale) will be set to (1, 1, 1)." +
                 "UIParticle: UIParticle.scale will be adjusted.")]
        private AutoScalingMode m_AutoScalingMode = AutoScalingMode.Transform;

        [SerializeField]
        private bool m_ResetScaleOnEnable;

        private readonly List<UIParticleRenderer> _renderers = new List<UIParticleRenderer>();
        private int _groupId;
        private Camera _orthoCamera;
        private DrivenRectTransformTracker _tracker;

        /// <summary>
        /// Should this graphic be considered a target for ray-casting?
        /// </summary>
        public override bool raycastTarget
        {
            get => false;
            set { }
        }

        /// <summary>
        /// Mesh sharing.
        /// None: disable mesh sharing.
        /// Auto: automatically select Primary/Replica.
        /// Primary: provides particle simulation results to the same group.
        /// Primary Simulator: Primary, but do not render the particle (simulation only).
        /// Replica: render simulation results provided by the primary.
        /// </summary>
        public MeshSharing meshSharing
        {
            get => m_MeshSharing;
            set => m_MeshSharing = value;
        }

        /// <summary>
        /// Mesh sharing group ID.
        /// If non-zero is specified, particle simulation results are shared within the group.
        /// </summary>
        public int groupId
        {
            get => _groupId;
            set
            {
                if (m_GroupId == value) return;
                m_GroupId = value;
                if (m_GroupId != m_GroupMaxId)
                {
                    ResetGroupId();
                }
            }
        }

        public int groupMaxId
        {
            get => m_GroupMaxId;
            set
            {
                if (m_GroupMaxId == value) return;
                m_GroupMaxId = value;
                ResetGroupId();
            }
        }

        /// <summary>
        /// Particle position mode.
        /// Relative: The particles will be emitted from the scaled position of the ParticleSystem.
        /// Absolute: The particles will be emitted from the world position of the ParticleSystem.
        /// </summary>
        public PositionMode positionMode
        {
            get => m_PositionMode;
            set => m_PositionMode = value;
        }

        /// <summary>
        /// Particle position mode.
        /// Relative: The particles will be emitted from the scaled position of the ParticleSystem.
        /// Absolute: The particles will be emitted from the world position of the ParticleSystem.
        /// </summary>
        public bool absoluteMode
        {
            get => m_PositionMode == PositionMode.Absolute;
            set => positionMode = value ? PositionMode.Absolute : PositionMode.Relative;
        }

        /// <summary>
        /// Prevents the root-Canvas scale from affecting the hierarchy-scaled ParticleSystem.
        /// </summary>
        [Obsolete("The autoScaling is now obsolete. Please use the autoScalingMode instead.", false)]
        public bool autoScaling
        {
            get => m_AutoScalingMode != AutoScalingMode.None;
            set => autoScalingMode = value ? AutoScalingMode.Transform : AutoScalingMode.None;
        }

        /// <summary>
        /// Auto scaling mode.
        /// Transform: Transform.lossyScale (=world scale) will be set to (1, 1, 1).
        /// UIParticle: UIParticle.scale will be adjusted.
        /// </summary>
        public AutoScalingMode autoScalingMode
        {
            get => m_AutoScalingMode;
            set
            {
                if (m_AutoScalingMode == value) return;
                m_AutoScalingMode = value;
                UpdateTracker();
            }
        }

        internal bool useMeshSharing => m_MeshSharing != MeshSharing.None;

        internal bool isPrimary =>
            m_MeshSharing == MeshSharing.Primary
            || m_MeshSharing == MeshSharing.PrimarySimulator;

        internal bool canSimulate =>
            m_MeshSharing == MeshSharing.None
            || m_MeshSharing == MeshSharing.Auto
            || m_MeshSharing == MeshSharing.Primary
            || m_MeshSharing == MeshSharing.PrimarySimulator;

        internal bool canRender =>
            m_MeshSharing == MeshSharing.None
            || m_MeshSharing == MeshSharing.Auto
            || m_MeshSharing == MeshSharing.Primary
            || m_MeshSharing == MeshSharing.Replica;

        /// <summary>
        /// Particle effect scale.
        /// </summary>
        public float scale
        {
            get => m_Scale3D.x;
            set => m_Scale3D = new Vector3(value, value, value);
        }

        /// <summary>
        /// Particle effect scale.
        /// </summary>
        public Vector3 scale3D
        {
            get => m_Scale3D;
            set => m_Scale3D = value;
        }

        /// <summary>
        /// Particle effect scale.
        /// </summary>
        public Vector3 scale3DForCalc => autoScalingMode == AutoScalingMode.UIParticle
            ? m_Scale3D.GetScaled(canvasScale)
            : m_Scale3D;

        public List<ParticleSystem> particles => m_Particles;

        /// <summary>
        /// Get all base materials to render.
        /// </summary>
        public IEnumerable<Material> materials
        {
            get
            {
                for (var i = 0; i < _renderers.Count; i++)
                {
                    var r = _renderers[i];
                    if (!r || !r.material) continue;
                    yield return r.material;
                }
            }
        }

        public override Material materialForRendering => null;

        /// <summary>
        /// Paused.
        /// </summary>
        public bool isPaused { get; private set; }

        public Vector3 parentScale { get; private set; }

        public Vector3 canvasScale { get; private set; }

        protected override void OnEnable()
        {
            ResetGroupId();
            UpdateTracker();
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

            // Reset scale for upgrade.
            if (m_ResetScaleOnEnable)
            {
                m_ResetScaleOnEnable = false;
                transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            UpdateTracker();
            UIParticleUpdater.Unregister(this);
            _renderers.ForEach(r => r.Reset());
            UnregisterDirtyMaterialCallback(UpdateRendererMaterial);

            base.OnDisable();
        }

        /// <summary>
        /// Callback for when properties have been changed by animation.
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateTracker();
        }
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_IgnoreCanvasScaler || m_AutoScaling)
            {
                m_IgnoreCanvasScaler = false;
                m_AutoScaling = false;
                m_AutoScalingMode = AutoScalingMode.Transform;
                m_ResetScaleOnEnable = true;

#if UNITY_EDITOR
                EditorApplication.delayCall += () =>
                {
                    if (!this || !gameObject || !transform || Application.isPlaying) return;
                    transform.localScale = Vector3.one;
                    m_ResetScaleOnEnable = false;
                    EditorUtility.SetDirty(this);
                };
#endif
            }

            if (m_AbsoluteMode)
            {
                m_AbsoluteMode = false;
                m_PositionMode = PositionMode.Absolute;
            }
        }

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

        public void StartEmission()
        {
            particles.Exec(p =>
            {
                var emission = p.emission;
                emission.enabled = true;
            });
        }

        public void StopEmission()
        {
            particles.Exec(p =>
            {
                var emission = p.emission;
                emission.enabled = false;
            });
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
                if (destroyOldParticles)
                {
                    Misc.Destroy(go);
                }
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
            root.GetComponentsInChildren(true, particles);
            particles.RemoveAll(x => x.GetComponentInParent<UIParticle>(true) != this);

            for (var i = 0; i < particles.Count; i++)
            {
                var ps = particles[i];
                var tsa = ps.textureSheetAnimation;
                if (tsa.mode == ParticleSystemAnimationMode.Sprites && tsa.uvChannelMask == 0)
                {
                    tsa.uvChannelMask = UVChannelFlags.UV0;
                }
            }

            RefreshParticles(particles);
        }

        public void RefreshParticles(List<ParticleSystem> particles)
        {
            // #246: Nullptr exceptions when using nested UIParticle components in hierarchy
            _renderers.Clear();
            foreach (Transform child in transform)
            {
                var uiParticleRenderer = child.GetComponent<UIParticleRenderer>();

                if (uiParticleRenderer != null)
                {
                    _renderers.Add(uiParticleRenderer);
                }
            }

            for (var i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].Reset(i);
            }

            var j = 0;
            for (var i = 0; i < particles.Count; i++)
            {
                var ps = particles[i];
                if (!ps) continue;
                GetRenderer(j++).Set(this, ps, false);
                if (ps.trails.enabled)
                {
                    GetRenderer(j++).Set(this, ps, true);
                }
            }
        }

        internal void UpdateTransformScale()
        {
            canvasScale = canvas.rootCanvas.transform.localScale.Inverse();
            parentScale = transform.parent.lossyScale;
            if (autoScalingMode != AutoScalingMode.Transform) return;

            var newScale = parentScale.Inverse();
            if (transform.localScale != newScale)
            {
                transform.localScale = newScale;
            }
        }

        internal void UpdateRenderers()
        {
            if (!isActiveAndEnabled) return;

            for (var i = 0; i < _renderers.Count; i++)
            {
                var r = _renderers[i];
                if (!r)
                {
                    RefreshParticles(particles);
                    break;
                }
            }

            var bakeCamera = GetBakeCamera();
            for (var i = 0; i < _renderers.Count; i++)
            {
                var r = _renderers[i];
                if (!r) continue;
                r.UpdateMesh(bakeCamera);
            }
        }

        internal void ResetGroupId()
        {
            _groupId = m_GroupId == m_GroupMaxId
                ? m_GroupId
                : Random.Range(m_GroupId, m_GroupMaxId + 1);
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

        private void UpdateRendererMaterial()
        {
            for (var i = 0; i < _renderers.Count; i++)
            {
                var r = _renderers[i];
                if (!r) continue;
                r.maskable = maskable;
                r.SetMaterialDirty();
            }
        }

        internal UIParticleRenderer GetRenderer(int index)
        {
            if (_renderers.Count <= index)
            {
                _renderers.Add(UIParticleRenderer.AddRenderer(this, index));
            }

            if (!_renderers[index])
            {
                _renderers[index] = UIParticleRenderer.AddRenderer(this, index);
            }

            return _renderers[index];
        }

        private Camera GetBakeCamera()
        {
            if (!canvas) return Camera.main;

            // When render mode is ScreenSpaceCamera or WorldSpace, use world camera.
            var root = canvas.rootCanvas;
            if (root.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                return root.worldCamera ? root.worldCamera : Camera.main;
            }

            // When render mode is ScreenSpaceOverlay, use ortho-camera.
            if (!_orthoCamera)
            {
                // Find existing ortho-camera.
                foreach (Transform child in transform)
                {
                    var cam = child.GetComponent<Camera>();
                    if (cam && cam.name == "[generated] UIParticleOverlayCamera")
                    {
                        _orthoCamera = cam;
                        break;
                    }
                }

                // Create ortho-camera.
                if (!_orthoCamera)
                {
                    var go = new GameObject("[generated] UIParticleOverlayCamera")
                    {
                        hideFlags = HideFlags.HideAndDontSave
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

        private void UpdateTracker()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (!enabled || !autoScaling || autoScalingMode != AutoScalingMode.Transform)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                _tracker.Clear();
            }
            else
            {
                _tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
            }
        }
    }
}
