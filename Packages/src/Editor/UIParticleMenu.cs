using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    internal class UIParticleMenu
    {
#if UNITY_6000_5_OR_NEWER
        [MenuItem("GameObject/UI (Canvas)/Particle System (Empty)", false, 2018)]
#else
        [MenuItem("GameObject/UI/Particle System (Empty)", false, 2018)]
#endif
        private static void AddParticleEmpty(MenuCommand menuCommand)
        {
            // Create empty UI element.
#if UNITY_6000_5_OR_NEWER
            EditorApplication.ExecuteMenuItem("GameObject/UI (Canvas)/Image");
#else
            EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
#endif
            var ui = Selection.activeGameObject;
            Object.DestroyImmediate(ui.GetComponent<Image>());

            // Add UIParticle.
            var uiParticle = ui.AddComponent<UIParticle>();
            uiParticle.name = "UIParticle";
            uiParticle.scale = 10;
            uiParticle.rectTransform.sizeDelta = Vector2.zero;
        }

#if UNITY_6000_5_OR_NEWER
        [MenuItem("GameObject/UI (Canvas)/Particle System", false, 2019)]
#else
        [MenuItem("GameObject/UI/Particle System", false, 2019)]
#endif
        private static void AddParticle(MenuCommand menuCommand)
        {
            // Create empty UIEffect.
            AddParticleEmpty(menuCommand);
            var uiParticle = Selection.activeGameObject.GetComponent<UIParticle>();

            // Create ParticleSystem.
#if UNITY_6000_5_OR_NEWER
            EditorApplication.ExecuteMenuItem("GameObject/Visual Effects/Particle System");
#else
            EditorApplication.ExecuteMenuItem("GameObject/Effects/Particle System");
#endif
            var ps = Selection.activeGameObject;
            ps.transform.SetParent(uiParticle.transform, false);
            ps.transform.localPosition = Vector3.zero;

            // Assign default material (UIAdditive).
            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            var path = AssetDatabase.GUIDToAssetPath("9944483a3e009401ba5dcc42f14d5c63");
            renderer.material = AssetDatabase.LoadAssetAtPath<Material>(path);

            // Refresh particles.
            uiParticle.RefreshParticles();
        }
    }
}
