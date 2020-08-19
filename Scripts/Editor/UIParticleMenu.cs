using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

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

        [MenuItem("Assets/Samples/Import UIParticle Sample")]
        static void ImportSample()
        {
            ImportSample("com.coffee.ui-particle", "Demo");
        }
#endif


        [MenuItem("GameObject/UI/Particle System", false, 2019)]
        public static void AddParticle(MenuCommand menuCommand)
        {
            // Create UI element.
            EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
            var ui = Selection.activeGameObject;

            // Create ParticleSystem.
            EditorApplication.ExecuteMenuItem("GameObject/Effects/Particle System");
            var ps = Selection.activeGameObject;
            var transform = ps.transform;
            var localRotation = transform.localRotation;

            transform.SetParent(ui.transform.parent, true);
            var pos = transform.localPosition;
            pos.z = 0;
            ps.transform.localPosition = pos;
            ps.transform.localRotation = localRotation;

            // Destroy UI elemant
            Object.DestroyImmediate(ui);

            // Assign default material.
            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            var defaultMat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
            renderer.material = defaultMat ? defaultMat : renderer.material;

            // Set to hierarchy mode
            var particleSystem = ps.GetComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;

            // Add UIParticle.
            var uiParticle = ps.AddComponent<UIParticle>();
            uiParticle.ignoreCanvasScaler = true;
            uiParticle.scale = 10;
        }
    }
}
