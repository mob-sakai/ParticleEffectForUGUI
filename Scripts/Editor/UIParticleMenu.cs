using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    public class UIParticleMenu
    {
#if !UNITY_2019_1_OR_NEWER
        static string GetPreviousSamplePath(string displayName, string sampleName)
        {
            string sampleRoot = string.Format("Assets/Samples/{0}", displayName);
            var sampleRootInfo = new DirectoryInfo(sampleRoot);
            if (!sampleRootInfo.Exists) return null;

            foreach (var versionDir in sampleRootInfo.GetDirectories())
            {
                var samplePath = Path.Combine(versionDir.ToString(), sampleName);
                if (Directory.Exists(samplePath))
                    return samplePath;
            }

            return null;
        }


        static void ImportSample(string packageName, string sampleName)
        {
            string jsonPath = string.Format("Packages/{0}/package.json", packageName);
            string json = File.ReadAllText(jsonPath);
            string version = Regex.Match(json, "\"version\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
            string displayName = Regex.Match(json, "\"displayName\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
            string src = string.Format("{0}/Samples~/{1}", Path.GetDirectoryName(jsonPath), sampleName);
            string dst = string.Format("Assets/Samples/{0}/{1}/{2}", displayName, version, sampleName);
            string previous = GetPreviousSamplePath(displayName, sampleName);

            if (!string.IsNullOrEmpty(previous))
            {
                string msg = "A different version of the sample is already imported at\n\n"
                             + previous
                             + "\n\nIt will be deleted when you update. Are you sure you want to continue?";
                if (!EditorUtility.DisplayDialog("Sample Importer", msg, "OK", "Cancel"))
                    return;

                FileUtil.DeleteFileOrDirectory(previous + ".meta");
                FileUtil.DeleteFileOrDirectory(previous);

                string versionDir = Path.GetDirectoryName(previous);
                if (Directory.GetFiles(versionDir, "*.meta", SearchOption.TopDirectoryOnly).Length == 0)
                {
                    FileUtil.DeleteFileOrDirectory(versionDir + ".meta");
                    FileUtil.DeleteFileOrDirectory(versionDir);
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(dst));
            FileUtil.CopyFileOrDirectory(src, dst);
            AssetDatabase.ImportAsset(dst, ImportAssetOptions.ImportRecursive);
        }

        [MenuItem("Assets/Samples/Import UIParticle Demo")]
        static void ImportSample()
        {
            ImportSample("com.coffee.ui-particle", "Demo");
        }

        [MenuItem("Assets/Samples/Import UIParticle Demo (Cartoon FX & War FX Demo)")]
        static void ImportSample_CFX()
        {
            ImportSample("com.coffee.ui-particle", "Cartoon FX & War FX Demo");
        }
#endif

        [MenuItem("GameObject/UI/Particle System (Empty)", false, 2018)]
        public static void AddParticleEmpty(MenuCommand menuCommand)
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
        public static void AddParticle(MenuCommand menuCommand)
        {
            // Create empty UIEffect.
            AddParticleEmpty(menuCommand);
            var uiParticle = Selection.activeGameObject.GetComponent<UIParticle>();

            // Create ParticleSystem.
            EditorApplication.ExecuteMenuItem("GameObject/Effects/Particle System");
            var ps = Selection.activeGameObject;
            ps.transform.SetParent(uiParticle.transform, false);
            ps.transform.localPosition = Vector3.zero;

            // Assign default material.
            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            var defaultMat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
            renderer.material = defaultMat ? defaultMat : renderer.material;

            // Refresh particles.
            uiParticle.RefreshParticles();
        }
    }
}
