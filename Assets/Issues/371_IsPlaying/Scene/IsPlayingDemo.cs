using UnityEngine;
using ParticleUI = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue371.Scene
{
    public class IsPlayingDemo : MonoBehaviour
    {
        [SerializeField] private ParticleUI uiParticle;
        [SerializeField] private ParticleSystem particleSystem;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20, 20, 450, 260));
            GUILayout.Label($"UIParticle.isPlaying: {uiParticle.isPlaying}");
            GUILayout.Label($"ParticleSystem.isPlaying: {particleSystem.isPlaying}");
            GUILayout.Label($"Live particles: {particleSystem.particleCount}");
            if (GUILayout.Button("Play")) uiParticle.Play();
            if (GUILayout.Button("Pause")) uiParticle.Pause();
            if (GUILayout.Button("Resume")) uiParticle.Resume();
            if (GUILayout.Button("Stop emission")) uiParticle.StopEmission();
            if (GUILayout.Button("Start emission")) uiParticle.StartEmission();
            if (GUILayout.Button("Stop")) uiParticle.Stop();
            if (GUILayout.Button("Clear")) uiParticle.Clear();
            GUILayout.EndArea();
        }
    }
}
