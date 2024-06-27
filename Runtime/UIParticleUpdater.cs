using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
    internal static class UIParticleUpdater
    {
        private static readonly List<UIParticle> s_ActiveParticles = new List<UIParticle>();
        private static readonly List<UIParticleAttractor> s_ActiveAttractors = new List<UIParticleAttractor>();
        private static readonly HashSet<int> s_UpdatedGroupIds = new HashSet<int>();
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

            // Simulate -> Primary
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (!uip || !uip.canvas || !uip.isPrimary || !s_UpdatedGroupIds.Add(uip.groupId)) continue;

                uip.UpdateTransformScale();
                uip.UpdateRenderers();
            }

            // Simulate -> Others
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (!uip || !uip.canvas) continue;

                uip.UpdateTransformScale();

                if (!uip.useMeshSharing)
                {
                    uip.UpdateRenderers();
                }
                else if (s_UpdatedGroupIds.Add(uip.groupId))
                {
                    uip.UpdateRenderers();
                }
            }

            s_UpdatedGroupIds.Clear();

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
