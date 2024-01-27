using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIParticleInternal
{
    internal static class CanvasExtensions
    {
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
