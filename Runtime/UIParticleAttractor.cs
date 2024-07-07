using System;
using System.Collections.Generic;
using Coffee.UIParticleExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace Coffee.UIExtensions
{
    [ExecuteAlways]
    public class UIParticleAttractor : MonoBehaviour, ISerializationCallbackReceiver
    {
        public enum Movement
        {
            Linear,
            Smooth,
            Sphere
        }

        public enum UpdateMode
        {
            Normal,
            UnscaledTime
        }

        [SerializeField]
        [HideInInspector]
        private ParticleSystem m_ParticleSystem;

        [SerializeField]
        private List<ParticleSystem> m_ParticleSystems = new List<ParticleSystem>();

        [Range(0.1f, 10f)]
        [SerializeField]
        private float m_DestinationRadius = 1;

        [Range(0f, 0.95f)]
        [SerializeField]
        private float m_DelayRate;

        [Range(0.001f, 100f)]
        [SerializeField]
        private float m_MaxSpeed = 1;

        [SerializeField]
        private Movement m_Movement;

        [SerializeField]
        private UpdateMode m_UpdateMode;

        [SerializeField]
        private UnityEvent m_OnAttracted;

        private List<UIParticle> _uiParticles = new List<UIParticle>();

        public float destinationRadius
        {
            get => m_DestinationRadius;
            set => m_DestinationRadius = Mathf.Clamp(value, 0.1f, 10f);
        }

        public float delay
        {
            get => m_DelayRate;
            set => m_DelayRate = value;
        }

        public float maxSpeed
        {
            get => m_MaxSpeed;
            set => m_MaxSpeed = value;
        }

        public Movement movement
        {
            get => m_Movement;
            set => m_Movement = value;
        }

        public UpdateMode updateMode
        {
            get => m_UpdateMode;
            set => m_UpdateMode = value;
        }

        public UnityEvent onAttracted
        {
            get => m_OnAttracted;
            set => m_OnAttracted = value;
        }

        /// <summary>
        /// The target ParticleSystems to attract. Use <see cref="AddParticleSystem"/> and
        /// <see cref="RemoveParticleSystem"/> to modify the list.
        /// </summary>
        public IReadOnlyList<ParticleSystem> particleSystems => m_ParticleSystems;

        public void AddParticleSystem(ParticleSystem ps)
        {
            if (m_ParticleSystems == null)
            {
                m_ParticleSystems = new List<ParticleSystem>();
            }

            var i = m_ParticleSystems.IndexOf(ps);
            if (0 <= i) return; // Already added: skip

            m_ParticleSystems.Add(ps);
            _uiParticles.Clear();
        }

        public void RemoveParticleSystem(ParticleSystem ps)
        {
            if (m_ParticleSystems == null)
            {
                return;
            }

            var i = m_ParticleSystems.IndexOf(ps);
            if (i < 0) return; // Not found. skip

            m_ParticleSystems.RemoveAt(i);
            _uiParticles.Clear();
        }

        private void Awake()
        {
            UpgradeIfNeeded();
        }

        private void OnEnable()
        {
            UIParticleUpdater.Register(this);
        }

        private void OnDisable()
        {
            UIParticleUpdater.Unregister(this);
        }

        private void OnDestroy()
        {
            _uiParticles = null;
            m_ParticleSystems = null;
        }

        internal void Attract()
        {
            // Collect UIParticle if needed (same size as m_ParticleSystems)
            CollectUIParticlesIfNeeded();

            for (var particleIndex = 0; particleIndex < this.m_ParticleSystems.Count; particleIndex++)
            {
                var particleSystem = m_ParticleSystems[particleIndex];

                // Skip: The ParticleSystem is not active
                if (particleSystem == null || !particleSystem.gameObject.activeInHierarchy) continue;

                // Skip: No active particles
                var count = particleSystem.particleCount;
                if (count == 0) continue;

                var particles = ParticleSystemExtensions.GetParticleArray(count);
                particleSystem.GetParticles(particles, count);

                var uiParticle = _uiParticles[particleIndex];
                var dstPos = this.GetDestinationPosition(uiParticle, particleSystem);
                for (var i = 0; i < count; i++)
                {
                    // Attracted
                    var p = particles[i];
                    if (0f < p.remainingLifetime && Vector3.Distance(p.position, dstPos) < this.m_DestinationRadius)
                    {
                        p.remainingLifetime = 0f;
                        particles[i] = p;

                        if (this.m_OnAttracted != null)
                        {
                            try
                            {
                                this.m_OnAttracted.Invoke();
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }

                        continue;
                    }

                    // Calc attracting time
                    var delayTime = p.startLifetime * this.m_DelayRate;
                    var duration = p.startLifetime - delayTime;
                    var time = Mathf.Max(0, p.startLifetime - p.remainingLifetime - delayTime);

                    // Delay
                    if (time <= 0) continue;

                    // Attract
                    p.position = this.GetAttractedPosition(p.position, dstPos, duration, time);
                    p.velocity *= 0.5f;
                    particles[i] = p;
                }

                particleSystem.SetParticles(particles, count);
            }
        }

        private Vector3 GetDestinationPosition(UIParticle uiParticle, ParticleSystem particleSystem)
        {
            var isUI = uiParticle && uiParticle.enabled;
            var psPos = particleSystem.transform.position;
            var attractorPos = transform.position;
            var dstPos = attractorPos;
            var isLocalSpace = particleSystem.IsLocalSpace();

            if (isLocalSpace)
            {
                dstPos = particleSystem.transform.InverseTransformPoint(dstPos);
            }

            if (isUI)
            {
                var inverseScale = uiParticle.parentScale.Inverse();
                var scale3d = uiParticle.scale3DForCalc;
                dstPos = dstPos.GetScaled(inverseScale, scale3d.Inverse());

                // Relative mode
                if (uiParticle.positionMode == UIParticle.PositionMode.Relative)
                {
                    var diff = uiParticle.transform.position - psPos;
                    diff.Scale(scale3d - inverseScale);
                    diff.Scale(scale3d.Inverse());
                    dstPos += diff;
                }

#if UNITY_EDITOR
                if (!Application.isPlaying && !isLocalSpace)
                {
                    dstPos += psPos - psPos.GetScaled(inverseScale, scale3d.Inverse());
                }
#endif
            }

            return dstPos;
        }

        private Vector3 GetAttractedPosition(Vector3 current, Vector3 target, float duration, float time)
        {
            var speed = m_MaxSpeed;
            switch (m_UpdateMode)
            {
                case UpdateMode.Normal:
                    speed *= 60 * Time.deltaTime;
                    break;
                case UpdateMode.UnscaledTime:
                    speed *= 60 * Time.unscaledDeltaTime;
                    break;
            }

            switch (m_Movement)
            {
                case Movement.Linear:
                    speed /= duration;
                    break;
                case Movement.Smooth:
                    target = Vector3.Lerp(current, target, time / duration);
                    break;
                case Movement.Sphere:
                    target = Vector3.Slerp(current, target, time / duration);
                    break;
            }

            return Vector3.MoveTowards(current, target, speed);
        }

        private void CollectUIParticlesIfNeeded()
        {
            if (m_ParticleSystems.Count == 0 || _uiParticles.Count != 0) return;

            // Expand capacity
            if (_uiParticles.Capacity < m_ParticleSystems.Capacity)
            {
                _uiParticles.Capacity = m_ParticleSystems.Capacity;
            }

            // Find UIParticle that controls the ParticleSystem
            for (var i = 0; i < m_ParticleSystems.Count; i++)
            {
                var ps = m_ParticleSystems[i];
                if (ps == null)
                {
                    _uiParticles.Add(null);
                    continue;
                }

                var uiParticle = ps.GetComponentInParent<UIParticle>(true);
                _uiParticles.Add(uiParticle.particles.Contains(ps) ? uiParticle : null);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _uiParticles.Clear();
        }
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UpgradeIfNeeded();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }

        private void UpgradeIfNeeded()
        {
            // Multiple ParticleSystems support: from 'm_ParticleSystem' to 'm_ParticleSystems'
            if (m_ParticleSystem != null)
            {
                if (!m_ParticleSystems.Contains(m_ParticleSystem))
                {
                    m_ParticleSystems.Add(m_ParticleSystem);
                }

                m_ParticleSystem = null;
                Debug.Log($"Upgraded!");
            }
        }
    }
}
