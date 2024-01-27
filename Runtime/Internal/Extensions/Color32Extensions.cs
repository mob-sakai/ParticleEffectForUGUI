using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIParticleInternal
{
    internal static class Color32Extensions
    {
        private static readonly List<Color32> s_Colors = new List<Color32>();
        private static byte[] s_LinearToGammaLut;
        private static byte[] s_GammaToLinearLut;

        public static byte LinearToGamma(this byte self)
        {
            if (s_LinearToGammaLut == null)
            {
                s_LinearToGammaLut = new byte[256];
                for (var i = 0; i < 256; i++)
                {
                    s_LinearToGammaLut[i] = (byte)(Mathf.LinearToGammaSpace(i / 255f) * 255f);
                }
            }

            return s_LinearToGammaLut[self];
        }

        public static byte GammaToLinear(this byte self)
        {
            if (s_GammaToLinearLut == null)
            {
                s_GammaToLinearLut = new byte[256];
                for (var i = 0; i < 256; i++)
                {
                    s_GammaToLinearLut[i] = (byte)(Mathf.GammaToLinearSpace(i / 255f) * 255f);
                }
            }

            return s_GammaToLinearLut[self];
        }

        public static void LinearToGamma(this Mesh self)
        {
            Profiler.BeginSample("(COF)[ColorExt] LinearToGamma (Mesh)");
            self.GetColors(s_Colors);
            var count = s_Colors.Count;
            for (var i = 0; i < count; i++)
            {
                var c = s_Colors[i];
                c.r = c.r.LinearToGamma();
                c.g = c.g.LinearToGamma();
                c.b = c.b.LinearToGamma();
                s_Colors[i] = c;
            }

            self.SetColors(s_Colors);
            Profiler.EndSample();
        }

        public static void GammaToLinear(this Mesh self)
        {
            Profiler.BeginSample("(COF)[ColorExt] GammaToLinear (Mesh)");
            self.GetColors(s_Colors);
            var count = s_Colors.Count;
            for (var i = 0; i < count; i++)
            {
                var c = s_Colors[i];
                c.r = c.r.GammaToLinear();
                c.g = c.g.GammaToLinear();
                c.b = c.b.GammaToLinear();
                s_Colors[i] = c;
            }

            self.SetColors(s_Colors);
            Profiler.EndSample();
        }
    }
}
