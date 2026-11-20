using System.Collections;
using System.Linq;
using Coffee.UIParticle.Issue354.Scene;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UIParticleComponent = Coffee.UIExtensions.UIParticle;

namespace Coffee.UIParticle.Issue354.Tests
{
    public class EmissionOverDistanceTests
    {
        [UnityTest]
        public IEnumerator PointerFollowerConvertsScreenPositionForOverlayCanvas()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var targetObject = new GameObject("Target", typeof(RectTransform), typeof(CanvasRenderer));
            try
            {
                var canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                targetObject.transform.SetParent(canvas.transform, false);
                var follower = targetObject.AddComponent<UIParticlePointerFollower>();
                SetCanvas(follower, canvas);
                yield return null;

                var screenPosition = new Vector2(Screen.width * 0.3f, Screen.height * 0.7f);
                Assert.IsTrue(follower.SetScreenPosition(screenPosition));
                var actual = RectTransformUtility.WorldToScreenPoint(null, targetObject.transform.position);
                Assert.That(actual.x, Is.EqualTo(screenPosition.x).Within(1f));
                Assert.That(actual.y, Is.EqualTo(screenPosition.y).Within(1f));
            }
            finally
            {
                Object.Destroy(canvasObject);
            }
        }

        [UnityTest]
        public IEnumerator PointerFollowerConvertsScreenPositionForCameraCanvas()
        {
            var cameraObject = new GameObject("Camera");
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var targetObject = new GameObject("Target", typeof(RectTransform), typeof(CanvasRenderer));
            try
            {
                var camera = cameraObject.AddComponent<Camera>();
                camera.transform.position = Vector3.back * 10;
                camera.orthographic = true;
                camera.orthographicSize = 5;

                var canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 10;
                targetObject.transform.SetParent(canvas.transform, false);
                var follower = targetObject.AddComponent<UIParticlePointerFollower>();
                SetCanvas(follower, canvas);
                yield return null;

                var screenPosition = new Vector2(Screen.width * 0.65f, Screen.height * 0.4f);
                Assert.IsTrue(follower.SetScreenPosition(screenPosition));
                var actual = RectTransformUtility.WorldToScreenPoint(camera, targetObject.transform.position);
                Assert.That(actual.x, Is.EqualTo(screenPosition.x).Within(1f));
                Assert.That(actual.y, Is.EqualTo(screenPosition.y).Within(1f));
            }
            finally
            {
                Object.Destroy(cameraObject);
                Object.Destroy(canvasObject);
            }
        }

        [UnityTest]
        public IEnumerator RateOverDistanceAndLocalTrailsWorkWhenMovingUIParticle()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var targetObject = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
            try
            {
                var canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                targetObject.transform.SetParent(canvas.transform, false);
                var particleSystem = targetObject.AddComponent<ParticleSystem>();
                var main = particleSystem.main;
                main.playOnAwake = false;
                main.loop = true;
                main.startLifetime = 10;
                main.startSpeed = 2;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                var emission = particleSystem.emission;
                emission.enabled = true;
                emission.rateOverTime = 0;
                emission.rateOverDistance = 1;
                var shape = particleSystem.shape;
                shape.enabled = false;
                var trails = particleSystem.trails;
                trails.enabled = true;
                trails.worldSpace = false;
                trails.minVertexDistance = 0;
                trails.lifetime = 10;
                var particleRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                particleRenderer.sharedMaterials =
                    new[] { particleRenderer.sharedMaterial, particleRenderer.sharedMaterial };
                var uiParticle = targetObject.AddComponent<UIParticleComponent>();
                uiParticle.scale = 2;
                uiParticle.RefreshParticles();
                yield return null;

                uiParticle.Play();
                for (var i = 0; i < 6; i++)
                {
                    targetObject.transform.position += Vector3.right * 20;
                    yield return null;
                    yield return new WaitForEndOfFrame();
                }

                Assert.Greater(particleSystem.particleCount, 0,
                    "Moving the UIParticle transform should produce distance-based particles.");

                yield return new WaitForEndOfFrame();
                var meshes = uiParticle.GetComponentsInChildren<CanvasRenderer>()
                    .Where(renderer => renderer.gameObject != uiParticle.gameObject
                                       && renderer.GetMesh() != null
                                       && renderer.GetMesh().vertexCount > 0)
                    .ToArray();
                Assert.AreEqual(2, meshes.Length);
            }
            finally
            {
                Object.Destroy(canvasObject);
            }
        }

        private static void SetCanvas(UIParticlePointerFollower follower, Canvas canvas)
        {
            var field = typeof(UIParticlePointerFollower).GetField(
                "canvas", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            field.SetValue(follower, canvas);
        }
    }
}
