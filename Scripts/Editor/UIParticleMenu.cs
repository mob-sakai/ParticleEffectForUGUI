#if IGNORE_ACCESS_CHECKS // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
#if !UNITY_2019_1_OR_NEWER
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Coffee.UIExtensions
{
    public class UIParticleMenu
    {
        static string GetPreviousSamplePath(string displayName, string sampleName)
        {
            string sampleRoot = $"Assets/Samples/{displayName}";
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
            string jsonPath = $"Packages/{packageName}/package.json";
            string json = File.ReadAllText(jsonPath);
            string version = Regex.Match(json, "\"version\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
            string displayName = Regex.Match(json, "\"displayName\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
            string src = $"{Path.GetDirectoryName(jsonPath)}/Samples~/{sampleName}";
            string dst = $"Assets/Samples/{displayName}/{version}/{sampleName}";
            string previous = GetPreviousSamplePath(displayName, sampleName);

            if (!string.IsNullOrEmpty(previous))
            {
                string msg = "A different version of the sample is already imported at\n\n"
                    + previous
                    + "\n\nIt will be deleted when you update. Are you sure you want to continue?";
                if (!EditorUtility.DisplayDialog("Sample Importer", msg, "OK", "Cancel"))
                    return;

                FileUtil.DeleteFileOrDirectory(previous);
                FileUtil.DeleteFileOrDirectory(previous + ".meta");
            }

            FileUtil.CopyDirectoryRecursive(src, dst);
            AssetDatabase.ImportAsset(dst, ImportAssetOptions.ImportRecursive);
        }

        [MenuItem("Assets/Samples/Import UIParticle Sample")]
        static void ImportSample()
        {
            ImportSample("com.coffee.ui-particle", "Demo");
        }
    }
}
#endif
#endif // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.