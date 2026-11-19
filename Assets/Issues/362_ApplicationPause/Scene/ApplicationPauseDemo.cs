using System.Collections;
using System.Threading;
using Coffee.UIExtensions;
using UnityEngine;
using ParticleUI = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue362.Scene
{
    public class ApplicationPauseDemo : MonoBehaviour
    {
        [SerializeField] private ParticleUI ui;
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private UIParticleAttractor attractor;
        private readonly ParticleSystem.Particle[] buffer = new ParticleSystem.Particle[1];
        private string result = "Press the button to simulate a long frame.";
        private bool running;

        private IEnumerator Start()
        {
            attractor.AddParticleSystem(particles);
            ui.Emit(1);
            yield return null;
        }

        private IEnumerator Run()
        {
            running = true;
            yield return new WaitForEndOfFrame();
            if (particles.GetParticles(buffer) != 1)
            {
                ui.Emit(1);
                particles.GetParticles(buffer);
            }
            var before = buffer[0];
            Thread.Sleep(1000);
            yield return null;
            yield return new WaitForEndOfFrame();
            if (particles.GetParticles(buffer) == 1)
            {
                result = $"unscaled delta: {Time.unscaledDeltaTime:F3}s, maximum: {Time.maximumDeltaTime:F3}s\n"
                         + $"particle lifetime advanced: {before.remainingLifetime - buffer[0].remainingLifetime:F3}s\n"
                         + $"attractor displacement: {Vector3.Distance(before.position, buffer[0].position):F3}";
            }
            else
            {
                result = "Particle was lost during the long frame.";
            }
            running = false;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(30, 30, 650, 200));
            GUILayout.Label("Issue #362: unscaled time after a long frame");
            GUILayout.Label(result);
            if (!running && GUILayout.Button("Simulate 1 second stall")) StartCoroutine(Run());
            GUILayout.EndArea();
        }
    }
}
