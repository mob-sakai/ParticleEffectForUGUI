using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIExtensions
{
    internal static class UIParticleUpdater
    {
        static readonly List<UIParticle> s_ActiveParticles = new List<UIParticle>();
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

            Profiler.BeginSample("[UIParticle] Refresh");
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                var uip = s_ActiveParticles[i];
                try
                {
                    uip.UpdateTransformScale();
                    uip.UpdateRenderers();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            Profiler.EndSample();
        }
    }
}
