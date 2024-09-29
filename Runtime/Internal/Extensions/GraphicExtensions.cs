using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace Coffee.UIParticleInternal
{
    /// <summary>
    /// Extension methods for Graphic class.
    /// </summary>
    internal static class GraphicExtensions
    {
        private static readonly Vector3[] s_WorldCorners = new Vector3[4];
        private static readonly Bounds s_ScreenBounds = new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 1, 1));

        /// <summary>
        /// Check if a Graphic component is currently in the screen view.
        /// </summary>
        public static void GetMaterialsForRendering(this Graphic self, List<Material> result)
        {
            result.Clear();
            if (!self) return;

            var cr = self.canvasRenderer;
            var count = cr.materialCount;
            var popCount = cr.popMaterialCount;

            if (result.Capacity < count + popCount)
            {
                result.Capacity = count + popCount;
            }

            for (var i = 0; i < count; i++)
            {
                result.Add(cr.GetMaterial(i));
            }

            for (var i = 0; i < popCount; i++)
            {
                result.Add(cr.GetPopMaterial(i));
            }
        }

        /// <summary>
        /// Check if a Graphic component is currently in the screen view.
        /// </summary>
        public static bool IsInScreen(this Graphic self)
        {
            if (!self || !self.canvas) return false;

            if (FrameCache.TryGet(self, nameof(IsInScreen), out bool result))
            {
                return result;
            }

            Profiler.BeginSample("(COF)[GraphicExt] IsInScreen");
            var cam = self.canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? self.canvas.worldCamera
                : null;
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            self.rectTransform.GetWorldCorners(s_WorldCorners);
            var screenSize = GetScreenSize();
            for (var i = 0; i < 4; i++)
            {
                if (cam)
                {
                    s_WorldCorners[i] = cam.WorldToViewportPoint(s_WorldCorners[i]);
                }
                else
                {
                    s_WorldCorners[i] = RectTransformUtility.WorldToScreenPoint(null, s_WorldCorners[i]);
                    s_WorldCorners[i].x /= screenSize.x;
                    s_WorldCorners[i].y /= screenSize.y;
                }

                s_WorldCorners[i].z = 0;
                min = Vector3.Min(s_WorldCorners[i], min);
                max = Vector3.Max(s_WorldCorners[i], max);
            }

            var bounds = new Bounds(min, Vector3.zero);
            bounds.Encapsulate(max);
            result = bounds.Intersects(s_ScreenBounds);
            FrameCache.Set(self, nameof(IsInScreen), result);
            Profiler.EndSample();

            return result;
        }

        /// <summary>
        /// Get the actual main texture of a Graphic component.
        /// </summary>
        public static Texture GetActualMainTexture(this Graphic self)
        {
            var image = self as Image;
            if (image == null) return self.mainTexture;

            var sprite = image.overrideSprite;
            return sprite ? sprite.GetActualTexture() : self.mainTexture;
        }

        private static Vector2Int GetScreenSize()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !Camera.current)
            {
                var res = UnityStats.screenRes.Split('x');
                return new Vector2Int(int.Parse(res[0]), int.Parse(res[1]));
            }
#endif
            return new Vector2Int(Screen.width, Screen.height);
        }

        public static float GetParentGroupAlpha(this Graphic self)
        {
            var alpha = self.canvasRenderer.GetAlpha();
            if (Mathf.Approximately(alpha, 0)) return 1;

            var inheritedAlpha = self.canvasRenderer.GetInheritedAlpha();
            return Mathf.Clamp01(inheritedAlpha / alpha);
        }
    }
}
