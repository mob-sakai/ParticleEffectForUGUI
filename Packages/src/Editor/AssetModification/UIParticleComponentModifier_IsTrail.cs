using Coffee.UIExtensions;
using Coffee.UIParticleInternal.AssetModification;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0612 // Type or member is obsolete

namespace Coffee.UIParticleInternal
{
    internal class UIParticleComponentModifier_IsTrail : ComponentModifier<UIParticle>
    {
        protected override bool ModifyComponent(UIParticle uip, bool dryRun)
        {
            if (!uip.m_IsTrail) return false;

            if (!dryRun)
            {
                var go = uip.gameObject;
                Object.DestroyImmediate(uip);
                EditorUtility.SetDirty(go);
            }

            return true;
        }

        public override string Report()
        {
            return "  -> UIParticle for trail is no longer needed. Removed.";
        }
    }
}
