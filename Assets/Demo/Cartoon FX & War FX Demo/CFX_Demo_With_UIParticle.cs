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
        private string _demoType;
        private Toggle _spawnOnUI;
        private UIParticle _uiParticle;

        // Start is called before the first frame update
        private void Start()
        {
            _uiParticle = GetComponentInChildren<UIParticle>();
            _spawnOnUI = GetComponentInChildren<Toggle>();
            _demo = FindObjectOfType("CFX_Demo_New") as MonoBehaviour
                    ?? FindObjectOfType("WFX_Demo_New") as MonoBehaviour
                    ?? FindObjectOfType("CFXR_Demo") as MonoBehaviour;
            _demoType = _demo?.GetType().Name;

            SetCanvasWidth(800);
            SetCanvasRenderOverlay(true);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_spawnOnUI.isOn || !_demo || !Input.GetMouseButtonDown(0)) return;

            if (_demoType == "CFX_Demo_New" || _demoType == "WFX_Demo_New")
            {
                SpawnParticleCFX();
            }
            else if (_demoType == "CFXR_Demo")
            {
                SpawnParticleCFXR();
            }
        }

        private void SpawnParticleCFXR()
        {
            var particle = _demo.GetType()
                .GetField("currentEffect", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?.GetValue(_demo) as GameObject;
            if (!particle) return;

            var instance = Instantiate(particle);
            foreach (var c in instance.GetComponentsInChildren<MonoBehaviour>())
            {
                if (c.GetType().Name == "CFXR_Effect")
                {
                    c.enabled = false;
                }
            }

            _uiParticle.SetParticleSystemInstance(instance, true);
        }

        private void SpawnParticleCFX()
        {
            var particle = _demo.GetType()
                .GetMethod("spawnParticle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?.Invoke(_demo, Array.Empty<object>()) as GameObject;
            if (!particle) return;

            particle.transform.localScale = Vector3.one;
            _uiParticle.SetParticleSystemInstance(particle, true);
        }

        private static Object FindObjectOfType(string typeName)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.Name == typeName);

#if UNITY_2023_2_OR_NEWER
            return type == null ? null : FindFirstObjectByType(type);
#else
            return type == null ? null : FindObjectOfType(type);
#endif
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
