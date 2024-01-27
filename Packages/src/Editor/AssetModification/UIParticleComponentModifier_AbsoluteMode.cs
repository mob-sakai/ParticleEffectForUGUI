using Coffee.UIExtensions;
using Coffee.UIParticleInternal.AssetModification;
using UnityEditor;

#pragma warning disable CS0612 // Type or member is obsolete

namespace Coffee.UIParticleInternal
{
    internal class UIParticleComponentModifier_AbsoluteMode : ComponentModifier<UIParticle>
    {
        protected override bool ModifyComponent(UIParticle uip, bool dryRun)
        {
            if (!uip.m_AbsoluteMode) return false;

            uip.m_AbsoluteMode = false;
            uip.positionMode = UIParticle.PositionMode.Absolute;

            if (!dryRun)
            {
                EditorUtility.SetDirty(uip);
            }

            return true;
        }

        public override string Report()
        {
            return "  -> UIParticle.absoluteMode is obsolete. Use UIParticle.positionMode instead.\n";
        }
    }
}
