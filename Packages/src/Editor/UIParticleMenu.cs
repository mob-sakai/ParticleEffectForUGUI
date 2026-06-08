using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    internal class UIParticleMenu
    {
#if UNITY_6000_5_OR_NEWER
        private const string k_MenuPathToCreateParticleSystem = "GameObject/Visual Effects/Particle System";
#else
        private const string k_MenuPathToCreateParticleSystem = "GameObject/Effects/Particle System";
#endif
#if UNITY_6000_3_OR_NEWER
        private const string k_MenuPathForUgui = "GameObject/UI (Canvas)";
#else
        private const string k_MenuPathForUgui = "GameObject/UI";
#endif

        [MenuItem(k_MenuPathForUgui + "/Particle System (Empty)", false, 2018)]
        private static void AddParticleEmpty(MenuCommand menuCommand)
        {
            // Create empty UI element.
            EditorApplication.ExecuteMenuItem(k_MenuPathForUgui + "/Image");
            var ui = Selection.activeGameObject;
            Object.DestroyImmediate(ui.GetComponent<Image>());

            // Add UIParticle.
            var uiParticle = ui.AddComponent<UIParticle>();
            uiParticle.name = "UIParticle";
            uiParticle.scale = 10;
            uiParticle.rectTransform.sizeDelta = Vector2.zero;
        }

        [MenuItem(k_MenuPathForUgui + "/Particle System", false, 2019)]
        private static void AddParticle(MenuCommand menuCommand)
        {
            // Create empty UIEffect.
            AddParticleEmpty(menuCommand);
            var uiParticle = Selection.activeGameObject.GetComponent<UIParticle>();

            // Create ParticleSystem.
            EditorApplication.ExecuteMenuItem(k_MenuPathToCreateParticleSystem);
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
