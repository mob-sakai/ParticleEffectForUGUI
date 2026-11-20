using UnityEngine;
using ParticleAttractor = Coffee.UIExtensions.UIParticleAttractor;
using ParticleUI = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue321.Scene
{
    public class AttractorDelayCurveDemo : MonoBehaviour
    {
        [SerializeField] private ParticleAttractor attractor;
        [SerializeField] private ParticleUI uiParticle;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20, 20, 500, 200));
            GUILayout.Label($"Delay mode: {attractor.delayRate.mode}");
            GUILayout.Label($"Particle count: {uiParticle.particles[0].particleCount}");
            if (GUILayout.Button("Emit particles")) uiParticle.Emit(20);
            GUILayout.EndArea();
        }
    }
}
