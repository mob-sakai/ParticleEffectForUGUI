using System.Collections;
using System.Threading;
using Coffee.UIExtensions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ParticleUI = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue362.Tests
{
    public class ApplicationPauseTests
    {
        [UnityTest]
        public IEnumerator AttractorRespectsScaledAndUnscaledTimeWhenPaused()
        {
            var oldTimeScale = Time.timeScale;
            var canvas = new GameObject("Canvas", typeof(Canvas));
            var system = new GameObject("ParticleSystem");
            var target = new GameObject("Attractor");
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var ps = system.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.playOnAwake = false;
                main.startSpeed = 0;
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var particle = new ParticleSystem.Particle
                {
                    position = Vector3.zero,
                    startLifetime = 5,
                    remainingLifetime = 4,
                    startSize = 1
                };
                ps.SetParticles(new[] { particle }, 1);
                target.transform.position = Vector3.right * 1000;
                var attractor = target.AddComponent<UIParticleAttractor>();
                attractor.movement = UIParticleAttractor.Movement.Linear;
                attractor.maxSpeed = 0.1f;
                attractor.updateMode = UIParticleAttractor.UpdateMode.Normal;
                attractor.AddParticleSystem(ps);
                Time.timeScale = 0;
                yield return null;
                yield return new WaitForEndOfFrame();
                var particles = new ParticleSystem.Particle[1];
                Assert.AreEqual(1, ps.GetParticles(particles));
                var pausedPosition = particles[0].position.x;
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.AreEqual(1, ps.GetParticles(particles));
                Assert.That(particles[0].position.x, Is.EqualTo(pausedPosition).Within(0.001f));
                attractor.updateMode = UIParticleAttractor.UpdateMode.UnscaledTime;
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.AreEqual(1, ps.GetParticles(particles));
                Assert.Greater(particles[0].position.x, pausedPosition);
            }
            finally
            {
                Time.timeScale = oldTimeScale;
                Object.Destroy(canvas);
                Object.Destroy(system);
                Object.Destroy(target);
            }
        }

        [UnityTest]
        public IEnumerator UnscaledAttractorCapsMovementAfterLongFrame()
        {
            var oldMaximumDeltaTime = Time.maximumDeltaTime;
            var canvas = new GameObject("Canvas", typeof(Canvas));
            var system = new GameObject("ParticleSystem");
            var target = new GameObject("Attractor");
            try
            {
                Time.maximumDeltaTime = 0.1f;
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                system.SetActive(false);
                var ps = system.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.playOnAwake = false;
                main.startLifetime = 5;
                main.startSpeed = 0;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                var shape = ps.shape;
                shape.enabled = false;
                system.SetActive(true);
                ps.Emit(1);
                target.transform.position = Vector3.right * 1000;
                var attractor = target.AddComponent<UIParticleAttractor>();
                attractor.updateMode = UIParticleAttractor.UpdateMode.UnscaledTime;
                attractor.movement = UIParticleAttractor.Movement.Linear;
                attractor.maxSpeed = 0.1f;
                attractor.AddParticleSystem(ps);
                yield return null;
                yield return new WaitForEndOfFrame();
                var particles = new ParticleSystem.Particle[1];
                Assert.AreEqual(1, ps.GetParticles(particles));
                var previous = particles[0].position.x;
                Thread.Sleep(800);
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.Greater(Time.unscaledDeltaTime, Time.maximumDeltaTime * 2);
                Assert.AreEqual(1, ps.GetParticles(particles));
                var distance = particles[0].position.x - previous;
                var maxDistance = attractor.maxSpeed * 60 * Time.maximumDeltaTime / particles[0].startLifetime;
                Assert.LessOrEqual(distance, maxDistance + 0.01f);
            }
            finally
            {
                Time.maximumDeltaTime = oldMaximumDeltaTime;
                Object.Destroy(canvas);
                Object.Destroy(system);
                Object.Destroy(target);
            }
        }

        [UnityTest]
        public IEnumerator UnscaledUiParticleCapsSimulationAfterLongFrame()
        {
            var oldMaximumDeltaTime = Time.maximumDeltaTime;
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                Time.maximumDeltaTime = 0.1f;
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var root = new GameObject("UI", typeof(RectTransform), typeof(CanvasRenderer));
                root.transform.SetParent(canvas.transform, false);
                var ui = root.AddComponent<ParticleUI>();
                var child = new GameObject("ParticleSystem");
                child.SetActive(false);
                child.transform.SetParent(root.transform, false);
                var ps = child.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.playOnAwake = false;
                main.useUnscaledTime = true;
                main.loop = false;
                main.duration = 1;
                main.startLifetime = 5;
                main.startSpeed = 0;
                var emission = ps.emission;
                emission.enabled = false;
                child.SetActive(true);
                ui.RefreshParticles();
                ui.Emit(1);
                yield return null;
                yield return new WaitForEndOfFrame();
                var particles = new ParticleSystem.Particle[1];
                Assert.AreEqual(1, ps.GetParticles(particles));
                var before = particles[0].remainingLifetime;
                Thread.Sleep(800);
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.Greater(Time.unscaledDeltaTime, Time.maximumDeltaTime * 2);
                Assert.AreEqual(1, ps.GetParticles(particles));
                Assert.LessOrEqual(before - particles[0].remainingLifetime, Time.maximumDeltaTime + 0.01f);
            }
            finally
            {
                Time.maximumDeltaTime = oldMaximumDeltaTime;
                Object.Destroy(canvas);
            }
        }
    }
}
