using UnityEngine;

namespace Coffee.UIExtensions.Demo
{
    public class UIParticle_Demo_UIParticleController : MonoBehaviour
    {
        public Transform root;

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
            foreach (var uip in root.GetComponentsInChildren<UIParticle>(true))
            {
                uip.scale = scale;
            }
        }
    }
}
