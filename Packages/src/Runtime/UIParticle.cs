using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Coffee.UIParticleInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
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
    public class UIParticle : UIBehaviour
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

        [SerializeField]
        private bool m_Maskable = true;

        [HideInInspector]
        [SerializeField]
        [Obsolete]
        internal bool m_IsTrail;

        [HideInInspector]
        [SerializeField]
        [Obsolete]
        internal bool m_AbsoluteMode;

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

        [FormerlySerializedAs("m_IgnoreParent")]
        [FormerlySerializedAs("m_IgnoreCanvasScaler")]
        [SerializeField]
        [Tooltip("Prevent the root-Canvas scale from affecting the hierarchy-scaled ParticleSystem.")]
        [Obsolete]
        internal bool m_AutoScaling;

        [SerializeField]
        [Tooltip("Transform: Transform.lossyScale (=world scale) will be set to (1, 1, 1)." +
                 "UIParticle: UIParticle.scale will be adjusted.")]
        private AutoScalingMode m_AutoScalingMode = AutoScalingMode.Transform;

        private readonly List<UIParticleRenderer> _renderers = new List<UIParticleRenderer>();
        private Canvas _canvas;
        private int _groupId;
        private Camera _orthoCamera;
        private DrivenRectTransformTracker _tracker;

        public RectTransform rectTransform => transform as RectTransform;

        public Canvas canvas
        {
            get
            {
                if (_canvas) return _canvas;

                var tr = transform;
                while (tr && !_canvas)
                {
                    if (tr.TryGetComponent(out _canvas)) return _canvas;
                    tr = tr.parent;
                }

                return null;
            }
        }

        /// <summary>
        /// Does this graphic allow masking.
        /// </summary>
        public bool maskable
        {
            get => m_Maskable;
            set
            {
                if (value == m_Maskable) return;
                m_Maskable = value;
                UpdateRendererMaterial();
            }
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

            //
            if (0 < particles.Count)
            {
                RefreshParticles(particles);
            }
            else
            {
                RefreshParticles();
            }

            UpdateRendererMaterial();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable()
        {
            UpdateTracker();
            UIParticleUpdater.Unregister(this);
            _renderers.ForEach(r => r.Reset());
            _canvas = null;
        }

        /// <summary>
        /// Called when the state of the parent Canvas is changed.
        /// </summary>
        protected override void OnCanvasHierarchyChanged()
        {
            _canvas = null;
        }

        /// <summary>
        /// Callback for when properties have been changed by animation.
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
        }

        /// <summary>
        /// This function is called when a direct or indirect parent of the transform of the GameObject has changed.
        /// </summary>
        protected override void OnTransformParentChanged()
        {
            _canvas = null;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateTracker();
            UpdateRendererMaterial();
        }
#endif

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
            _orthoCamera.orthographicSize = 10;
            _orthoCamera.transform.SetPositionAndRotation(new Vector3(0, 0, -1000), Quaternion.identity);
            _orthoCamera.orthographic = true;
            _orthoCamera.farClipPlane = 2000f;

            return _orthoCamera;
        }

        private void UpdateTracker()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (!enabled || autoScalingMode != AutoScalingMode.Transform)
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
