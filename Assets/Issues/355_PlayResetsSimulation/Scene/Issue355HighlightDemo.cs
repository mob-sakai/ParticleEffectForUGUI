using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIParticle.Issue355
{
    public class Issue355HighlightDemo : MonoBehaviour
    {
        [SerializeField] private Coffee.UIExtensions.UIParticle uiParticle;
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private float transitionInterval = 3f;

        private readonly Color[] colors = { Color.cyan, Color.green, Color.red };
        private readonly string[] colorNames = { "BLUE", "GREEN", "RED" };
        private ParticleSystem.Particle[] beforeParticles;
        private ParticleSystem.Particle[] afterParticles;
        private readonly Dictionary<uint, int> particleIds = new Dictionary<uint, int>();
        private GUIStyle labelStyle;
        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private int colorIndex;
        private float nextTransition;
        private bool hasResult;
        private bool lastTransitionPassed;
        private int transitions;
        private int failedTransitions;
        private int beforeCount;
        private int afterCount;
        private int preservedCount;

        public int Transitions => transitions;
        public int FailedTransitions => failedTransitions;
        public int BeforeCount => beforeCount;
        public int AfterCount => afterCount;
        public int PreservedCount => preservedCount;

        private void Start()
        {
            beforeParticles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
            afterParticles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
            SetColor();
            uiParticle.Play();
            nextTransition = Time.time + transitionInterval;
        }

        private void Update()
        {
            if (Time.time >= nextTransition)
            {
                ChangeHighlight();
            }
        }

        private void ChangeHighlight()
        {
            beforeCount = particleSystem.GetParticles(beforeParticles);
            particleIds.Clear();
            for (var i = 0; i < beforeCount; i++)
            {
                var id = beforeParticles[i].randomSeed;
                particleIds.TryGetValue(id, out var count);
                particleIds[id] = count + 1;
            }

            colorIndex = (colorIndex + 1) % colors.Length;
            SetColor();
            uiParticle.Play();

            afterCount = particleSystem.GetParticles(afterParticles);
            preservedCount = 0;
            for (var i = 0; i < afterCount; i++)
            {
                var id = afterParticles[i].randomSeed;
                if (!particleIds.TryGetValue(id, out var count) || count == 0) continue;
                preservedCount++;
                particleIds[id] = count - 1;
            }

            lastTransitionPassed = beforeCount > 0 && afterCount == beforeCount && preservedCount == beforeCount;
            hasResult = true;
            transitions++;
            if (!lastTransitionPassed) failedTransitions++;
            nextTransition = Time.time + transitionInterval;
        }

        private void SetColor()
        {
            var main = particleSystem.main;
            main.startColor = colors[colorIndex];
        }

        private void OnGUI()
        {
            var scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 20 };
                boxStyle = new GUIStyle(GUI.skin.box) { fontSize = 20 };
                buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 18 };
            }

            GUI.Box(new Rect(30, 25, 710, 245), "Issue #355 - Highlight color changes", boxStyle);
            GUI.Label(new Rect(50, 60, 670, 25), $"Every {transitionInterval:g} seconds: change color, then call UIParticle.Play()", labelStyle);
            GUI.Label(new Rect(50, 90, 670, 25), $"Current highlight: {colorNames[colorIndex]}", labelStyle);
            GUI.Label(new Rect(50, 120, 570, 25), hasResult
                ? $"Last transition: {(lastTransitionPassed ? "PASS" : "FAIL")}  |  transitions: {transitions}  failures: {failedTransitions}"
                : "Waiting for the first transition...", labelStyle);
            GUI.Label(new Rect(50, 150, 670, 25),
                $"Particles before / after / preserved: {beforeCount} / {afterCount} / {preservedCount}", labelStyle);
            if (GUI.Button(new Rect(50, 190, 250, 45), "Change color + Play()", buttonStyle))
            {
                ChangeHighlight();
            }
        }
    }
}
