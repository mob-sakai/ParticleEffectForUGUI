using System.Collections.Generic;
using Coffee.UIParticleExtensions;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
    internal static class UIParticleUpdater
    {
        private static readonly List<UIParticle> s_ActiveParticles = new List<UIParticle>();
        private static readonly List<UIParticleAttractor> s_ActiveAttractors = new List<UIParticleAttractor>();
        private static readonly Dictionary<int, Vector3> s_GroupScales = new Dictionary<int, Vector3>();
        private static int s_FrameCount;

        public static int uiParticleCount => s_ActiveParticles.Count;

        public static void Register(UIParticle particle)
        {
            if (!particle) return;
            s_ActiveParticles.Add(particle);
        }

        public static void Unregister(UIParticle particle)
        {
            if (!particle) return;
            s_ActiveParticles.Remove(particle);
        }

        public static void Register(UIParticleAttractor attractor)
        {
            if (!attractor) return;
            s_ActiveAttractors.Add(attractor);
        }

        public static void Unregister(UIParticleAttractor attractor)
        {
            if (!attractor) return;
            s_ActiveAttractors.Remove(attractor);
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void InitializeOnLoad()
        {
            Canvas.willRenderCanvases -= Refresh;
            Canvas.willRenderCanvases += Refresh;
        }

        private static void Refresh()
        {
            // Do not allow it to be called in the same frame.
            if (s_FrameCount == Time.frameCount) return;
            s_FrameCount = Time.frameCount;
            s_GroupScales.Clear();

            // Simulate -> Primary
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                // Process only primary UIParticles.
                var uip = s_ActiveParticles[i];
                if (!uip || !uip.canvas || !uip.isPrimary || s_GroupScales.ContainsKey(uip.groupId)) continue;

                s_GroupScales.Add(uip.groupId, uip.transform.parent.lossyScale);
                uip.UpdateTransformScale(Vector3.one);
                uip.UpdateRenderers();
            }

            // Simulate -> Others
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (!uip || !uip.canvas) continue;

                // Case 1: Not using mesh sharing.
                if (!uip.useMeshSharing)
                {
                    uip.UpdateTransformScale(Vector3.one);
                    uip.UpdateRenderers();
                }
                // Case 2: Using mesh sharing as primary.
                else if (!s_GroupScales.TryGetValue(uip.groupId, out var groupScale))
                {
                    s_GroupScales.Add(uip.groupId, uip.transform.parent.lossyScale);
                    uip.UpdateTransformScale(Vector3.one);
                    uip.UpdateRenderers();
                }
                // Case 3: Using mesh sharing as replica. (Only scaling)
                else
                {
                    var parentScale = uip.transform.parent.lossyScale;
                    var ratio = parentScale.IsVisible()
                        ? groupScale.GetScaled(parentScale.Inverse())
                        : Vector3.one;
                    ratio = Vector3.one;
                    uip.UpdateTransformScale(ratio);
                }
            }

            // Attract
            for (var i = 0; i < s_ActiveAttractors.Count; i++)
            {
                s_ActiveAttractors[i].Attract();
            }
        }

        public static void GetGroupedRenderers(int groupId, int index, List<UIParticleRenderer> results)
        {
            results.Clear();
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (uip.useMeshSharing && uip.groupId == groupId)
                {
                    results.Add(uip.GetRenderer(index));
                }
            }
        }

        internal static UIParticle GetPrimary(int groupId)
        {
            UIParticle primary = null;
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (!uip.useMeshSharing || uip.groupId != groupId) continue;
                if (uip.isPrimary) return uip;
                if (!primary && uip.canSimulate) primary = uip;
            }

            return primary;
        }
    }
}
