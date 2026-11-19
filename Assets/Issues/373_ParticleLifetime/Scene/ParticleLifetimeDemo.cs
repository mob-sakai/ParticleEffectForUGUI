using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIParticle.Issue373.Scene
{
    public class ParticleLifetimeDemo : MonoBehaviour
    {
        [SerializeField] private ParticleSystem uiSystem;
        [SerializeField] private ParticleSystem nativeSystem;
        private readonly ParticleSystem.Particle[] buffer = new ParticleSystem.Particle[128];
        private string result = "Run a comparison to measure particle lifetimes.";
        private bool pauseAfterEmit;
        private bool running;

        private IEnumerator Compare()
        {
            running = true;
            uiSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            nativeSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var starts = new Dictionary<uint, float>();
            var uiLast = new Dictionary<uint, float>();
            var nativeLast = new Dictionary<uint, float>();
            var emitted = 0u;
            var next = Time.realtimeSinceStartup;
            var deadline = next + 15;
            while ((emitted < 100 || uiSystem.particleCount > 0 || nativeSystem.particleCount > 0)
                   && Time.realtimeSinceStartup < deadline)
            {
                if (emitted < 100 && Time.realtimeSinceStartup >= next)
                {
                    next = Time.realtimeSinceStartup + 0.01f;
                    var parameters = new ParticleSystem.EmitParams { randomSeed = emitted + 1, startLifetime = 1 };
                    uiSystem.Emit(parameters, 1);
                    if (pauseAfterEmit) uiSystem.Pause(false);
                    nativeSystem.Emit(parameters, 1);
                    starts[emitted + 1] = Time.realtimeSinceStartup;
                    emitted++;
                }

                Record(uiSystem, uiLast);
                Record(nativeSystem, nativeLast);
                yield return null;
            }

            if (uiLast.ContainsKey(1) && nativeLast.ContainsKey(1)
                && uiLast.ContainsKey(100) && nativeLast.ContainsKey(100))
            {
                result = $"First particle: UI {uiLast[1] - starts[1]:F3}s / native {nativeLast[1] - starts[1]:F3}s\n"
                         + $"Last particle: UI {uiLast[100] - starts[100]:F3}s / native {nativeLast[100] - starts[100]:F3}s";
            }
            else
            {
                result = "The comparison did not observe all particles before the timeout.";
            }
            running = false;
        }

        private void Record(ParticleSystem ps, Dictionary<uint, float> timestamps)
        {
            var count = ps.GetParticles(buffer);
            for (var i = 0; i < count; i++) timestamps[buffer[i].randomSeed] = Time.realtimeSinceStartup;
            ps.SetParticles(buffer, count);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(30, 30, 650, 200));
            GUILayout.Label("Issue #373: scripted particle lifetime");
            GUILayout.Label(result);
            if (!running)
            {
                pauseAfterEmit = GUILayout.Toggle(pauseAfterEmit, "Pause UI ParticleSystem immediately after each Emit");
                if (GUILayout.Button("Run comparison")) StartCoroutine(Compare());
            }
            GUILayout.EndArea();
        }
    }
}
