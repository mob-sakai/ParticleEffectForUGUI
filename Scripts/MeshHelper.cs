using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIExtensions
{
    internal static class MeshHelper
    {
        private static CombineInstance[] s_CombineInstances;
        private static int s_TempIndex;
        private static int s_CurrentIndex;
        static readonly List<Color32> s_Colors = new List<Color32>();
        private static int s_RefCount;
        private static Matrix4x4 s_Transform;
        public static uint activeMeshIndices { get; private set; }

        public static void Register()
        {
            if (0 < s_RefCount++) return;
            s_CombineInstances = new CombineInstance[8];
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

        public static Mesh GetTemporaryMesh(int index)
        {
            if (s_CombineInstances.Length <= s_TempIndex) s_TempIndex = s_CombineInstances.Length - 1;
            s_CurrentIndex = index;
            activeMeshIndices += (uint)(1 << s_CurrentIndex);
            s_CombineInstances[s_TempIndex].transform = s_Transform;
            return s_CombineInstances[s_TempIndex++].mesh;
        }

        public static void DiscardTemporaryMesh()
        {
            if (s_TempIndex == 0) return;
            s_TempIndex--;
            activeMeshIndices -= (uint)(1 << s_CurrentIndex);
        }

        public static void SetTransform(Matrix4x4 transform)
        {
            s_Transform = transform;
        }

        public static void Clear()
        {
            if (s_CombineInstances == null) return;
            s_CurrentIndex = 0;
            activeMeshIndices = 0;
            s_TempIndex = 0;
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

        public static void CombineMesh(Mesh result)
        {
            if (!result || s_TempIndex == 0) return;

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
