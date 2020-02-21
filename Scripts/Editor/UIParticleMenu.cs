#if IGNORE_ACCESS_CHECKS // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
#if !UNITY_2019_1_OR_NEWER
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Coffee.UIExtensions
{
    public class UIParticleMenu
    {
        [MenuItem("Assets/Samples/Import UIParticle Sample")]
        static void ImportSample()
        {
            const string sampleGuid = "dc0fe9e7fe61947fab1522ab29e2fc88";
            const string jsonGuid = "823dc693d087a4b559c7e1547274cc7d";
            const string SAMPLE_NAME = "Demo";

            string jsonPath = AssetDatabase.GUIDToAssetPath(jsonGuid);
            string json = File.ReadAllText(jsonPath);
            string version = Regex.Match(json, "\"version\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
            string displayName = Regex.Match(json, "\"displayName\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
            string src = Path.GetDirectoryName(jsonPath) + "/Samples~/" + SAMPLE_NAME;
            string dst = string.Format("Assets/Samples/{0}/{1}/{2}",displayName, version, SAMPLE_NAME);

            // Remove old samples
            string samplePath = AssetDatabase.GUIDToAssetPath(sampleGuid);
            if (samplePath.StartsWith("Assets/") && FileUtil.PathExists(samplePath))
            {
                FileUtil.DeleteFileOrDirectory(samplePath);
                FileUtil.DeleteFileOrDirectory(samplePath + ".meta");
            }

            FileUtil.CopyDirectoryRecursive(src, dst);
            FileUtil.CopyFileOrDirectory(src + ".meta", dst + ".meta");
            AssetDatabase.ImportAsset(dst, ImportAssetOptions.ImportRecursive);
        }
    }
}
#endif
#endif // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.