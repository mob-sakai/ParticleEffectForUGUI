using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Coffee.UIParticle.Issue321.Tests
{
    public class AttractorDelayCurveTests
    {
        private static readonly MethodInfo s_Attract = typeof(Coffee.UIExtensions.UIParticleAttractor)
            .GetMethod("Attract", BindingFlags.Instance | BindingFlags.NonPublic);

        [Test]
        public void ConstantDelayRemainsCompatibleWithExistingDelayProperty()
        {
            var attractorObject = new GameObject("Attractor");
            try
            {
                var attractor = attractorObject.AddComponent<Coffee.UIExtensions.UIParticleAttractor>();
                attractor.delay = 0.4f;
                Assert.AreEqual(ParticleSystemCurveMode.Constant, attractor.delayRate.mode);
                Assert.That(attractor.delayRate.constant, Is.EqualTo(0.4f).Within(0.0001f));
                Assert.That(attractor.delay, Is.EqualTo(0.4f).Within(0.0001f));
                attractor.delay = -1;
                Assert.AreEqual(0, attractor.delayRate.constant);
                attractor.delay = 1;
                Assert.AreEqual(0.95f, attractor.delayRate.constant);
            }
            finally
            {
                Object.DestroyImmediate(attractorObject);
            }
        }

        [Test]
        public void TwoConstantsUseStablePerParticleRandomValues()
        {
            var particleSystemObject = new GameObject("ParticleSystem");
            var attractorObject = new GameObject("Attractor");
            try
            {
                var particleSystem = CreateParticleSystem(particleSystemObject);
                var attractor = CreateAttractor(attractorObject, particleSystem);
                attractor.delayRate = new ParticleSystem.MinMaxCurve(0.2f, 0.8f);
                particleSystem.SetParticles(new[]
                {
                    CreateParticle(0x00000000, 0.5f),
                    CreateParticle(0x00ffffff, 0.5f)
                }, 2);

                InvokeAttract(attractor);
                var particles = new ParticleSystem.Particle[2];
                Assert.AreEqual(2, particleSystem.GetParticles(particles));
                Assert.That(particles[0].position.x, Is.GreaterThan(0));
                Assert.That(particles[1].position.x, Is.EqualTo(0).Within(0.0001f));
            }
            finally
            {
                Object.DestroyImmediate(particleSystemObject);
                Object.DestroyImmediate(attractorObject);
            }
        }

        [Test]
        public void CurvesAndTwoCurvesAreEvaluatedPerParticle()
        {
            var particleSystemObject = new GameObject("ParticleSystem");
            var attractorObject = new GameObject("Attractor");
            try
            {
                var particleSystem = CreateParticleSystem(particleSystemObject);
                var attractor = CreateAttractor(attractorObject, particleSystem);
                var earlyCurve = AnimationCurve.Linear(0, 0.2f, 1, 0.2f);
                var lateCurve = AnimationCurve.Linear(0, 0.8f, 1, 0.8f);
                particleSystem.SetParticles(new[]
                {
                    CreateParticle(0x00000000, 0.5f),
                    CreateParticle(0x00ffffff, 0.5f)
                }, 2);

                attractor.delayRate = new ParticleSystem.MinMaxCurve(1, earlyCurve);
                InvokeAttract(attractor);
                var particles = new ParticleSystem.Particle[2];
                particleSystem.GetParticles(particles);
                Assert.That(particles[0].position.x, Is.GreaterThan(0));
                Assert.That(particles[1].position.x, Is.GreaterThan(0));

                attractor.delayRate = new ParticleSystem.MinMaxCurve(1, earlyCurve, lateCurve);
                particleSystem.SetParticles(new[]
                {
                    CreateParticle(0x00000000, 0.5f),
                    CreateParticle(0x00ffffff, 0.5f)
                }, 2);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);
                Assert.That(particles[0].position.x, Is.GreaterThan(0));
                Assert.That(particles[1].position.x, Is.EqualTo(0).Within(0.0001f));
            }
            finally
            {
                Object.DestroyImmediate(particleSystemObject);
                Object.DestroyImmediate(attractorObject);
            }
        }

        [Test]
        public void LegacyFloatDelayIsUpgradedToAConstantCurve()
        {
            var attractorObject = new GameObject("Attractor");
            try
            {
                var attractor = attractorObject.AddComponent<Coffee.UIExtensions.UIParticleAttractor>();
                var type = attractor.GetType();
                type.GetField("m_DelayRate", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(attractor, 0.35f);
                type.GetField("m_IsDelayRateUpgraded", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(attractor, false);
                type.GetMethod("UpgradeIfNeeded", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(attractor, null);
                Assert.AreEqual(ParticleSystemCurveMode.Constant, attractor.delayRate.mode);
                Assert.That(attractor.delayRate.constant, Is.EqualTo(0.35f).Within(0.0001f));
                Assert.IsTrue((bool)type.GetField("m_IsDelayRateUpgraded", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(attractor));
            }
            finally
            {
                Object.DestroyImmediate(attractorObject);
            }
        }

        private static ParticleSystem CreateParticleSystem(GameObject go)
        {
            var particleSystem = go.AddComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startLifetime = 1;
            main.startSpeed = 0;
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return particleSystem;
        }

        private static Coffee.UIExtensions.UIParticleAttractor CreateAttractor(
            GameObject go, ParticleSystem particleSystem)
        {
            go.transform.position = Vector3.right * 100;
            var attractor = go.AddComponent<Coffee.UIExtensions.UIParticleAttractor>();
            attractor.maxSpeed = 10000;
            attractor.destinationRadius = 0.1f;
            attractor.AddParticleSystem(particleSystem);
            return attractor;
        }

        private static ParticleSystem.Particle CreateParticle(uint randomSeed, float remainingLifetime)
        {
            return new ParticleSystem.Particle
            {
                randomSeed = randomSeed,
                startLifetime = 1,
                remainingLifetime = remainingLifetime,
                startSize = 1
            };
        }

        private static void InvokeAttract(Coffee.UIExtensions.UIParticleAttractor attractor)
        {
            Assert.IsNotNull(s_Attract);
            s_Attract.Invoke(attractor, null);
        }
    }
}
