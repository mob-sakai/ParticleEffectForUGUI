using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIParticleExtensions
{
    internal class ModifiedMaterial
    {
        private static readonly List<MatEntry> s_Entries = new List<MatEntry>();

        public static Material Add(Material baseMat, Texture texture, int id)
        {
            MatEntry e;
            for (var i = 0; i < s_Entries.Count; ++i)
            {
                e = s_Entries[i];
                if (e.baseMat != baseMat || e.texture != texture || e.id != id) continue;
                ++e.count;
                return e.customMat;
            }

            e = new MatEntry();
            e.count = 1;
            e.baseMat = baseMat;
            e.texture = texture;
            e.id = id;
            e.customMat = new Material(baseMat);
            e.customMat.hideFlags = HideFlags.HideAndDontSave;
            if (texture)
                e.customMat.mainTexture = texture;
            s_Entries.Add(e);
            // Debug.LogFormat(">>>> ModifiedMaterial.Add -> count = {0} {1} {2} {3}", s_Entries.Count, baseMat, texture, id);
            return e.customMat;
        }

        public static void Remove(Material customMat)
        {
            if (!customMat) return;

            for (var i = 0; i < s_Entries.Count; ++i)
            {
                var e = s_Entries[i];
                if (e.customMat != customMat) continue;
                if (--e.count == 0)
                {
                    // Debug.LogFormat(">>>> ModifiedMaterial.Add -> count = {0} {1} {2} {3}", s_Entries.Count - 1, e.customMat, e.texture, e.id);
                    DestroyImmediate(e.customMat);
                    e.baseMat = null;
                    e.texture = null;
                    s_Entries.RemoveAt(i);
                }

                break;
            }
        }

        private static void DestroyImmediate(Object obj)
        {
            if (!obj) return;
            if (Application.isEditor)
                Object.DestroyImmediate(obj);
            else
                Object.Destroy(obj);
        }

        private class MatEntry
        {
            public Material baseMat;
            public Material customMat;
            public int count;
            public Texture texture;
            public int id;
        }
    }
}
