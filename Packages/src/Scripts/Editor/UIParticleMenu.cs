using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    internal class UIParticleMenu
    {
        [MenuItem("GameObject/UI/Particle System (Empty)", false, 2018)]
        private static void AddParticleEmpty(MenuCommand menuCommand)
        {
            // Create empty UI element.
            EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
            var ui = Selection.activeGameObject;
            Object.DestroyImmediate(ui.GetComponent<Image>());

            // Add UIParticle.
            var uiParticle = ui.AddComponent<UIParticle>();
            uiParticle.name = "UIParticle";
            uiParticle.scale = 10;
            uiParticle.rectTransform.sizeDelta = Vector2.zero;
        }

        [MenuItem("GameObject/UI/Particle System", false, 2019)]
        private static void AddParticle(MenuCommand menuCommand)
        {
            // Create empty UIEffect.
            AddParticleEmpty(menuCommand);
            var uiParticle = Selection.activeGameObject.GetComponent<UIParticle>();

            // Create ParticleSystem.
            EditorApplication.ExecuteMenuItem("GameObject/Effects/Particle System");
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
