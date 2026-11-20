using System;
using System.Collections.Generic;
using Coffee.UIParticleInternal;
using UnityEditor;
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
            Sphere,
            Hemisphere,
            InSmooth,
            OutSmooth
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
        [HideInInspector]
        private float m_DelayRate;

        [SerializeField]
        private ParticleSystem.MinMaxCurve m_DelayRateCurve = new ParticleSystem.MinMaxCurve(0);

        [SerializeField]
        [HideInInspector]
        private bool m_IsDelayRateUpgraded;

        [Range(0.001f, 100f)]
        [SerializeField]
        private float m_MaxSpeed = 1;

        [SerializeField]
        private Movement m_Movement;

        [SerializeField]
        private UpdateMode m_UpdateMode;

        [Range(0f, 1f)]
        [SerializeField]
        private float m_SphereCurveRate = 1f;

        [Min(0f)]
        [SerializeField]
        private float m_HemisphereRadius = 100f;

        [SerializeField]
        private UnityEvent m_OnAttracted;

        private List<UIParticle> _uiParticles = new List<UIParticle>();
        private readonly Dictionary<long, AttractionParticle> _attractionParticles =
            new Dictionary<long, AttractionParticle>();
        private readonly List<long> _staleAttractionParticleKeys = new List<long>();

        public float destinationRadius
        {
            get => m_DestinationRadius;
            set => m_DestinationRadius = Mathf.Clamp(value, 0.1f, 10f);
        }

        public float delay
        {
            get => Mathf.Clamp(EvaluateDelayRate(0.5f), 0f, 0.95f);
            set
            {
                m_DelayRate = Mathf.Clamp(value, 0f, 0.95f);
                m_DelayRateCurve = new ParticleSystem.MinMaxCurve(m_DelayRate);
                m_IsDelayRateUpgraded = true;
            }
        }

        public ParticleSystem.MinMaxCurve delayRate
        {
            get => m_DelayRateCurve;
            set
            {
                m_DelayRateCurve = value;
                m_IsDelayRateUpgraded = true;
            }
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

        public float sphereCurveRate
        {
            get => m_SphereCurveRate;
            set => m_SphereCurveRate = Mathf.Clamp(value, 0f, 1f);
        }

        public float hemisphereRadius
        {
            get => m_HemisphereRadius;
            set => m_HemisphereRadius = Mathf.Max(0f, value);
        }

        public UnityEvent onAttracted
        {
            get => m_OnAttracted;
            set => m_OnAttracted = value;
        }

        /// <summary>
        /// The target ParticleSystems to attract. Use <see cref="AddParticleSystem"/> and
        /// <see cref="RemoveParticleSystem"/> or <see cref="ClearParticleSystems"/> to modify the list.
        /// </summary>
        public IReadOnlyList<ParticleSystem> particleSystems
        {
            get
            {
                if (m_ParticleSystems == null)
                {
                    m_ParticleSystems = new List<ParticleSystem>();
                }

                return m_ParticleSystems;
            }
        }

        public void AddParticleSystem(ParticleSystem ps)
        {
            if (ps == null) return;

            if (m_ParticleSystems == null)
            {
                m_ParticleSystems = new List<ParticleSystem>();
            }

            var i = m_ParticleSystems.IndexOf(ps);
            if (0 <= i) return; // Already added: skip

            m_ParticleSystems.Add(ps);
            _uiParticles.Clear();
            _attractionParticles.Clear();
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
            _attractionParticles.Clear();
        }

        public void ClearParticleSystems()
        {
            m_ParticleSystems?.Clear();
            _uiParticles?.Clear();
            _attractionParticles.Clear();
            _staleAttractionParticleKeys.Clear();
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
            _attractionParticles.Clear();
        }

        private void OnDestroy()
        {
            _uiParticles = null;
            m_ParticleSystems = null;
        }

        internal void Attract()
        {
            if (m_ParticleSystems == null || m_ParticleSystems.Count == 0) return;

            // Collect UIParticle if needed (same size as m_ParticleSystems)
            CollectUIParticlesIfNeeded();

            for (var particleIndex = 0; particleIndex < m_ParticleSystems.Count; particleIndex++)
            {
                var particleSystem = m_ParticleSystems[particleIndex];

                // Skip: The ParticleSystem is not active
                if (particleSystem == null || !particleSystem.gameObject.activeInHierarchy) continue;

                // Skip: No active particles
                var count = particleSystem.particleCount;
                if (count == 0) continue;

                var particles = ParticleSystemExtensions.GetParticleArray(count);
                particleSystem.GetParticles(particles, count);

                var uiParticle = particleIndex < _uiParticles.Count ? _uiParticles[particleIndex] : null;
                var dstPos = GetDestinationPosition(uiParticle, particleSystem);
                var hemisphereDirection = transform.up;
                if (particleSystem.IsLocalSpace())
                {
                    hemisphereDirection = particleSystem.transform.InverseTransformDirection(hemisphereDirection);
                }

                for (var i = 0; i < count; i++)
                {
                    // Attracted
                    var p = particles[i];
                    var isHemisphere = m_Movement == Movement.Hemisphere;
                    var hasStableOrigin = isHemisphere
                                          || m_Movement == Movement.InSmooth
                                          || m_Movement == Movement.OutSmooth;
                    var particleKey = hasStableOrigin ? GetAttractionParticleKey(particleSystem, p) : 0;

                    if (0f < p.remainingLifetime && Vector3.Distance(p.position, dstPos) < m_DestinationRadius)
                    {
                        p.remainingLifetime = 0f;
                        particles[i] = p;
                        if (hasStableOrigin) _attractionParticles.Remove(particleKey);

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
                    var delayRate = EvaluateDelayRate((p.randomSeed & 0x00ffffff) / 16777215f);
                    var delayTime = p.startLifetime * Mathf.Clamp(delayRate, 0f, 0.95f);
                    var duration = p.startLifetime - delayTime;
                    var time = Mathf.Max(0, p.startLifetime - p.remainingLifetime - delayTime);

                    // Delay
                    if (time <= 0) continue;

                    // Attract
                    var movementOrigin = p.position;
                    var attractionTime = time;
                    if (hasStableOrigin)
                    {
                        if (!_attractionParticles.TryGetValue(particleKey, out var state))
                        {
                            state = new AttractionParticle
                            {
                                origin = p.position,
                                lastSeenFrame = Time.frameCount
                            };
                        }
                        else
                        {
                            state.lastSeenFrame = Time.frameCount;
                        }

                        var progress = duration > 0 ? Mathf.Clamp01(time / duration) : 1f;
                        if (isHemisphere)
                        {
                            state.progress = Mathf.Lerp(state.progress, 1f, progress);
                            attractionTime = state.progress * duration;
                        }

                        _attractionParticles[particleKey] = state;
                        movementOrigin = state.origin;
                    }

                    p.position = GetAttractedPosition(
                        p.position, dstPos, duration, attractionTime, hemisphereDirection, movementOrigin);
                    p.velocity *= 0.5f;
                    particles[i] = p;
                }

                particleSystem.SetParticles(particles, count);
            }

            RemoveStaleAttractionParticles();
        }

        private Vector3 GetDestinationPosition(UIParticle uiParticle, ParticleSystem particleSystem)
        {
            var isUI = uiParticle != null && uiParticle.enabled;
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

        private Vector3 GetAttractedPosition(
            Vector3 current, Vector3 target, float duration, float time, Vector3 hemisphereDirection,
            Vector3 movementOrigin)
        {
            var speed = m_MaxSpeed;
            switch (m_UpdateMode)
            {
                case UpdateMode.Normal:
                    speed *= 60 * Time.deltaTime;
                    break;
                case UpdateMode.UnscaledTime:
                    speed *= 60 * Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime);
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
                case Movement.InSmooth:
                    var inProgress = duration > 0 ? Mathf.Clamp01(time / duration) : 1f;
                    target = Vector3.Lerp(movementOrigin, target, inProgress * inProgress);
                    break;
                case Movement.OutSmooth:
                    var outProgress = duration > 0 ? Mathf.Clamp01(time / duration) : 1f;
                    var remainingProgress = 1f - outProgress;
                    target = Vector3.Lerp(movementOrigin, target, 1f - remainingProgress * remainingProgress);
                    break;
                case Movement.Sphere:
                    var t = time / duration;
                    var linear = Vector3.Lerp(current, target, t);
                    var spherical = Vector3.Slerp(current, target, t);
                    target = Vector3.Lerp(linear, spherical, m_SphereCurveRate);
                    break;
                case Movement.Hemisphere:
                    var progress = duration > 0 ? Mathf.Clamp01(time / duration) : 1f;
                    var direction = target - movementOrigin;
                    var perpendicular = Vector3.ProjectOnPlane(hemisphereDirection, direction);
                    if (perpendicular.sqrMagnitude < 0.000001f)
                    {
                        perpendicular = Vector3.ProjectOnPlane(Vector3.forward, direction);
                    }

                    if (perpendicular.sqrMagnitude < 0.000001f)
                    {
                        perpendicular = Vector3.ProjectOnPlane(Vector3.right, direction);
                    }

                    if (perpendicular.sqrMagnitude > 0)
                    {
                        perpendicular.Normalize();
                    }

                    var linearTarget = Vector3.Lerp(movementOrigin, target, progress);
                    var curvedTarget = linearTarget
                                       + perpendicular * (m_HemisphereRadius * Mathf.Sin(Mathf.PI * progress));
                    var directLength = direction.magnitude;
                    var targetOffset = curvedTarget - current;
                    if (directLength > 0f && targetOffset.sqrMagnitude > 0f)
                    {
                        var alignment = Vector3.Dot(
                            targetOffset.normalized, direction / directLength);
                        if (alignment > 0.000001f)
                        {
                            speed /= alignment;
                        }
                    }

                    target = curvedTarget;
                    break;
            }

            return Vector3.MoveTowards(current, target, speed);
        }

        private static long GetAttractionParticleKey(ParticleSystem particleSystem, ParticleSystem.Particle particle)
        {
            return ((long)particleSystem.GetInstanceID() << 32) | particle.randomSeed;
        }

        private float EvaluateDelayRate(float randomValue)
        {
            return m_DelayRateCurve.Evaluate(randomValue, randomValue);
        }

        private void RemoveStaleAttractionParticles()
        {
            _staleAttractionParticleKeys.Clear();
            foreach (var pair in _attractionParticles)
            {
                if (pair.Value.lastSeenFrame != Time.frameCount)
                {
                    _staleAttractionParticleKeys.Add(pair.Key);
                }
            }

            for (var i = 0; i < _staleAttractionParticleKeys.Count; i++)
            {
                _attractionParticles.Remove(_staleAttractionParticleKeys[i]);
            }
        }

        private struct AttractionParticle
        {
            public Vector3 origin;
            public float progress;
            public int lastSeenFrame;
        }

        private void CollectUIParticlesIfNeeded()
        {
            if (m_ParticleSystems == null) return;
            if (_uiParticles == null)
            {
                _uiParticles = new List<UIParticle>();
            }

            if (m_ParticleSystems.Count == 0 || _uiParticles.Count == m_ParticleSystems.Count) return;

            _uiParticles.Clear();

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
                _uiParticles.Add(uiParticle != null && uiParticle.particles != null && uiParticle.particles.Contains(ps)
                    ? uiParticle
                    : null);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _uiParticles?.Clear();
        }
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UpgradeIfNeeded();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UpgradeDelayRateIfNeeded();
        }

        private void UpgradeIfNeeded()
        {
            if (m_ParticleSystems == null)
            {
                m_ParticleSystems = new List<ParticleSystem>();
            }

            UpgradeDelayRateIfNeeded();

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

        private void UpgradeDelayRateIfNeeded()
        {
            if (m_IsDelayRateUpgraded) return;

            m_DelayRateCurve = new ParticleSystem.MinMaxCurve(Mathf.Clamp(m_DelayRate, 0f, 0.95f));
            m_IsDelayRateUpgraded = true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIParticleAttractor))]
    [CanEditMultipleObjects]
    internal class UIParticleAttractorEditor : Editor
    {
        private SerializedProperty m_ParticleSystems;
        private SerializedProperty m_DestinationRadius;
        private SerializedProperty m_DelayRateCurve;
        private SerializedProperty m_MaxSpeed;
        private SerializedProperty m_Movement;
        private SerializedProperty m_UpdateMode;
        private SerializedProperty m_SphereCurveRate;
        private SerializedProperty m_HemisphereRadius;
        private SerializedProperty m_OnAttracted;

        private void OnEnable()
        {
            m_ParticleSystems = serializedObject.FindProperty("m_ParticleSystems");
            m_DestinationRadius = serializedObject.FindProperty("m_DestinationRadius");
            m_DelayRateCurve = serializedObject.FindProperty("m_DelayRateCurve");
            m_MaxSpeed = serializedObject.FindProperty("m_MaxSpeed");
            m_Movement = serializedObject.FindProperty("m_Movement");
            m_UpdateMode = serializedObject.FindProperty("m_UpdateMode");
            m_SphereCurveRate = serializedObject.FindProperty("m_SphereCurveRate");
            m_HemisphereRadius = serializedObject.FindProperty("m_HemisphereRadius");
            m_OnAttracted = serializedObject.FindProperty("m_OnAttracted");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_ParticleSystems);
            EditorGUILayout.PropertyField(m_DestinationRadius);
            EditorGUILayout.PropertyField(m_DelayRateCurve);
            EditorGUILayout.PropertyField(m_MaxSpeed);
            EditorGUILayout.PropertyField(m_Movement);

            if (m_Movement.enumValueIndex == (int)UIParticleAttractor.Movement.Sphere)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_SphereCurveRate);
                EditorGUI.indentLevel--;
            }
            else if (m_Movement.enumValueIndex == (int)UIParticleAttractor.Movement.Hemisphere)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_HemisphereRadius);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(m_UpdateMode);
            EditorGUILayout.PropertyField(m_OnAttracted);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
