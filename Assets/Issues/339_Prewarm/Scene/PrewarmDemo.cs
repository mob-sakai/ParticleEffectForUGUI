using System.Collections;
using UnityEngine;

namespace Coffee.UIParticle.Issue339.Scene
{
    public class PrewarmDemo : MonoBehaviour
    {
        [SerializeField] private GameObject[] windows;
        [SerializeField] private ParticleSystem[] uiSystems;
        [SerializeField] private ParticleSystem[] nativeSystems;
        private readonly int[] uiCounts = new int[3];
        private readonly int[] nativeCounts = new int[3];
        private bool capturing;

        private IEnumerator Start()
        {
            yield return Capture();
        }

        private IEnumerator Capture()
        {
            capturing = true;
            foreach (var window in windows) window.SetActive(false);
            yield return null;
            foreach (var window in windows) window.SetActive(true);
            yield return null;
            yield return new WaitForEndOfFrame();
            for (var i = 0; i < uiSystems.Length; i++)
            {
                uiCounts[i] = uiSystems[i].particleCount;
                nativeCounts[i] = nativeSystems[i].particleCount;
            }
            capturing = false;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(30, 30, 600, 400));
            GUILayout.Label("Issue #339: Prewarm on activation (Lifetime 5, rate 10/s)");
            var durations = new[] { "1", "2.5", "5" };
            for (var i = 0; i < uiCounts.Length; i++)
                GUILayout.Label($"Duration {durations[i]}: UI Particle {uiCounts[i]}, native {nativeCounts[i]}");
            if (!capturing && GUILayout.Button("Deactivate and reactivate"))
                StartCoroutine(Capture());
            GUILayout.EndArea();
        }
    }
}
