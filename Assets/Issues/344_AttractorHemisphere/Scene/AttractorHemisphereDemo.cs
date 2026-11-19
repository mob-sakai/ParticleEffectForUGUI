using UnityEngine;
using ParticleAttractor = Coffee.UIExtensions.UIParticleAttractor;
using ParticleUI = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue344.Scene
{
    public class AttractorHemisphereDemo : MonoBehaviour
    {
        [SerializeField] private ParticleAttractor attractor;
        [SerializeField] private ParticleUI uiParticle;
        [SerializeField] private ParticleSystem particleSystem;
        private float radius = 2;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20, 20, 480, 250));
            GUILayout.Label($"Movement: {attractor.movement}");
            radius = GUILayout.HorizontalSlider(radius, 0, 200);
            GUILayout.Label($"Hemisphere radius: {radius:F2}");
            attractor.hemisphereRadius = radius;
            if (GUILayout.Button("Hemisphere")) attractor.movement = ParticleAttractor.Movement.Hemisphere;
            if (GUILayout.Button("Linear")) attractor.movement = ParticleAttractor.Movement.Linear;
            if (GUILayout.Button("Emit")) uiParticle.Emit(20);
            GUILayout.EndArea();
        }
    }
}
