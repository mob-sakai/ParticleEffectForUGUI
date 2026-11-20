using UnityEngine;
using ParticleAttractor = Coffee.UIExtensions.UIParticleAttractor;
using ParticleUI = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue378.Scene
{
    public class AttractorSmoothDemo : MonoBehaviour
    {
        [SerializeField] private ParticleAttractor attractor;
        [SerializeField] private ParticleUI uiParticle;
        [SerializeField] private ParticleSystem particleSystem;

        private static readonly ParticleAttractor.Movement[] s_Movements =
        {
            ParticleAttractor.Movement.Linear,
            ParticleAttractor.Movement.Smooth,
            ParticleAttractor.Movement.Sphere,
            ParticleAttractor.Movement.Hemisphere,
            ParticleAttractor.Movement.InSmooth,
            ParticleAttractor.Movement.OutSmooth
        };

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20, 20, 360, 390), GUI.skin.box);
            GUILayout.Label("Particle Attractor");

            if (attractor == null)
            {
                GUILayout.Label("Assign an Attractor to this demo.");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label($"Movement: {attractor.movement}");
            DrawMovementButtons();

            GUILayout.Space(8);
            GUILayout.Label($"Max speed: {attractor.maxSpeed:F1}");
            attractor.maxSpeed = GUILayout.HorizontalSlider(attractor.maxSpeed, 1, 200);

            GUILayout.Space(8);
            var isRegistered = IsParticleSystemRegistered();
            GUILayout.Label($"Registered ParticleSystems: {attractor.particleSystems.Count}");
            GUILayout.BeginHorizontal();
            GUI.enabled = particleSystem != null && !isRegistered;
            if (GUILayout.Button("Register target")) attractor.AddParticleSystem(particleSystem);
            GUI.enabled = particleSystem != null && isRegistered;
            if (GUILayout.Button("Remove target")) attractor.RemoveParticleSystem(particleSystem);
            GUI.enabled = 0 < attractor.particleSystems.Count;
            if (GUILayout.Button("Clear targets")) attractor.ClearParticleSystems();
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUI.enabled = uiParticle != null;
            if (GUILayout.Button("Emit 20")) uiParticle.Emit(20);
            if (GUILayout.Button("Clear particles")) uiParticle.Clear();
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawMovementButtons()
        {
            for (var i = 0; i < s_Movements.Length; i += 2)
            {
                GUILayout.BeginHorizontal();
                DrawMovementButton(s_Movements[i]);
                if (i + 1 < s_Movements.Length)
                {
                    DrawMovementButton(s_Movements[i + 1]);
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DrawMovementButton(ParticleAttractor.Movement movement)
        {
            var isSelected = attractor.movement == movement;
            if (GUILayout.Toggle(isSelected, movement.ToString(), "Button"))
            {
                attractor.movement = movement;
            }
        }

        private bool IsParticleSystemRegistered()
        {
            if (particleSystem == null)
            {
                return false;
            }

            var particleSystems = attractor.particleSystems;
            for (var i = 0; i < particleSystems.Count; i++)
            {
                if (particleSystems[i] == particleSystem)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
