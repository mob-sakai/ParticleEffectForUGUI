using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Coffee.UIParticleExtensions;
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
        [Obsolete]
        internal bool m_IsTrail;

        [HideInInspector]
        [FormerlySerializedAs("m_IgnoreParent")]
        [SerializeField]
        [Obsolete]
        private bool m_IgnoreCanvasScaler;

        [HideInInspector]
        [SerializeField]
        [Obsolete]
        internal bool m_AbsoluteMode;

        [Tooltip("Scale the rendering particles. When the `3D` toggle is enabled, 3D scale (x, y, z) is supported.")]
        [SerializeField]
        private Vector3 m_Scale3D = new Vector3(10, 10, 10);

        [Tooltip("If you want to update material properties (e.g. _MainTex_ST, _Color) in AnimationClip, " +
                 "use this to mark as animatable.")]
        [SerializeField]
        internal AnimatableProperty[] m_AnimatableProperties = new AnimatableProperty[0];

        [Tooltip("Particles")]
        [SerializeField]
        private List<ParticleSystem> m_Particles = new List<ParticleSystem>();

        [Tooltip("Particle simulation results are shared within the same group. " +
                 "A large number of the same effects can be displayed with a small load.\n" +
                 "None: Disable mesh sharing.\n" +
                 "Auto: Automatically select Primary/Replica.\n" +
                 "Primary: Provides particle simulation results to the same group.\n" +
                 "Primary Simulator: Primary, but do not render the particle (simulation only).\n" +
                 "Replica: Render simulation results provided by the primary.")]
        [SerializeField]
        private MeshSharing m_MeshSharing = MeshSharing.None;

        [Tooltip("Mesh sharing group ID.\n" +
                 "If non-zero is specified, particle simulation results are shared within the group.")]
        [SerializeField]
        private int m_GroupId;

        [SerializeField]
        private int m_GroupMaxId;

        [Tooltip("Emission position mode.\n" +
                 "Relative: The particles will be emitted from the scaled position.\n" +
                 "Absolute: The particles will be emitted from the world position.")]
        [SerializeField]
        private PositionMode m_PositionMode = PositionMode.Relative;

        [SerializeField]
        [Obsolete]
        internal bool m_AutoScaling;

        [SerializeField]
        [Tooltip(
            "How to automatically adjust when the Canvas scale is changed by the screen size or reference resolution.\n" +
            "None: Do nothing.\n" +
            "Transform: Transform.lossyScale (=world scale) will be set to (1, 1, 1).\n" +
            "UIParticle: UIParticle.scale will be adjusted.")]
        private AutoScalingMode m_AutoScalingMode = AutoScalingMode.Transform;

        [SerializeField]
        [Tooltip("Use a custom view.\n" +
                 "Use this if the particles are not displayed correctly due to min/max particle size.")]
        private bool m_UseCustomView;

        [SerializeField]
        [Tooltip("Custom view size.\n" +
                 "Change the bake view size.")]
        private float m_CustomViewSize = 10;

        private readonly List<UIParticleRenderer> _renderers = new List<UIParticleRenderer>();
        private Camera _bakeCamera;
        private Canvas _canvas;
        private int _groupId;
        private bool _isScaleStored;
        private Vector3 _storedScale;
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
        /// Particle simulation results are shared within the same group.
        /// A large number of the same effects can be displayed with a small load.
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
        /// Emission position mode.
        /// Relative: The particles will be emitted from the scaled position.
        /// Absolute: The particles will be emitted from the world position.
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
        [Obsolete("The absoluteMode is now obsolete. Please use the autoScalingMode instead.", false)]
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
        /// How to automatically adjust when the Canvas scale is changed by the screen size or reference resolution.
        /// <para/>
        /// None: Do nothing.
        /// <para/>
        /// Transform: Transform.lossyScale (=world scale) will be set to (1, 1, 1).
        /// <para/>
        /// UIParticle: UIParticle.scale will be adjusted.
        /// </summary>
        public AutoScalingMode autoScalingMode
        {
            get => m_AutoScalingMode;
            set
            {
                if (m_AutoScalingMode == value) return;
                m_AutoScalingMode = value;

                if (autoScalingMode != AutoScalingMode.Transform && _isScaleStored)
                {
                    transform.localScale = _storedScale;
                    _isScaleStored = false;
                }
            }
        }

        /// <summary>
        /// Use a custom view.
        /// Use this if the particles are not displayed correctly due to min/max particle size.
        /// </summary>
        public bool useCustomView
        {
            get => m_UseCustomView;
            set => m_UseCustomView = value;
        }

        /// <summary>
        /// Custom view size.
        /// Change the bake view size.
        /// </summary>
        public float customViewSize
        {
            get => m_CustomViewSize;
            set => m_CustomViewSize = Mathf.Max(0.1f, value);
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
        public Vector3 scale3DForCalc => autoScalingMode == AutoScalingMode.Transform
            ? m_Scale3D
            : m_Scale3D.GetScaled(canvasScale, transform.localScale);

        public List<ParticleSystem> particles => m_Particles;

        /// <summary>
        /// Paused.
        /// </summary>
        public bool isPaused { get; private set; }

        public Vector3 parentScale { get; private set; }

        public Vector3 canvasScale { get; private set; }

        protected override void OnEnable()
        {
            _isScaleStored = false;
            ResetGroupId();
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

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            _tracker.Clear();
            if (autoScalingMode == AutoScalingMode.Transform && _isScaleStored)
            {
                transform.localScale = _storedScale;
            }

            _isScaleStored = false;
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

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
#pragma warning disable CS0612 // Type or member is obsolete
            if (m_IgnoreCanvasScaler || m_AutoScaling)
            {
                m_IgnoreCanvasScaler = false;
                m_AutoScaling = false;
                m_AutoScalingMode = AutoScalingMode.Transform;
            }

            if (m_AbsoluteMode)
            {
                m_AbsoluteMode = false;
                m_PositionMode = PositionMode.Absolute;
            }
#pragma warning restore CS0612 // Type or member is obsolete
        }

        /// <summary>
        /// Play the ParticleSystems.
        /// </summary>
        public void Play()
        {
            particles.Exec(p => p.Simulate(0, false, true));
            isPaused = false;
        }

        /// <summary>
        /// Pause the ParticleSystems.
        /// </summary>
        public void Pause()
        {
            particles.Exec(p => p.Pause());
            isPaused = true;
        }

        /// <summary>
        /// Unpause the ParticleSystems.
        /// </summary>
        public void Resume()
        {
            isPaused = false;
        }

        /// <summary>
        /// Stop the ParticleSystems.
        /// </summary>
        public void Stop()
        {
            particles.Exec(p => p.Stop());
            isPaused = true;
        }

        /// <summary>
        /// Start emission of the ParticleSystems.
        /// </summary>
        public void StartEmission()
        {
            particles.Exec(p =>
            {
                var emission = p.emission;
                emission.enabled = true;
            });
        }

        /// <summary>
        /// Stop emission of the ParticleSystems.
        /// </summary>
        public void StopEmission()
        {
            particles.Exec(p =>
            {
                var emission = p.emission;
                emission.enabled = false;
            });
        }

        /// <summary>
        /// Clear the particles of the ParticleSystems.
        /// </summary>
        public void Clear()
        {
            particles.Exec(p => p.Clear());
            isPaused = true;
        }

        /// <summary>
        /// Get all base materials to render.
        /// </summary>
        public void GetMaterials(List<Material> result)
        {
            if (result == null) return;

            for (var i = 0; i < _renderers.Count; i++)
            {
                var r = _renderers[i];
                if (!r || !r.material) continue;
                result.Add(r.material);
            }
        }

        /// <summary>
        /// Refresh UIParticle using the ParticleSystem instance.
        /// </summary>
        public void SetParticleSystemInstance(GameObject instance)
        {
            SetParticleSystemInstance(instance, true);
        }

        /// <summary>
        /// Refresh UIParticle using the ParticleSystem instance.
        /// </summary>
        public void SetParticleSystemInstance(GameObject instance, bool destroyOldParticles)
        {
            if (!instance) return;

            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var go = transform.GetChild(i).gameObject;
                if (go.TryGetComponent<Camera>(out var cam) && cam == _bakeCamera) continue;
                if (go.TryGetComponent<UIParticleRenderer>(out var _)) continue;

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

        /// <summary>
        /// Refresh UIParticle using the prefab.
        /// The prefab is automatically instantiated.
        /// </summary>
        public void SetParticleSystemPrefab(GameObject prefab)
        {
            if (!prefab) return;

            SetParticleSystemInstance(Instantiate(prefab.gameObject), true);
        }

        /// <summary>
        /// Refresh UIParticle.
        /// Collect ParticleSystems under the GameObject and refresh the UIParticle.
        /// </summary>
        public void RefreshParticles()
        {
            RefreshParticles(gameObject);
        }

        /// <summary>
        /// Refresh UIParticle.
        /// Collect ParticleSystems under the GameObject and refresh the UIParticle.
        /// </summary>
        private void RefreshParticles(GameObject root)
        {
            if (!root) return;
            root.GetComponentsInChildren(true, particles);
            for (var i = particles.Count - 1; 0 <= i; i--)
            {
                var ps = particles[i];
                if (!ps || ps.GetComponentInParent<UIParticle>(true) != this)
                {
                    particles.RemoveAt(i);
                }
            }

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

        /// <summary>
        /// Refresh UIParticle using a list of ParticleSystems.
        /// </summary>
        public void RefreshParticles(List<ParticleSystem> particleSystems)
        {
            // Collect children UIParticleRenderer components.
            // #246: Nullptr exceptions when using nested UIParticle components in hierarchy
            _renderers.Clear();
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.TryGetComponent(out UIParticleRenderer uiParticleRenderer))
                {
                    _renderers.Add(uiParticleRenderer);
                }
            }

            // Reset the UIParticleRenderer components.
            for (var i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].Reset(i);
            }

            // Set the ParticleSystem to the UIParticleRenderer. If the trail is enabled, set it additionally.
            var j = 0;
            for (var i = 0; i < particleSystems.Count; i++)
            {
                var ps = particleSystems[i];
                if (!ps) continue;
                GetRenderer(j++).Set(this, ps, false);

                // If the trail is enabled, set it additionally.
                if (ps.trails.enabled)
                {
                    GetRenderer(j++).Set(this, ps, true);
                }
            }
        }

        internal void UpdateTransformScale()
        {
            _tracker.Clear();
            canvasScale = canvas.rootCanvas.transform.localScale.Inverse();
            parentScale = transform.parent.lossyScale;
            if (autoScalingMode != AutoScalingMode.Transform)
            {
                if (_isScaleStored)
                {
                    transform.localScale = _storedScale;
                }

                _isScaleStored = false;
                return;
            }

            var currentScale = transform.localScale;
            if (!_isScaleStored)
            {
                _storedScale = currentScale.IsVisible() ? currentScale : Vector3.one;
                _isScaleStored = true;
            }

            _tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
            var newScale = parentScale.Inverse();
            if (currentScale != newScale)
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
                if (r) continue;

                RefreshParticles(particles);
                break;
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
            if (!useCustomView && canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.rootCanvas.worldCamera)
            {
                return canvas.rootCanvas.worldCamera;
            }

            if (_bakeCamera)
            {
                _bakeCamera.orthographicSize = useCustomView ? customViewSize : 10;
                return _bakeCamera;
            }

            // Find existing baking camera.
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<Camera>(out var cam)
                    && cam.name == "[generated] UIParticle BakingCamera")
                {
                    _bakeCamera = cam;
                    break;
                }
            }

            // Create baking camera.
            if (!_bakeCamera)
            {
                var go = new GameObject("[generated] UIParticle BakingCamera");
                go.SetActive(false);
                go.transform.SetParent(transform, false);
                _bakeCamera = go.AddComponent<Camera>();
            }

            // Setup baking camera.
            _bakeCamera.enabled = false;
            _bakeCamera.orthographicSize = useCustomView ? customViewSize : 10;
            _bakeCamera.transform.SetPositionAndRotation(new Vector3(0, 0, -1000), Quaternion.identity);
            _bakeCamera.orthographic = true;
            _bakeCamera.farClipPlane = 2000f;
            _bakeCamera.clearFlags = CameraClearFlags.Nothing;
            _bakeCamera.cullingMask = 0; // Nothing
            _bakeCamera.allowHDR = false;
            _bakeCamera.allowMSAA = false;
            _bakeCamera.renderingPath = RenderingPath.Forward;
            _bakeCamera.useOcclusionCulling = false;

            _bakeCamera.gameObject.SetActive(false);
            _bakeCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;

            return _bakeCamera;
        }
    }
}
