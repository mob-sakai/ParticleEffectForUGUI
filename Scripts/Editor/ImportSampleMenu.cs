#if !UNITY_2019_1_OR_NEWER
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Coffee.UIExtensions
{
    internal static class ImportSampleMenu_UIParticle
    {
        private const string k_DisplayName = "UI Particle";
        private const string k_JsonGuid = "823dc693d087a4b559c7e1547274cc7d";

        [MenuItem("Assets/Samples/" + k_DisplayName + "/Demo")]
        private static void ImportSample()
        {
            ImportSample(k_JsonGuid, "Demo");
        }

        [MenuItem("Assets/Samples/" + k_DisplayName + "/Cartoon FX & War FX Demo")]
        private static void ImportSample_CFX()
        {
            ImportSample(k_JsonGuid, "Cartoon FX & War FX Demo");
        }

        private static void ImportSample(string jsonGuid, string sampleName)
        {
            var jsonPath = AssetDatabase.GUIDToAssetPath(jsonGuid);
            var packageRoot = Path.GetDirectoryName(jsonPath).Replace('\\', '/');
            var json = File.ReadAllText(jsonPath);
            var version = Regex.Match(json, "\"version\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
            var src = string.Format("{0}/Samples~/{1}", packageRoot, sampleName);
            var dst = string.Format("Assets/Samples/{0}/{1}/{2}", k_DisplayName, version, sampleName);
            var previousPath = GetPreviousSamplePath(k_DisplayName, sampleName);

            // Remove the previous sample directory.
            if (!string.IsNullOrEmpty(previousPath))
            {
                var msg = "A different version of the sample is already imported at\n\n"
                          + previousPath
                          + "\n\nIt will be deleted when you update. Are you sure you want to continue?";
                if (!EditorUtility.DisplayDialog("Sample Importer", msg, "OK", "Cancel"))
                    return;

                FileUtil.DeleteFileOrDirectory(previousPath);

                var metaFile = previousPath + ".meta";
                if (File.Exists(metaFile))
                    FileUtil.DeleteFileOrDirectory(metaFile);
            }

            if (!Directory.Exists(dst))
                FileUtil.DeleteFileOrDirectory(dst);

            var dstDir = Path.GetDirectoryName(dst);
            if (!Directory.Exists(dstDir))
                Directory.CreateDirectory(dstDir);

            if (Directory.Exists(src))
                FileUtil.CopyFileOrDirectory(src, dst);
            else
                throw new DirectoryNotFoundException(src);

            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        }

        private static string GetPreviousSamplePath(string displayName, string sampleName)
        {
            var sampleRoot = string.Format("Assets/Samples/{0}", displayName);
            var sampleRootInfo = new DirectoryInfo(sampleRoot);
            if (!sampleRootInfo.Exists) return null;

            return sampleRootInfo.GetDirectories()
                .Select(versionDir => Path.Combine(versionDir.ToString(), sampleName))
                .FirstOrDefault(Directory.Exists);
        }
    }
}
#endif
