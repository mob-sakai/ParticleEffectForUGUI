using UnityEngine;

namespace Coffee.UIExtensions.Demo
{
    public class UIParticle_PerformanceDemo : MonoBehaviour
    {
        public GameObject copyOrigin;
        public int copyCount;
        public Canvas root;

        private void Start()
        {
            Application.targetFrameRate = 60;

            if (copyOrigin)
            {
                copyOrigin.SetActive(false);

                var parent = copyOrigin.transform.parent;
                for (var i = 0; i < copyCount; i++)
                {
                    var go = GameObject.Instantiate(copyOrigin, parent, false);
                    go.name = string.Format("{0} {1}", copyOrigin.name, i + 1);
                    go.hideFlags = HideFlags.DontSave;

                    go.SetActive(true);
                }
            }
        }

        public void ChangeScreenSize()
        {
            if (Screen.width == 400 && Screen.height == 720)
                Screen.SetResolution(720, 720, false);
            else if (Screen.width == 720 && Screen.height == 720)
                Screen.SetResolution(720, 400, false);
            else
                Screen.SetResolution(400, 720, false);
        }

        public void EnableAnimations(bool enabled)
        {
            foreach (var animator in FindObjectsOfType<Animator>())
            {
                animator.enabled = enabled;
            }
        }

        public void UIParticle_Enable(bool enabled)
        {

            foreach (var uip in root.GetComponentsInChildren<UIParticle>(true))
            {
                uip.enabled = enabled;
            }
            if (!enabled)
            {
                foreach (var ps in FindObjectsOfType<ParticleSystem>())
                {
                    ps.Play(false);
                }
            }
        }

        public void UIParticle_MeshSharing(bool enabled)
        {
            foreach (var uip in root.GetComponentsInChildren<UIParticle>(true))
            {
                uip.meshSharing = enabled ? UIParticle.MeshSharing.Auto : UIParticle.MeshSharing.None;
            }
        }

        public void UIParticle_RandomGroup(bool enabled)
        {
            foreach (var uip in root.GetComponentsInChildren<UIParticle>(true))
            {
                uip.groupMaxId = enabled ? 4 : 0;
            }
        }

        public void UIParticle_Scale(float scale)
        {
            foreach (var uip in FindObjectsOfType<UIParticle>())
            {
                uip.scale = scale;
            }
        }

        public void ParticleSystem_WorldSpaseSimulation(bool enabled)
        {
            foreach (var ps in FindObjectsOfType<ParticleSystem>())
            {
                var main = ps.main;
                main.simulationSpace = enabled ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
            }
        }

        public void ParticleSystem_SetScale(float scale)
        {
            foreach (var ps in FindObjectsOfType<ParticleSystem>())
            {
                ps.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        public void Canvas_WorldSpace(bool flag)
        {
            if (flag)
            {
                var canvas = FindObjectOfType<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.transform.rotation = Quaternion.Euler(new Vector3(0, 10, 0));
            }
        }

        public void Canvas_CameraSpace(bool flag)
        {
            if (flag)
            {
                var canvas = FindObjectOfType<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
            }
        }

        public void Canvas_Overlay(bool flag)
        {
            if (flag)
            {
                var canvas = FindObjectOfType<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }
    }
}
