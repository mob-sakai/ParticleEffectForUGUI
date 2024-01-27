#if UNITY_2021_3_0 || UNITY_2021_3_1 || UNITY_2021_3_2 || UNITY_2021_3_3 || UNITY_2021_3_4 || UNITY_2021_3_5 || UNITY_2021_3_6 || UNITY_2021_3_7 || UNITY_2021_3_8 || UNITY_2021_3_9
#elif UNITY_2021_3_10 || UNITY_2021_3_11 || UNITY_2021_3_12 || UNITY_2021_3_13 || UNITY_2021_3_14 || UNITY_2021_3_15 || UNITY_2021_3_16 || UNITY_2021_3_17 || UNITY_2021_3_18 || UNITY_2021_3_19
#elif UNITY_2021_3_20 || UNITY_2021_3_21 || UNITY_2021_3_22 || UNITY_2021_3_23 || UNITY_2021_3_24 || UNITY_2021_3_25 || UNITY_2021_3_26 || UNITY_2021_3_27 || UNITY_2021_3_28 || UNITY_2021_3_29
#elif UNITY_2021_3_30 || UNITY_2021_3_31 || UNITY_2021_3_32 || UNITY_2021_3_33
#elif UNITY_2022_2_0 || UNITY_2022_2_1 || UNITY_2022_2_2 || UNITY_2022_2_3 || UNITY_2022_2_4 || UNITY_2022_2_5 || UNITY_2022_2_6 || UNITY_2022_2_7 || UNITY_2022_2_8 || UNITY_2022_2_9
#elif UNITY_2022_2_10 || UNITY_2022_2_11 || UNITY_2022_2_12 || UNITY_2022_2_13 || UNITY_2022_2_14
#elif UNITY_2021_3 || UNITY_2022_2 || UNITY_2022_3 || UNITY_2023_2_OR_NEWER
#define CANVAS_SUPPORT_ALWAYS_GAMMA
#endif

using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_MODULE_VR
using UnityEngine.XR;
#endif

namespace Coffee.UIParticleInternal
{
    internal static class CanvasExtensions
    {
        public static bool ShouldGammaToLinearInShader(this Canvas canvas)
        {
            return QualitySettings.activeColorSpace == ColorSpace.Linear &&
#if CANVAS_SUPPORT_ALWAYS_GAMMA
                   canvas.vertexColorAlwaysGammaSpace;
#else
                   false;
#endif
        }

        public static bool ShouldGammaToLinearInMesh(this Canvas canvas)
        {
            return QualitySettings.activeColorSpace == ColorSpace.Linear &&
#if CANVAS_SUPPORT_ALWAYS_GAMMA
                   !canvas.vertexColorAlwaysGammaSpace;
#else
                   true;
#endif
        }

        public static bool IsStereoCanvas(this Canvas canvas)
        {
#if UNITY_MODULE_VR
            if (FrameCache.TryGet<bool>(canvas, nameof(IsStereoCanvas), out var stereo)) return stereo;

            stereo =
                canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera != null
                && XRSettings.enabled && !string.IsNullOrEmpty(XRSettings.loadedDeviceName);
            FrameCache.Set(canvas, nameof(IsStereoCanvas), stereo);
            return stereo;
#else
            return false;
#endif
        }

        /// <summary>
        /// Gets the view-projection matrix for a Canvas.
        /// </summary>
        public static void GetViewProjectionMatrix(this Canvas canvas, out Matrix4x4 vpMatrix)
        {
            canvas.GetViewProjectionMatrix(Camera.MonoOrStereoscopicEye.Mono, out vpMatrix);
        }

        /// <summary>
        /// Gets the view-projection matrix for a Canvas.
        /// </summary>
        public static void GetViewProjectionMatrix(this Canvas canvas, Camera.MonoOrStereoscopicEye eye,
            out Matrix4x4 vpMatrix)
        {
            if (FrameCache.TryGet(canvas, nameof(GetViewProjectionMatrix), out vpMatrix)) return;

            canvas.GetViewProjectionMatrix(eye, out var viewMatrix, out var projectionMatrix);
            vpMatrix = viewMatrix * projectionMatrix;
            FrameCache.Set(canvas, nameof(GetViewProjectionMatrix), vpMatrix);
        }

        /// <summary>
        /// Gets the view and projection matrices for a Canvas.
        /// </summary>
        public static void GetViewProjectionMatrix(this Canvas canvas, out Matrix4x4 vMatrix, out Matrix4x4 pMatrix)
        {
            canvas.GetViewProjectionMatrix(Camera.MonoOrStereoscopicEye.Mono, out vMatrix, out pMatrix);
        }

        /// <summary>
        /// Gets the view and projection matrices for a Canvas.
        /// </summary>
        public static void GetViewProjectionMatrix(this Canvas canvas, Camera.MonoOrStereoscopicEye eye,
            out Matrix4x4 vMatrix, out Matrix4x4 pMatrix)
        {
            if (FrameCache.TryGet(canvas, "GetViewMatrix", (int)eye, out vMatrix) &&
                FrameCache.TryGet(canvas, "GetProjectionMatrix", (int)eye, out pMatrix))
            {
                return;
            }

            // Get view and projection matrices.
            Profiler.BeginSample("(COF)[CanvasExt] GetViewProjectionMatrix");
            var rootCanvas = canvas.rootCanvas;
            var cam = rootCanvas.worldCamera;
            if (rootCanvas && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay && cam)
            {
                if (eye == Camera.MonoOrStereoscopicEye.Mono)
                {
                    vMatrix = cam.worldToCameraMatrix;
                    pMatrix = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
                }
                else
                {
                    pMatrix = cam.GetStereoProjectionMatrix((Camera.StereoscopicEye)eye);
                    vMatrix = cam.GetStereoViewMatrix((Camera.StereoscopicEye)eye);
                    pMatrix = GL.GetGPUProjectionMatrix(pMatrix, false);
                }
            }
            else
            {
                var pos = rootCanvas.transform.position;
                vMatrix = Matrix4x4.TRS(
                    new Vector3(-pos.x, -pos.y, -1000),
                    Quaternion.identity,
                    new Vector3(1, 1, -1f));
                pMatrix = Matrix4x4.TRS(
                    new Vector3(0, 0, -1),
                    Quaternion.identity,
                    new Vector3(1 / pos.x, 1 / pos.y, -2 / 10000f));
            }

            FrameCache.Set(canvas, "GetViewMatrix", (int)eye, vMatrix);
            FrameCache.Set(canvas, "GetProjectionMatrix", (int)eye, pMatrix);

            Profiler.EndSample();
        }
    }
}
