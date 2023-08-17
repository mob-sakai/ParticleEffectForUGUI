using UnityEngine;
using UnityEngine.Serialization;

namespace Coffee.UIExtensions.Demo
{
    public class UIParticle_Demo_UIParticleController : MonoBehaviour
    {
        [FormerlySerializedAs("root")]
        [SerializeField]
        private Transform m_RootTransform;

        public void UIParticle_MeshSharing(bool flag)
        {
            foreach (var uip in m_RootTransform.GetComponentsInChildren<UIParticle>(true))
            {
                uip.meshSharing = flag
                    ? UIParticle.MeshSharing.Auto
                    : UIParticle.MeshSharing.None;
            }
        }

        public void UIParticle_RandomGroup(bool flag)
        {
            foreach (var uip in m_RootTransform.GetComponentsInChildren<UIParticle>(true))
            {
                uip.groupMaxId = flag
                    ? 4
                    : 0;
            }
        }

        public void UIParticle_Scale(float scale)
        {
            foreach (var uip in m_RootTransform.GetComponentsInChildren<UIParticle>(true))
            {
                uip.scale = scale;
            }
        }
    }
}
