using UnityEngine;
using Coffee.UIParticleExtensions;
using UnityEngine.Events;
using System;

namespace Coffee.UIExtensions
{
    [ExecuteAlways]
    public class UIParticleAttractor : MonoBehaviour
    {
        public enum Movement
        {
            Linear,
            Smooth,
            Sphere,
        }

        [SerializeField]
        private ParticleSystem m_ParticleSystem;

        [Range(0.1f, 10f)]
        [SerializeField]
        private float m_DestinationRadius = 1;

        [Range(0f, 0.95f)]
        [SerializeField]
        private float m_DelayRate = 0;

        [Range(0.001f, 100f)]
        [SerializeField]
        private float m_MaxSpeed = 1;

        [SerializeField]
        private Movement m_Movement;

        [SerializeField]
        private UnityEvent m_OnAttracted;

        public float delay
        {
            get
            {
                return m_DelayRate;
            }
            set
            {
                m_DelayRate = value;
            }
        }

        public float maxSpeed
        {
            get
            {
                return m_MaxSpeed;
            }
            set
            {
                m_MaxSpeed = value;
            }
        }

        public Movement movement
        {
            get
            {
                return m_Movement;
            }
            set
            {
                m_Movement = value;
            }
        }

        private UIParticle _uiParticle;

        private void OnEnable()
        {
            if (m_ParticleSystem == null)
            {
                Debug.LogError("No particle system attached to particle attractor script", this);
                enabled = false;
                return;
            }

            _uiParticle = m_ParticleSystem.GetComponentInParent<UIParticle>();
            if (_uiParticle && !_uiParticle.particles.Contains(m_ParticleSystem))
            {
                _uiParticle = null;
            }
            UIParticleUpdater.Register(this);
        }

        private void OnDisable()
        {
            _uiParticle = null;
            UIParticleUpdater.Unregister(this);
        }

        internal void Attract()
        {
            if (m_ParticleSystem == null) return;

            var count = m_ParticleSystem.particleCount;
            if (count == 0) return;

            var particles = ParticleSystemExtensions.GetParticleArray(count);
            m_ParticleSystem.GetParticles(particles, count);

            var dstPos = GetDestinationPosition();
            for (var i = 0; i < count; i++)
            {
                // Attracted
                var p = particles[i];
                if (0f < p.remainingLifetime && Vector3.Distance(p.position, dstPos) < m_DestinationRadius)
                {
                    p.remainingLifetime = 0f;
                    particles[i] = p;

                    if (m_OnAttracted != null)
                    {
                        try
                        {
                            m_OnAttracted.Invoke();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    continue;
                }

                // Calc attracting time
                var delayTime = p.startLifetime * m_DelayRate;
                var duration = p.startLifetime - delayTime;
                var time = Mathf.Max(0, p.startLifetime - p.remainingLifetime - delayTime);

                // Delay
                if (time <= 0) continue;

                // Attract
                p.position = GetAttractedPosition(p.position, dstPos, duration, time);
                p.velocity *= 0.5f;
                particles[i] = p;
            }

            m_ParticleSystem.SetParticles(particles, count);
        }

        private Vector3 GetDestinationPosition()
        {
            var isUI = _uiParticle && _uiParticle.enabled;
            var psPos = m_ParticleSystem.transform.position;
            var attractorPos = transform.position;
            var dstPos = attractorPos;
            if (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local)
            {
                dstPos = m_ParticleSystem.transform.InverseTransformPoint(dstPos);
                if (isUI)
                {
                    dstPos = dstPos.GetScaled(_uiParticle.transform.localScale, _uiParticle.scale3D.Inverse());
                }
            }
            else
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && isUI)
                {
                    var diff = dstPos - psPos;
                    diff = diff.GetScaled(_uiParticle.transform.localScale, _uiParticle.scale3D.Inverse());
                    return psPos + diff;
                }
#endif
                if (isUI)
                {
                    dstPos.Scale(_uiParticle.transform.localScale);
                    dstPos.Scale(_uiParticle.scale3D.Inverse());
                }
            }
            return dstPos;
        }

        private Vector3 GetAttractedPosition(Vector3 current, Vector3 target, float duration, float time)
        {
            var speed = m_MaxSpeed;
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

    }
}