using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
#if !UNITY_2021_2_OR_NEWER
using UnityEngine.Rendering;
#endif

namespace Coffee.UIParticleInternal
{
    internal abstract class ShaderPreProcessorBase : IPreprocessShaders
    {
        public abstract int callbackOrder { get; }

        public abstract void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data);

        protected static void Log(Shader shader, IList<ShaderCompilerData> data, IEnumerable<string> ignoredKeywords)
        {
            Console.WriteLine($"[{shader.name}] {data.Count} variants available:");
            foreach (var (platform, keywords) in data
                         .Select(d => (d.shaderCompilerPlatform, GetKeyWords(shader, d, ignoredKeywords)))
                         .OrderBy(t => t.Item2))
            {
                Console.WriteLine($"  - {platform}: {keywords}");
            }

            Console.WriteLine();
        }

        protected static void StripUnusedVariantsIf(IList<ShaderCompilerData> data, bool condition)
        {
            if (condition)
            {
                data.Clear();
            }
        }

        protected static void StripUnusedVariantsIf(IList<ShaderCompilerData> data, Predicate<ShaderCompilerData> pred)
        {
            for (var i = data.Count - 1; i >= 0; --i)
            {
                if (pred(data[i]))
                {
                    data.RemoveAt(i);
                }
            }
        }

        private static string GetKeyWords(Shader shader, ShaderCompilerData data, IEnumerable<string> ignoredKeywords)
        {
            return string.Join("|", data.shaderKeywordSet.GetShaderKeywords()
#if UNITY_2021_2_OR_NEWER
                .Select(x => x.name)
#else
                .Select(x => ShaderKeyword.GetKeywordName(shader, x))
#endif
                .Where(k => ignoredKeywords == null || !ignoredKeywords.Contains(k))
                .OrderBy(k => k));
        }
    }
}
