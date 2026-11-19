using System.Collections.Generic;
using Coffee.UIParticleInternal;
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
            if (particle == null || s_ActiveParticles.Contains(particle)) return;
            s_ActiveParticles.Add(particle);
        }

        public static void Unregister(UIParticle particle)
        {
            if (particle == null) return;
            s_ActiveParticles.Remove(particle);
        }

        public static void Register(UIParticleAttractor attractor)
        {
            if (attractor == null || s_ActiveAttractors.Contains(attractor)) return;
            s_ActiveAttractors.Add(attractor);
        }

        public static void Unregister(UIParticleAttractor attractor)
        {
            if (attractor == null) return;
            s_ActiveAttractors.Remove(attractor);
        }

#if UNITY_EDITOR
#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnDomainReload()
        {
            s_ActiveParticles.Clear();
            s_ActiveAttractors.Clear();
            s_UpdatedGroupIds.Clear();
            s_FrameCount = 0;
        }
#endif

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            UIExtraCallbacks.onAfterCanvasRebuild += Refresh;

            EditorApplication.playModeStateChanged += state =>
            {
                UIExtraCallbacks.onAfterCanvasRebuild -= Refresh;
                if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode)
                {
                    UIExtraCallbacks.onAfterCanvasRebuild += Refresh;
                }
            };
        }
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            UIExtraCallbacks.onAfterCanvasRebuild += Refresh;
        }
#endif

        private static void Refresh()
        {
            // Do not allow it to be called in the same frame.
            if (s_FrameCount == Time.frameCount) return;
            s_FrameCount = Time.frameCount;

            // Simulate -> Primary
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (uip == null || uip.canvas == null || !uip.isPrimary || !s_UpdatedGroupIds.Add(uip.groupId)) continue;

                uip.UpdateTransformScale();
                uip.UpdateRenderers();
            }

            // Simulate -> Others
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (uip == null || uip.canvas == null) continue;

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
                var attractor = s_ActiveAttractors[i];
                if (attractor != null) attractor.Attract();
            }
        }

        public static void GetGroupedRenderers(int groupId, int index, List<UIParticleRenderer> results)
        {
            if (results == null) return;

            results.Clear();
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                if (uip != null && uip.useMeshSharing && uip.groupId == groupId)
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
                if (uip == null) continue;
                if (!uip.useMeshSharing || uip.groupId != groupId) continue;
                if (uip.isPrimary) return uip;
                if (primary == null && uip.canSimulate) primary = uip;
            }

            return primary;
        }
    }
}
