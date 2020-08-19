using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIExtensions
{
    internal static class MeshHelper
    {
        private static CombineInstance[] s_CombineInstances;
        private static int s_CurrentIndex;
        static readonly List<Color32> s_Colors = new List<Color32>();
        private static int s_RefCount;

        public static void Register()
        {
            if (0 < s_RefCount++) return;
            s_CombineInstances = new CombineInstance[2];
        }

        public static void Unregister()
        {
            s_RefCount--;

            if (0 < s_RefCount || s_CombineInstances == null) return;

            for (var i = 0; i < s_CombineInstances.Length; i++)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Object.DestroyImmediate(s_CombineInstances[i].mesh);
                else
#endif
                {
                    Object.Destroy(s_CombineInstances[i].mesh);
                }
            }

            s_CombineInstances = null;
        }

        public static Mesh GetTemporaryMesh()
        {
            return s_CombineInstances[s_CurrentIndex++].mesh;
        }

        public static void Clear()
        {
            s_CurrentIndex = 0;
            for (var i = 0; i < s_CombineInstances.Length; i++)
            {
                if (!s_CombineInstances[i].mesh)
                {
                    var mesh = new Mesh();
                    mesh.MarkDynamic();
                    s_CombineInstances[i].mesh = mesh;
                }
                else
                {
                    s_CombineInstances[i].mesh.Clear(false);
                }
            }
        }

        public static void CombineMesh(Mesh result, Matrix4x4 transform)
        {
            if (!result || s_CurrentIndex == 0) return;

            for (var i = 0; i < 2; i++)
                s_CombineInstances[i].transform = transform;

            result.CombineMeshes(s_CombineInstances, false, true);
            result.RecalculateBounds();
        }

        public static void ModifyColorSpaceToLinear(this Mesh self)
        {
            self.GetColors(s_Colors);

            for (var i = 0; i < s_Colors.Count; i++)
                s_Colors[i] = ((Color) s_Colors[i]).gamma;

            self.SetColors(s_Colors);
            s_Colors.Clear();
        }
    }
}
