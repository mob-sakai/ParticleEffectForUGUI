using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UIParticleAttractor = Coffee.UIExtensions.UIParticleAttractor;

namespace Coffee.UIParticle.Issue378.Tests
{
    public class AttractorSmoothTests
    {
        private static readonly MethodInfo s_Attract = typeof(UIParticleAttractor)
            .GetMethod("Attract", BindingFlags.Instance | BindingFlags.NonPublic);

        [Test]
        public void InSmoothAndOutSmoothEaseOnlyTheirNamedSide()
        {
            var particleSystemObject = new GameObject("ParticleSystem");
            var attractorObject = new GameObject("Attractor");
            try
            {
                var particleSystem = CreateParticleSystem(particleSystemObject);
                var attractor = CreateAttractor(attractorObject, particleSystem, Vector3.right * 10);
                var particles = new ParticleSystem.Particle[1];

                attractor.movement = UIParticleAttractor.Movement.InSmooth;
                var particle = CreateParticle(Vector3.zero, 0.5f, 1234);
                particleSystem.SetParticles(new[] { particle }, 1);
                InvokeAttract(attractor);
                Assert.AreEqual(1, particleSystem.GetParticles(particles));
                Assert.That(particles[0].position.x, Is.EqualTo(2.5f).Within(0.001f));

                particle = particles[0];
                particle.remainingLifetime = 0.25f;
                particleSystem.SetParticles(new[] { particle }, 1);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);
                Assert.That(particles[0].position.x, Is.EqualTo(5.625f).Within(0.001f));

                attractor.movement = UIParticleAttractor.Movement.OutSmooth;
                particle = CreateParticle(Vector3.zero, 0.5f, 5678);
                particleSystem.SetParticles(new[] { particle }, 1);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);
                Assert.That(particles[0].position.x, Is.EqualTo(7.5f).Within(0.001f));
            }
            finally
            {
                Object.DestroyImmediate(particleSystemObject);
                Object.DestroyImmediate(attractorObject);
            }
        }

        [Test]
        public void ClearParticleSystemsClearsListAndAllowsFreshAttraction()
        {
            var particleSystemObject = new GameObject("ParticleSystem");
            var attractorObject = new GameObject("Attractor");
            try
            {
                var particleSystem = CreateParticleSystem(particleSystemObject);
                var attractor = CreateAttractor(attractorObject, particleSystem, Vector3.right * 10);
                attractor.movement = UIParticleAttractor.Movement.InSmooth;
                var particle = CreateParticle(Vector3.zero, 0.5f, 1234);
                particleSystem.SetParticles(new[] { particle }, 1);
                InvokeAttract(attractor);

                var particles = new ParticleSystem.Particle[1];
                Assert.AreEqual(1, particleSystem.GetParticles(particles));
                Assert.That(particles[0].position.x, Is.EqualTo(2.5f).Within(0.001f));
                Assert.AreEqual(1, attractor.particleSystems.Count);

                attractor.ClearParticleSystems();

                Assert.AreEqual(0, attractor.particleSystems.Count);
                particle = CreateParticle(Vector3.right * 20, 0.5f, 1234);
                particleSystem.SetParticles(new[] { particle }, 1);
                attractor.transform.position = Vector3.right * 30;
                attractor.AddParticleSystem(particleSystem);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);

                Assert.That(particles[0].position.x, Is.EqualTo(22.5f).Within(0.001f));
            }
            finally
            {
                Object.DestroyImmediate(particleSystemObject);
                Object.DestroyImmediate(attractorObject);
            }
        }

        [Test]
        public void NewMovementValuesPreserveExistingSerializedValues()
        {
            Assert.AreEqual(0, (int)UIParticleAttractor.Movement.Linear);
            Assert.AreEqual(1, (int)UIParticleAttractor.Movement.Smooth);
            Assert.AreEqual(2, (int)UIParticleAttractor.Movement.Sphere);
            Assert.AreEqual(3, (int)UIParticleAttractor.Movement.Hemisphere);
            Assert.AreEqual(4, (int)UIParticleAttractor.Movement.InSmooth);
            Assert.AreEqual(5, (int)UIParticleAttractor.Movement.OutSmooth);
        }

        private static ParticleSystem CreateParticleSystem(GameObject gameObject)
        {
            var particleSystem = gameObject.AddComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startSpeed = 0;
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return particleSystem;
        }

        private static UIParticleAttractor CreateAttractor(
            GameObject gameObject, ParticleSystem particleSystem, Vector3 destination)
        {
            gameObject.transform.position = destination;
            var attractor = gameObject.AddComponent<UIParticleAttractor>();
            attractor.maxSpeed = 10000;
            attractor.AddParticleSystem(particleSystem);
            return attractor;
        }

        private static ParticleSystem.Particle CreateParticle(Vector3 position, float remainingLifetime, uint seed)
        {
            return new ParticleSystem.Particle
            {
                position = position,
                startLifetime = 1,
                remainingLifetime = remainingLifetime,
                startSize = 1,
                randomSeed = seed
            };
        }

        private static void InvokeAttract(UIParticleAttractor attractor)
        {
            Assert.IsNotNull(s_Attract);
            s_Attract.Invoke(attractor, null);
        }
    }
}
