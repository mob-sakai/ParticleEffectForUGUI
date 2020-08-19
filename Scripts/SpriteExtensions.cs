using System;
using System.Reflection;
using UnityEngine;

namespace Coffee.UIExtensions
{
    internal static class SpriteExtensions
    {
#if UNITY_EDITOR
        private static Type tSpriteEditorExtension = Type.GetType("UnityEditor.Experimental.U2D.SpriteEditorExtension, UnityEditor")
                                                     ?? Type.GetType("UnityEditor.U2D.SpriteEditorExtension, UnityEditor");

        private static MethodInfo miGetActiveAtlasTexture = tSpriteEditorExtension
            .GetMethod("GetActiveAtlasTexture", BindingFlags.Static | BindingFlags.NonPublic);

        internal static Texture2D GetActualTexture(this Sprite self)
        {
            if (!self) return null;

            if (Application.isPlaying) return self.texture;
            var ret = miGetActiveAtlasTexture.Invoke(null, new[] {self}) as Texture2D;
            return ret ? ret : self.texture;
        }
#else
        internal static Texture2D GetActualTexture(this Sprite self)
        {
            return self ? self.texture : null;
        }
#endif
    }
}
