using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIParticleExtensions
{
    internal static class MeshHelper
    {
        public static List<bool> activeMeshIndices { get; private set; }
        private static readonly List<CombineInstanceEx> s_CachedInstance;
        private static int count;

        public static void Init()
        {
            activeMeshIndices = new List<bool>();
        }

        static MeshHelper()
        {
            s_CachedInstance = new List<CombineInstanceEx>(8);
            for (var i = 0; i < 8; i++)
            {
                s_CachedInstance.Add(new CombineInstanceEx());
            }
        }

        private static CombineInstanceEx Get(int index, long hash)
        {
            if (0 < count && s_CachedInstance[count - 1].hash == hash)
                return s_CachedInstance[count - 1];

            if (s_CachedInstance.Count <= count)
            {
                var newInst = new CombineInstanceEx();
                s_CachedInstance.Add(newInst);
            }

            var inst = s_CachedInstance[count];
            inst.hash = hash;
            if (inst.index != -1) return inst;

            inst.index = index;
            count++;
            return inst;
        }

        public static Mesh GetTemporaryMesh()
        {
            return MeshPool.Rent();
        }

        public static void Push(int index, long hash, Mesh mesh, Matrix4x4 transform)
        {
            if (mesh.vertexCount <= 0)
            {
                DiscardTemporaryMesh(mesh);
                return;
            }

            Profiler.BeginSample("[UIParticle] MeshHelper > Get CombineInstanceEx");
            var inst = Get(index, hash);
            Profiler.EndSample();

            Profiler.BeginSample("[UIParticle] MeshHelper > Push To Mesh Helper");
            inst.Push(mesh, transform);
            Profiler.EndSample();

            activeMeshIndices[inst.index] = true;
        }

        public static void Clear()
        {
            count = 0;
            activeMeshIndices.Clear();
            foreach (var inst in s_CachedInstance)
            {
                inst.Clear();
            }
        }

        public static void CombineMesh(Mesh result)
        {
            if (count == 0) return;

            for (var i = 0; i < count; i++)
            {
                Profiler.BeginSample("[UIParticle] MeshHelper > Combine Mesh Internal");
                s_CachedInstance[i].Combine();
                Profiler.EndSample();
            }

            Profiler.BeginSample("[UIParticle] MeshHelper > Combine Mesh");
            var cis = CombineInstanceArrayPool.Get(s_CachedInstance, count);
            result.CombineMeshes(cis, false, true);
            cis.Clear();
            Profiler.EndSample();

            result.RecalculateBounds();
        }

        public static void DiscardTemporaryMesh(Mesh mesh)
        {
            MeshPool.Return(mesh);
        }
    }
}
