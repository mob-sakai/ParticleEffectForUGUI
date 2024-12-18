using System;
using UnityEngine;
using UnityEngine.U2D;
#if UNITY_EDITOR
using System.Reflection;
#endif

namespace Coffee.UIParticleInternal
{
    /// <summary>
    /// Extension methods for Sprite class.
    /// </summary>
    internal static class SpriteExtensions
    {
#if UNITY_EDITOR
        private static readonly Type s_SpriteEditorExtensionType =
            Type.GetType("UnityEditor.Experimental.U2D.SpriteEditorExtension, UnityEditor")
            ?? Type.GetType("UnityEditor.U2D.SpriteEditorExtension, UnityEditor");

        private static readonly Func<Sprite, Texture2D> s_GetActiveAtlasTextureMethod =
            (Func<Sprite, Texture2D>)Delegate.CreateDelegate(typeof(Func<Sprite, Texture2D>),
                s_SpriteEditorExtensionType
                    .GetMethod("GetActiveAtlasTexture", BindingFlags.Static | BindingFlags.NonPublic));

        private static readonly Func<Sprite, SpriteAtlas> s_GetActiveAtlasMethod =
            (Func<Sprite, SpriteAtlas>)Delegate.CreateDelegate(typeof(Func<Sprite, SpriteAtlas>),
                s_SpriteEditorExtensionType
                    .GetMethod("GetActiveAtlas", BindingFlags.Static | BindingFlags.NonPublic));

        /// <summary>
        /// Get the actual texture of a sprite in play mode or edit mode.
        /// </summary>
        public static Texture2D GetActualTexture(this Sprite self)
        {
            if (!self) return null;

            var ret = s_GetActiveAtlasTextureMethod(self);
            return ret ? ret : self.texture;
        }

        /// <summary>
        /// Get the active sprite atlas of a sprite in play mode or edit mode.
        /// </summary>
        public static SpriteAtlas GetActiveAtlas(this Sprite self)
        {
            if (!self) return null;

            return s_GetActiveAtlasMethod(self);
        }
#else
        /// <summary>
        /// Get the actual texture of a sprite in play mode.
        /// </summary>
        internal static Texture2D GetActualTexture(this Sprite self)
        {
            return self ? self.texture : null;
        }
#endif
    }
}
