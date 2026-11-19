using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ParticleUI = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue373.Tests
{
    public class ParticleLifetimeTests
    {
        private static ParticleSystem CreateSystem(Transform parent)
        {
            var go = new GameObject("ParticleSystem");
            go.SetActive(false);
            go.transform.SetParent(parent, false);
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = false;
            main.startLifetime = 1;
            main.startSpeed = 0;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            var emission = ps.emission;
            emission.enabled = false;
            var shape = ps.shape;
            shape.enabled = false;
            go.SetActive(true);
            return ps;
        }

        [Explicit("直接Emit後の自動再生により寿命が短縮する既知の問題です。")]
        [UnityTest]
        public IEnumerator ScriptedEmissionLifetimeMatchesNativeSimulation()
        {
            yield return CompareLifetime(false);
        }

        [UnityTest]
        public IEnumerator PausingAfterEachEmitAvoidsDoubleSimulation()
        {
            yield return CompareLifetime(true);
        }

        private IEnumerator CompareLifetime(bool pauseAfterEmit)
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            var nativeRoot = new GameObject("Native");
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var root = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
                root.transform.SetParent(canvas.transform, false);
                var ui = root.AddComponent<ParticleUI>();
                var uiPs = CreateSystem(root.transform);
                var native = CreateSystem(nativeRoot.transform);
                ui.RefreshParticles();
                var uiBuffer = new ParticleSystem.Particle[128];
                var nativeBuffer = new ParticleSystem.Particle[128];
                var starts = new Dictionary<uint, float>();
                var uiLast = new Dictionary<uint, float>();
                var nativeLast = new Dictionary<uint, float>();
                var emitted = 0u;
                var next = Time.realtimeSinceStartup;
                var wasPlaying = 0;
                yield return null;
                var deadline = Time.realtimeSinceStartup + 15;
                while (emitted < 100 || uiPs.particleCount > 0 || native.particleCount > 0)
                {
                    Assert.Less(Time.realtimeSinceStartup, deadline, "The emission comparison timed out.");
                    if (emitted < 100 && Time.realtimeSinceStartup >= next)
                    {
                        next = Time.realtimeSinceStartup + 0.01f;
                        var parameters = new ParticleSystem.EmitParams
                        {
                            randomSeed = emitted + 1,
                            startLifetime = 1
                        };
                        uiPs.Emit(parameters, 1);
                        if (uiPs.isPlaying) wasPlaying++;
                        if (pauseAfterEmit) uiPs.Pause(false);
                        native.Emit(parameters, 1);
                        starts.Add(emitted + 1, Time.realtimeSinceStartup);
                        emitted++;
                    }

                    var count = uiPs.GetParticles(uiBuffer);
                    for (var i = 0; i < count; i++) uiLast[uiBuffer[i].randomSeed] = Time.realtimeSinceStartup;
                    uiPs.SetParticles(uiBuffer, count);
                    count = native.GetParticles(nativeBuffer);
                    for (var i = 0; i < count; i++) nativeLast[nativeBuffer[i].randomSeed] = Time.realtimeSinceStartup;
                    native.SetParticles(nativeBuffer, count);
                    yield return null;
                }

                Assert.AreEqual(100, uiLast.Count);
                Assert.AreEqual(100, nativeLast.Count);
                var worst = 0f;
                for (uint seed = 1; seed <= 100; seed++)
                {
                    var uiLifetime = uiLast[seed] - starts[seed];
                    var nativeLifetime = nativeLast[seed] - starts[seed];
                    worst = Mathf.Max(worst, Mathf.Abs(uiLifetime - nativeLifetime));
                }
                Assert.Less(worst, 0.1f, $"Maximum difference from native: {worst:F3}s; playing after Emit: {wasPlaying}; first UI/native: {uiLast[1] - starts[1]:F3}/{nativeLast[1] - starts[1]:F3}; last: {uiLast[100] - starts[100]:F3}/{nativeLast[100] - starts[100]:F3}");
            }
            finally
            {
                Object.Destroy(canvas);
                Object.Destroy(nativeRoot);
            }
        }
    }
}
