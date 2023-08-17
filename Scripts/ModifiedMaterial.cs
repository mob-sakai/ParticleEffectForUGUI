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
            for (var i = 0; i < s_Entries.Count; i++)
            {
                e = s_Entries[i];
                if (e.baseMat != baseMat || e.texture != texture || e.id != id) continue;
                ++e.count;
                return e.customMat;
            }

            e = new MatEntry
            {
                count = 1,
                baseMat = baseMat,
                texture = texture,
                id = id,
                customMat = new Material(baseMat)
                {
                    name = $"{baseMat.name}_{id}",
                    hideFlags = HideFlags.HideAndDontSave,
                    mainTexture = texture ? texture : null
                }
            };
            s_Entries.Add(e);
            //Debug.LogFormat(">>>> ModifiedMaterial.Add -> count = count:{0}, mat:{1}, tex:{2}, id:{3}", s_Entries.Count, baseMat, texture, id);
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
                    //Debug.LogFormat(">>>> ModifiedMaterial.Remove -> count:{0}, mat:{1}, tex:{2}, id:{3}", s_Entries.Count - 1, e.customMat, e.texture, e.id);
                    Misc.DestroyImmediate(e.customMat);
                    e.customMat = null;
                    e.baseMat = null;
                    e.texture = null;
                    s_Entries.RemoveAt(i);
                }

                break;
            }
        }

        private class MatEntry
        {
            public Material baseMat;
            public int count;
            public Material customMat;
            public int id;
            public Texture texture;
        }
    }
}
