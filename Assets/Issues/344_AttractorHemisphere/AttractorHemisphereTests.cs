using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Coffee.UIParticle.Issue344.Tests
{
    public class AttractorHemisphereTests
    {
        private static readonly MethodInfo s_Attract = typeof(Coffee.UIExtensions.UIParticleAttractor)
            .GetMethod("Attract", BindingFlags.Instance | BindingFlags.NonPublic);

        [Test]
        public void HemisphereCreatesAnOffsetAtTheMidpointAndReachesBothEndpoints()
        {
            var particleSystemObject = new GameObject("ParticleSystem");
            var attractorObject = new GameObject("Attractor");
            try
            {
                var particleSystem = particleSystemObject.AddComponent<ParticleSystem>();
                var main = particleSystem.main;
                main.playOnAwake = false;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                main.startSpeed = 0;
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var particle = CreateParticle(Vector3.zero, 0.5f);
                particleSystem.SetParticles(new[] { particle }, 1);
                attractorObject.transform.position = Vector3.right * 10;

                var attractor = attractorObject.AddComponent<Coffee.UIExtensions.UIParticleAttractor>();
                attractor.movement = Coffee.UIExtensions.UIParticleAttractor.Movement.Hemisphere;
                attractor.hemisphereRadius = 2;
                attractor.maxSpeed = 10000;
                attractor.AddParticleSystem(particleSystem);

                InvokeAttract(attractor);
                var particles = new ParticleSystem.Particle[1];
                Assert.AreEqual(1, particleSystem.GetParticles(particles));
                Assert.That(particles[0].position.x, Is.EqualTo(5).Within(0.001f));
                Assert.That(particles[0].position.y, Is.EqualTo(2).Within(0.001f));

                particle.position = particles[0].position;
                particle.remainingLifetime = 0.25f;
                particleSystem.SetParticles(new[] { particle }, 1);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);
                Assert.That(particles[0].position.x, Is.EqualTo(8.75f).Within(0.001f),
                    $"Position after second update: {particles[0].position}; input: {particle.position}");
                Assert.That(particles[0].position.y,
                    Is.EqualTo(2 * Mathf.Sin(Mathf.PI * 0.875f)).Within(0.001f));

                attractor.hemisphereRadius = 0;
                particle = CreateParticle(Vector3.zero, 0.5f);
                particle.randomSeed = 2345;
                particleSystem.SetParticles(new[] { particle }, 1);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);
                Assert.That(particles[0].position, Is.EqualTo(Vector3.right * 5));

                attractor.hemisphereRadius = 2;
                particle = CreateParticle(Vector3.zero, 1f);
                particle.randomSeed = 3456;
                particleSystem.SetParticles(new[] { particle }, 1);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);
                Assert.That(particles[0].position, Is.EqualTo(Vector3.zero));

                particle = CreateParticle(Vector3.zero, 0.001f);
                particle.randomSeed = 4567;
                particleSystem.SetParticles(new[] { particle }, 1);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);
                Assert.Less(Vector3.Distance(particles[0].position, Vector3.right * 10), 0.02f);
            }
            finally
            {
                Object.DestroyImmediate(particleSystemObject);
                Object.DestroyImmediate(attractorObject);
            }
        }

        [Test]
        public void HemisphereRadiusCanBeZeroAndCannotBeNegative()
        {
            Assert.AreEqual(0, (int)Coffee.UIExtensions.UIParticleAttractor.Movement.Linear);
            Assert.AreEqual(1, (int)Coffee.UIExtensions.UIParticleAttractor.Movement.Smooth);
            Assert.AreEqual(2, (int)Coffee.UIExtensions.UIParticleAttractor.Movement.Sphere);
            Assert.AreEqual(3, (int)Coffee.UIExtensions.UIParticleAttractor.Movement.Hemisphere);

            var go = new GameObject("Attractor");
            try
            {
                var attractor = go.AddComponent<Coffee.UIExtensions.UIParticleAttractor>();
                attractor.hemisphereRadius = 3;
                Assert.AreEqual(3, attractor.hemisphereRadius);
                attractor.hemisphereRadius = 0;
                Assert.AreEqual(0, attractor.hemisphereRadius);
                attractor.hemisphereRadius = -1;
                Assert.AreEqual(0, attractor.hemisphereRadius);
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void HemisphereFollowsAttractorUpAxisAndHandlesParallelPath()
        {
            var particleSystemObject = new GameObject("ParticleSystem");
            var attractorObject = new GameObject("Attractor");
            try
            {
                var particleSystem = particleSystemObject.AddComponent<ParticleSystem>();
                var main = particleSystem.main;
                main.playOnAwake = false;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                main.startSpeed = 0;
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var attractor = attractorObject.AddComponent<Coffee.UIExtensions.UIParticleAttractor>();
                attractor.movement = Coffee.UIExtensions.UIParticleAttractor.Movement.Hemisphere;
                attractor.hemisphereRadius = 2;
                attractor.maxSpeed = 10000;
                attractor.transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                attractor.transform.position = Vector3.right * 10;
                attractor.AddParticleSystem(particleSystem);
                var particles = new ParticleSystem.Particle[1];

                particleSystem.SetParticles(new[] { CreateParticle(Vector3.zero, 0.5f) }, 1);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);
                Assert.That(particles[0].position.z, Is.EqualTo(2).Within(0.001f));
                Assert.That(particles[0].position.y, Is.EqualTo(0).Within(0.001f));

                attractor.transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.right);
                particleSystem.SetParticles(new[] { CreateParticle(Vector3.zero, 0.5f) }, 1);
                InvokeAttract(attractor);
                particleSystem.GetParticles(particles);
                Assert.IsFalse(float.IsNaN(particles[0].position.x));
                Assert.IsFalse(float.IsNaN(particles[0].position.y));
                Assert.IsFalse(float.IsNaN(particles[0].position.z));
            }
            finally
            {
                Object.DestroyImmediate(particleSystemObject);
                Object.DestroyImmediate(attractorObject);
            }
        }

        [Test]
        public void HemisphereUsesParticleSystemLocalDirectionForLocalSimulation()
        {
            var particleSystemObject = new GameObject("ParticleSystem");
            var attractorObject = new GameObject("Attractor");
            try
            {
                particleSystemObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                var particleSystem = particleSystemObject.AddComponent<ParticleSystem>();
                var main = particleSystem.main;
                main.playOnAwake = false;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                main.startSpeed = 0;
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                var attractor = attractorObject.AddComponent<Coffee.UIExtensions.UIParticleAttractor>();
                attractor.movement = Coffee.UIExtensions.UIParticleAttractor.Movement.Hemisphere;
                attractor.hemisphereRadius = 2;
                attractor.maxSpeed = 10000;
                attractorObject.transform.position = Vector3.right * 10;
                attractor.AddParticleSystem(particleSystem);
                particleSystem.SetParticles(new[] { CreateParticle(Vector3.zero, 0.5f) }, 1);

                InvokeAttract(attractor);
                var particles = new ParticleSystem.Particle[1];
                Assert.AreEqual(1, particleSystem.GetParticles(particles));
                Assert.That(particles[0].position.x, Is.EqualTo(2).Within(0.001f));
                Assert.That(particles[0].position.y, Is.EqualTo(-5).Within(0.001f));
            }
            finally
            {
                Object.DestroyImmediate(particleSystemObject);
                Object.DestroyImmediate(attractorObject);
            }
        }

        [Test]
        public void HemisphereCompensatesForLongerCurveDistance()
        {
            var go = new GameObject("Attractor");
            try
            {
                var attractor = go.AddComponent<Coffee.UIExtensions.UIParticleAttractor>();
                var method = typeof(Coffee.UIExtensions.UIParticleAttractor).GetMethod(
                    "GetAttractedPosition", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(method);
                var destination = Vector3.right * 10;
                var origin = Vector3.zero;
                attractor.movement = Coffee.UIExtensions.UIParticleAttractor.Movement.Hemisphere;
                attractor.maxSpeed = 0.01f;

                attractor.hemisphereRadius = 0;
                var previousStraight = Vector3.right;
                var straight = (Vector3)method.Invoke(
                    attractor, new object[] { previousStraight, destination, 1f, 0.2f, Vector3.up, origin });

                attractor.hemisphereRadius = 5;
                var previousCurved = Vector3.right + Vector3.up * (5 * Mathf.Sin(Mathf.PI * 0.1f));
                var curved = (Vector3)method.Invoke(
                    attractor, new object[] { previousCurved, destination, 1f, 0.2f, Vector3.up, origin });

                Assert.That(curved.x - previousCurved.x, Is.EqualTo(straight.x - previousStraight.x)
                    .Within(0.05f * Mathf.Max(0.001f, straight.x - previousStraight.x)));
                Assert.Greater(curved.y, previousCurved.y);
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        private static ParticleSystem.Particle CreateParticle(Vector3 position, float remainingLifetime)
        {
            return new ParticleSystem.Particle
            {
                position = position,
                startLifetime = 1,
                remainingLifetime = remainingLifetime,
                startSize = 1,
                randomSeed = 1234
            };
        }

        private static void InvokeAttract(Coffee.UIExtensions.UIParticleAttractor attractor)
        {
            Assert.IsNotNull(s_Attract);
            s_Attract.Invoke(attractor, null);
        }
    }
}
