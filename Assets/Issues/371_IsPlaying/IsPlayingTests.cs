using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ParticleUI = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue371.Tests
{
    public class IsPlayingTests
    {
        private static ParticleSystem CreateSystem(Transform parent, bool loop = true)
        {
            var go = new GameObject("ParticleSystem");
            go.SetActive(false);
            go.transform.SetParent(parent, false);
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = false;
            main.loop = loop;
            main.duration = 0.3f;
            main.startLifetime = 0.3f;
            main.startSpeed = 0;
            var emission = ps.emission;
            emission.rateOverTime = 30;
            go.SetActive(true);
            return ps;
        }

        private static ParticleUI CreateUI(GameObject canvas)
        {
            var root = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
            root.transform.SetParent(canvas.transform, false);
            return root.AddComponent<ParticleUI>();
        }

        [UnityTest]
        public IEnumerator ReportsPlaybackPauseResumeAndStop()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var ui = CreateUI(canvas);
                var ps = CreateSystem(ui.transform);
                ui.RefreshParticles();
                Assert.IsFalse(ui.isPlaying);
                ui.Play();
                yield return new WaitForSeconds(0.2f);
                Assert.Greater(ps.particleCount, 0);
                Assert.IsFalse(ps.isPlaying);
                Assert.IsTrue(ui.isPlaying);

                ui.Pause();
                Assert.IsFalse(ui.isPlaying);
                ui.Resume();
                Assert.IsTrue(ui.isPlaying);
                ui.Stop();
                Assert.IsFalse(ui.isPlaying);
                ui.Play();
                yield return null;
                Assert.IsTrue(ui.isPlaying);
                ui.Clear();
                Assert.IsFalse(ui.isPlaying);
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        [UnityTest]
        public IEnumerator NonLoopingSystemRemainsPlayingUntilParticlesDie()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var ui = CreateUI(canvas);
                var ps = CreateSystem(ui.transform, false);
                ui.RefreshParticles();
                ui.Play();
                yield return new WaitForSeconds(0.2f);
                Assert.Greater(ps.particleCount, 0);
                ui.StopEmission();
                Assert.IsTrue(ui.isPlaying);

                var deadline = Time.realtimeSinceStartup + 5;
                while (ui.isPlaying && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.IsFalse(ui.isPlaying);
                Assert.AreEqual(0, ps.particleCount);
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        [UnityTest]
        public IEnumerator ReportsPlayingDuringStartDelayBeforeFirstParticle()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var ui = CreateUI(canvas);
                var ps = CreateSystem(ui.transform, false);
                var main = ps.main;
                main.startDelay = 0.5f;
                ui.RefreshParticles();
                ui.Play();
                yield return new WaitForSeconds(0.1f);
                Assert.AreEqual(0, ps.particleCount);
                Assert.IsTrue(ui.isPlaying);
                ui.Pause();
                Assert.IsFalse(ui.isPlaying);
                ui.Resume();
                Assert.IsTrue(ui.isPlaying);
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        [UnityTest]
        public IEnumerator ReportsManuallyEmittedParticles()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var ui = CreateUI(canvas);
                var ps = CreateSystem(ui.transform, false);
                var emission = ps.emission;
                emission.enabled = false;
                ui.RefreshParticles();
                ui.Emit(1);
                Assert.AreEqual(1, ps.particleCount);
                Assert.IsTrue(ui.isPlaying);
                ui.Pause();
                Assert.IsFalse(ui.isPlaying);
                ui.Resume();
                Assert.IsTrue(ui.isPlaying);
            }
            finally
            {
                Object.Destroy(canvas);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator IgnoresUnregisteredChildParticleSystems()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var ui = CreateUI(canvas);
                var parent = CreateSystem(ui.transform, false);
                var child = CreateSystem(parent.transform);
                ui.RefreshParticles();
                ui.particles.Remove(child);
                child.Play(false);
                parent.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                Assert.IsTrue(child.IsAlive(false));
                Assert.IsFalse(parent.IsAlive(false));
                Assert.IsFalse(parent.isPlaying);
                Assert.IsFalse(ui.isPlaying);
            }
            finally
            {
                Object.Destroy(canvas);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator AggregatesRegisteredSystemsAndIgnoresInactiveOrMissingEntries()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var ui = CreateUI(canvas);
                var first = CreateSystem(ui.transform);
                var second = CreateSystem(ui.transform);
                ui.RefreshParticles();
                ui.particles.Add(null);
                ui.Play();
                yield return new WaitForSeconds(0.1f);
                first.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                Assert.IsTrue(ui.isPlaying);
                second.gameObject.SetActive(false);
                Assert.IsFalse(ui.isPlaying);
                second.gameObject.SetActive(true);
                ui.Play();
                yield return null;
                Assert.IsTrue(ui.isPlaying);
                ui.enabled = false;
                Assert.IsFalse(ui.isPlaying);
                ui.enabled = true;
                ui.Stop();
                Assert.IsFalse(ui.isPlaying);
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        [UnityTest]
        public IEnumerator ReplicaReportsItsOwnSystemsRatherThanPrimary()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var primary = CreateUI(canvas);
                var replica = CreateUI(canvas);
                primary.meshSharing = ParticleUI.MeshSharing.Primary;
                replica.meshSharing = ParticleUI.MeshSharing.Replica;
                CreateSystem(primary.transform);
                CreateSystem(replica.transform);
                primary.RefreshParticles();
                replica.RefreshParticles();
                primary.Play();
                yield return new WaitForSeconds(0.2f);
                Assert.IsTrue(primary.isPlaying);
                Assert.IsFalse(replica.isPlaying);
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }
    }
}
