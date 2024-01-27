using Coffee.UIExtensions;
using Coffee.UIParticleInternal.AssetModification;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0612 // Type or member is obsolete

namespace Coffee.UIParticleInternal
{
    internal class UIParticleComponentModifier_AutoScaling : ComponentModifier<UIParticle>
    {
        protected override bool ModifyComponent(UIParticle uip, bool dryRun)
        {
            if (!uip.m_AutoScaling) return false;

            uip.m_AutoScaling = false;
            uip.autoScalingMode = UIParticle.AutoScalingMode.Transform;
            uip.transform.localScale = Vector3.one;

            if (!dryRun)
            {
                EditorUtility.SetDirty(uip);
                EditorUtility.SetDirty(uip.transform);
            }

            return true;
        }

        public override string Report()
        {
            return
                "  -> UIParticle.ignoreCanvasScaler and UIParticle.autoScaling are obsolete." +
                " Use UIParticle.autoScalingMode instead.\n";
        }
    }
}
