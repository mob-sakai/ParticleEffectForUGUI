using System;
using System.Collections.Generic;
using Coffee.UIParticleExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace Coffee.UIExtensions
{
    [ExecuteAlways]
    public class UIParticleAttractor : MonoBehaviour
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
        private List<ParticleSystem> m_ParticleSystems;

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

        private UIParticle[] _uiParticles;

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

            if (!m_ParticleSystems.Contains(ps))
            {
                m_ParticleSystems.Add(ps);
                ApplyParticleSystems();
            }
        }

        public void RemoveParticleSystem(ParticleSystem ps)
        {
            if (m_ParticleSystems == null)
            {
                return;
            }

            if (m_ParticleSystems.Contains(ps))
            {
                m_ParticleSystems.Remove(ps);
                ApplyParticleSystems();
            }
        }

        private void OnEnable()
        {
            ApplyParticleSystems();
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
            if (m_ParticleSystems == null) return;

            for (var particleIndex = 0; particleIndex < this.m_ParticleSystems.Count; particleIndex++)
            {
                var particleSystem = m_ParticleSystems[particleIndex];
                if (particleSystem == null || !particleSystem.gameObject.activeInHierarchy) continue;

                var count = particleSystem.particleCount;
                if (count == 0) continue;

                var particles = ParticleSystemExtensions.GetParticleArray(count);
                particleSystem.GetParticles(particles, count);

                var uiParticle = this._uiParticles != null && particleIndex < _uiParticles.Length
                    ? _uiParticles[particleIndex]
                    : null;

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

        private void ApplyParticleSystems()
        {
            _uiParticles = null;
            if (m_ParticleSystems == null || m_ParticleSystems.Count == 0)
            {
                return;
            }

            _uiParticles = new UIParticle[m_ParticleSystems.Count];
            for (var i = 0; i < this.m_ParticleSystems.Count; i++)
            {
                var particleSystem = m_ParticleSystems[i];
                if (particleSystem == null) continue;

                var uiParticle = particleSystem.GetComponentInParent<UIParticle>(true);
                _uiParticles[i] = uiParticle.particles.Contains(particleSystem) ? uiParticle : null;
            }
        }
    }
}
