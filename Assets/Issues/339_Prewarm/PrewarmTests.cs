using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ParticleUI = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue339.Tests
{
    public class PrewarmTests
    {
        private static ParticleSystem CreateSystem(Transform parent, float duration, bool prewarm)
        {
            var go = new GameObject("ParticleSystem");
            go.SetActive(false);
            go.transform.SetParent(parent, false);
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = true;
            main.loop = true;
            main.duration = duration;
            main.prewarm = prewarm;
            main.startLifetime = 5;
            main.startSpeed = 0;
            main.maxParticles = 200;
            var emission = ps.emission;
            emission.rateOverTime = 10;
            var shape = ps.shape;
            shape.enabled = false;
            go.SetActive(true);
            return ps;
        }

        [Explicit("Durationより長い寿命ではUnity標準と初期粒子数が一致しません。")]
        [UnityTest]
        public IEnumerator PrewarmMatchesNativeOneCycleAfterActivationAndReactivation()
        {
            var results = new List<string>();
            var differences = new List<string>();
            foreach (var duration in new[] { 1f, 2.5f, 5f })
            {
                var canvas = new GameObject("Canvas", typeof(Canvas));
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var uiWindow = new GameObject("UI window", typeof(RectTransform), typeof(CanvasRenderer));
                var nativeWindow = new GameObject("Native window");
                uiWindow.SetActive(false);
                nativeWindow.SetActive(false);
                uiWindow.transform.SetParent(canvas.transform, false);
                try
                {
                    uiWindow.AddComponent<ParticleUI>();
                    var uiPs = CreateSystem(uiWindow.transform, duration, true);
                    var nativePs = CreateSystem(nativeWindow.transform, duration, true);
                    for (var activation = 0; activation < 2; activation++)
                    {
                        uiWindow.SetActive(true);
                        nativeWindow.SetActive(true);
                        yield return null;
                        yield return new WaitForEndOfFrame();
                        var uiCount = uiPs.particleCount;
                        var nativeCount = nativePs.particleCount;
                        var result = $"Duration={duration}, activation={activation}, UI={uiCount}, native={nativeCount}";
                        results.Add(result);
                        if (Mathf.Abs(uiCount - nativeCount) > 2) differences.Add(result);
                        uiWindow.SetActive(false);
                        nativeWindow.SetActive(false);
                        yield return null;
                    }
                }
                finally
                {
                    Object.Destroy(canvas);
                    Object.Destroy(nativeWindow);
                }
                yield return null;
            }
            Assert.IsEmpty(differences, string.Join("; ", results));
        }

        [UnityTest]
        public IEnumerator NoPrewarmDoesNotStartWithOneCycleOfParticles()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var window = new GameObject("UI window", typeof(RectTransform), typeof(CanvasRenderer));
            window.SetActive(false);
            window.transform.SetParent(canvas.transform, false);
            try
            {
                window.AddComponent<ParticleUI>();
                var ps = CreateSystem(window.transform, 2.5f, false);
                window.SetActive(true);
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.Less(ps.particleCount, 5);
            }
            finally { Object.Destroy(canvas); }
        }

        [UnityTest]
        public IEnumerator PrewarmPreservesParticlesEmittedBeforeFirstUpdate()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var window = new GameObject("UI window", typeof(RectTransform), typeof(CanvasRenderer));
            window.SetActive(false);
            window.transform.SetParent(canvas.transform, false);
            try
            {
                var ui = window.AddComponent<ParticleUI>();
                var ps = CreateSystem(window.transform, 1, true);
                window.SetActive(true);
                ui.Emit(1);
                var before = new ParticleSystem.Particle[200];
                var count = ps.GetParticles(before);
                Assert.Greater(count, 0);
                var seed = before[count - 1].randomSeed;
                yield return null;
                yield return new WaitForEndOfFrame();
                var after = new ParticleSystem.Particle[200];
                count = ps.GetParticles(after);
                var found = false;
                for (var i = 0; i < count; i++) found |= after[i].randomSeed == seed;
                Assert.IsTrue(found, "An explicitly emitted particle must survive the first simulation update.");
            }
            finally { Object.Destroy(canvas); }
        }
    }
}
