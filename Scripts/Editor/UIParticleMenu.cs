#if IGNORE_ACCESS_CHECKS // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
#if !UNITY_2019_1_OR_NEWER
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
            const string src = "Packages/com.coffee.ui-particle/Samples~/Demo";
            const string dst = "Assets/Samples/UI Particle/3.0.0/Demo";

            if (FileUtil.PathExists(dst))
                FileUtil.DeleteFileOrDirectory(dst);

            FileUtil.CopyDirectoryRecursive(src, dst);
            AssetDatabase.ImportAsset(dst, ImportAssetOptions.ImportRecursive);
        }
    }
}
#endif
#endif // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.