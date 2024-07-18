using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Coffee.UIExtensions.Demo
{
    public class UIParticle_Demo : MonoBehaviour
    {
        [FormerlySerializedAs("root")]
        [SerializeField]
        private Canvas m_RootCanvas;

        private int _height;
        private int _score;
        private int _width;

        private void Start()
        {
            _width = Screen.width;
            _height = Screen.height;
        }

        public void ResizeScreen()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.LinuxPlayer:
                    if (Screen.width == _width && Screen.height == _height)
                    {
                        Screen.SetResolution(_height, _width, Screen.fullScreen);
                    }
                    else if (Screen.width == _height && Screen.height == _width)
                    {
                        Screen.SetResolution(Mathf.Min(_width, _height), Mathf.Min(_width, _height), Screen.fullScreen);
                    }
                    else
                    {
                        Screen.SetResolution(_width, _height, Screen.fullScreen);
                    }

                    break;
            }
        }

        public void FullScreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        public void EnableAnimations(bool flag)
        {
#if UNITY_2023_1_OR_NEWER
            foreach (var animator in FindObjectsByType<Animator>(FindObjectsInactive.Include, FindObjectsSortMode.None))
#else
            foreach (var animator in FindObjectsOfType<Animator>())
#endif
            {
                animator.enabled = flag;
            }
        }

        public void UIParticle_MeshSharing(bool flag)
        {
            foreach (var uip in m_RootCanvas.GetComponentsInChildren<UIParticle>(true))
            {
                uip.meshSharing = flag
                    ? UIParticle.MeshSharing.Auto
                    : UIParticle.MeshSharing.None;
            }
        }

        public void UIParticle_RandomGroup(bool flag)
        {
            foreach (var uip in m_RootCanvas.GetComponentsInChildren<UIParticle>(true))
            {
                uip.groupMaxId = flag
                    ? 4
                    : 0;
            }
        }

        public void UIParticle_Scale(float scale)
        {
#if UNITY_2023_1_OR_NEWER
            foreach (var uip in FindObjectsByType<UIParticle>(FindObjectsInactive.Include, FindObjectsSortMode.None))
#else
            foreach (var uip in FindObjectsOfType<UIParticle>())
#endif
            {
                uip.scale = scale;
            }
        }

        public void ParticleSystem_WorldSpaseSimulation(bool flag)
        {
#if UNITY_2023_1_OR_NEWER
            foreach (var p in FindObjectsByType<ParticleSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None))
#else
            foreach (var p in FindObjectsOfType<ParticleSystem>())
#endif
            {
                var main = p.main;
                main.simulationSpace = flag
                    ? ParticleSystemSimulationSpace.World
                    : ParticleSystemSimulationSpace.Local;
            }
        }

        public void ParticleSystem_WorldSpaseSimulation(ParticleSystem ps)
        {
            foreach (var p in ps.GetComponentsInChildren<ParticleSystem>())
            {
                var main = p.main;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                p.Clear();
            }
        }

        public void ParticleSystem_LocalSpaseSimulation(ParticleSystem ps)
        {
            foreach (var p in ps.GetComponentsInChildren<ParticleSystem>())
            {
                var main = p.main;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                p.Clear();
            }
        }

        public void ParticleSystem_Emit(ParticleSystem ps)
        {
            ps.Emit(5);
        }

        public void ParticleSystem_SetScale(float scale)
        {
#if UNITY_2023_1_OR_NEWER
            foreach (var ps in FindObjectsByType<ParticleSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None))
#else
            foreach (var ps in FindObjectsOfType<ParticleSystem>())
#endif
            {
                ps.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        public void UIParticleAttractor_Linear(UIParticleAttractor attractor)
        {
            attractor.movement = UIParticleAttractor.Movement.Linear;
        }

        public void UIParticleAttractor_Smooth(UIParticleAttractor attractor)
        {
            attractor.movement = UIParticleAttractor.Movement.Smooth;
        }

        public void UIParticleAttractor_Sphere(UIParticleAttractor attractor)
        {
            attractor.movement = UIParticleAttractor.Movement.Sphere;
        }

        public void UIParticleAttractor_OnAttract(Text scoreText)
        {
            _score++;
            scoreText.text = _score.ToString();
            scoreText.GetComponent<Animator>().Play(0);
        }

        public void Canvas_WorldSpace(bool flag)
        {
            if (!flag) return;

            m_RootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            m_RootCanvas.renderMode = RenderMode.WorldSpace;
            m_RootCanvas.transform.rotation = Quaternion.Euler(new Vector3(0, 10, 0));
        }

        public void Canvas_CameraSpace(bool flag)
        {
            if (!flag) return;

            m_RootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        }

        public void Canvas_Overlay(bool flag)
        {
            if (!flag) return;

            m_RootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }
}
