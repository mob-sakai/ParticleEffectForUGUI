using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coffee.UIParticle.Issue355.Tests
{
    public class PlayResetsSimulationTests
    {
        [UnityTest]
        public IEnumerator PlayDoesNotResetAnAlreadyRunningSimulation()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

                var particleObject = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
                particleObject.transform.SetParent(canvasObject.transform, false);
                var uiParticle = particleObject.AddComponent<Coffee.UIExtensions.UIParticle>();

                var systemObject = new GameObject("ParticleSystem");
                systemObject.SetActive(false);
                systemObject.transform.SetParent(particleObject.transform, false);
                var particleSystem = systemObject.AddComponent<ParticleSystem>();
                var main = particleSystem.main;
                main.playOnAwake = false;
                main.loop = true;
                main.startLifetime = 10f;
                main.startSpeed = 0f;
                var emission = particleSystem.emission;
                emission.rateOverTime = 10f;
                systemObject.SetActive(true);

                uiParticle.RefreshParticles();
                uiParticle.Play();
                yield return new WaitForSeconds(0.5f);
                Assert.Greater(particleSystem.particleCount, 0, "初回Play後に粒子が生成される必要があります。");
                var runningParticleCount = particleSystem.particleCount;
                uiParticle.Play();
                Assert.AreEqual(runningParticleCount, particleSystem.particleCount);

                particleSystem.Simulate(1f, false, true, false);
                var particleCountBeforePlay = particleSystem.particleCount;
                Assert.Greater(particleCountBeforePlay, 0);

                uiParticle.Play();

                Assert.AreEqual(particleCountBeforePlay, particleSystem.particleCount);

                uiParticle.Pause();
                Assert.IsTrue(uiParticle.isPaused);
                uiParticle.Resume();
                Assert.IsFalse(uiParticle.isPaused);
                Assert.AreEqual(particleCountBeforePlay, particleSystem.particleCount);

                uiParticle.Pause();
                uiParticle.Play();
                Assert.IsFalse(uiParticle.isPaused);
                Assert.AreEqual(0, particleSystem.particleCount);

                particleSystem.Simulate(1f, false, false, false);
                Assert.Greater(particleSystem.particleCount, 0);
                uiParticle.Stop();
                Assert.IsTrue(uiParticle.isPaused);
                uiParticle.Play();
                Assert.IsFalse(uiParticle.isPaused);
                Assert.AreEqual(0, particleSystem.particleCount);
                particleSystem.Simulate(1f, false, false, false);
                Assert.Greater(particleSystem.particleCount, 0);
            }
            finally
            {
                Object.Destroy(canvasObject);
            }

            yield return null;
        }
    }
}
