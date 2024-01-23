using UnityEngine;
using UnityEngine.Serialization;

namespace Coffee.UIExtensions.Demo
{
    public class UIParticle_PerformanceDemo : MonoBehaviour
    {
        [FormerlySerializedAs("copyOrigin")]
        [SerializeField]
        private GameObject m_CopyOrigin;

        [FormerlySerializedAs("copyCount")]
        [SerializeField]
        public int m_CopyCount;

        [FormerlySerializedAs("root")]
        [SerializeField]
        public Canvas m_RootCanvas;

        private void Start()
        {
            Application.targetFrameRate = 60;

            if (m_CopyOrigin)
            {
                m_CopyOrigin.SetActive(false);

                var parent = m_CopyOrigin.transform.parent;
                for (var i = 0; i < m_CopyCount; i++)
                {
                    var go = Instantiate(m_CopyOrigin, parent, false);
                    go.name = string.Format("{0} {1}", m_CopyOrigin.name, i + 1);
                    go.hideFlags = HideFlags.DontSave;

                    go.SetActive(true);
                }
            }
        }

        public void UIParticle_Enable(bool flag)
        {
            foreach (var uip in m_RootCanvas.GetComponentsInChildren<UIParticle>(true))
            {
                uip.enabled = flag;
            }

            if (!flag)
            {
                foreach (var ps in FindObjectsOfType<ParticleSystem>())
                {
                    ps.Play(false);
                }
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

        public void ParticleSystem_SetScale(float scale)
        {
            foreach (var ps in FindObjectsOfType<ParticleSystem>())
            {
                ps.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}
