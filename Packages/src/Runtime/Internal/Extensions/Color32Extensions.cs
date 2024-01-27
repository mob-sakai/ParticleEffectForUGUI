using UnityEngine;

namespace Coffee.UIParticleInternal
{
    internal static class Color32Extensions
    {
        private static byte[] s_LinearToGammaLut;

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
    }
}
