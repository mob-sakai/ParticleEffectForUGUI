using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIExtensions
{
    internal static class UIParticleUpdater
    {
        static readonly List<UIParticle> s_ActiveParticles = new List<UIParticle>();
        static readonly List<UIParticleAttractor> s_ActiveAttractors = new List<UIParticleAttractor>();
        static readonly HashSet<int> s_UpdatedGroupIds = new HashSet<int>();
        private static int frameCount = 0;

        public static int uiParticleCount
        {
            get
            {
                return s_ActiveParticles.Count;
            }
        }

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
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            Canvas.willRenderCanvases -= Refresh;
            Canvas.willRenderCanvases += Refresh;
        }

        private static void Refresh()
        {
            // Do not allow it to be called in the same frame.
            if (frameCount == Time.frameCount) return;
            frameCount = Time.frameCount;

            // Simulate -> Primary
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (!uip || !uip.canvas || !uip.isPrimary || s_UpdatedGroupIds.Contains(uip.groupId)) continue;

                s_UpdatedGroupIds.Add(uip.groupId);
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
                else if (!s_UpdatedGroupIds.Contains(uip.groupId))
                {
                    s_UpdatedGroupIds.Add(uip.groupId);
                    uip.UpdateRenderers();
                }
            }

            s_UpdatedGroupIds.Clear();

            // Attract
            for (var i = 0; i < s_ActiveAttractors.Count; i++)
            {
                s_ActiveAttractors[i].Attract();
            }

            // UpdateParticleCount.
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (!uip || !uip.canvas) continue;

                uip.UpdateParticleCount();
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
