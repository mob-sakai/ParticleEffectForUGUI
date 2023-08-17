using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Coffee.UIExtensions.Demo
{
    public class CFX_Demo_With_UIParticle : MonoBehaviour
    {
        private MonoBehaviour _demo;
        private Toggle _spawnOnUI;
        private UIParticle _uiParticle;

        // Start is called before the first frame update
        private void Start()
        {
            _uiParticle = GetComponentInChildren<UIParticle>();
            _spawnOnUI = GetComponentInChildren<Toggle>();
            _demo = FindObjectOfType("CFX_Demo_New") as MonoBehaviour
                    ?? FindObjectOfType("WFX_Demo_New") as MonoBehaviour;

            SetCanvasWidth(800);
            SetCanvasRenderOverlay(true);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_spawnOnUI.isOn || !_demo || !Input.GetMouseButtonDown(0)) return;

            foreach (Transform child in _uiParticle.transform)
            {
                Destroy(child.gameObject);
            }

            var particle = _demo.GetType()
                .GetMethod("spawnParticle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?.Invoke(_demo, Array.Empty<object>()) as GameObject;
            if (!particle) return;

            particle.transform.localScale = Vector3.one;
            _uiParticle.SetParticleSystemInstance(particle);
        }

        private static Object FindObjectOfType(string typeName)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.Name == typeName);

            return type == null ? null : FindObjectOfType(type);
        }

        public void SetCanvasWidth(int width)
        {
            var scaler = GetComponentInParent<CanvasScaler>();
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0;
            var resolution = scaler.referenceResolution;
            resolution.x = width;
            scaler.referenceResolution = resolution;
        }

        public void SetCanvasRenderOverlay(bool enable)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (enable)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else
            {
                canvas.worldCamera = Camera.main;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.planeDistance = 5;
            }
        }

        public void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}
