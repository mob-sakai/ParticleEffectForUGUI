#if !OPEN_SESAME && UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;

namespace Coffee.OpenSesame
{
    [InitializeOnLoad]
    internal class Portable
    {
        static Portable()
        {
            var assemblyName = typeof(Portable).Assembly.GetName().Name;
            var asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
            UnityEngine.Debug.LogFormat("<b>Request to recompile: {0} ({1})</b>", assemblyName, asmdefPath);
            AssetDatabase.ImportAsset(asmdefPath);
        }
    }
}
#endif
