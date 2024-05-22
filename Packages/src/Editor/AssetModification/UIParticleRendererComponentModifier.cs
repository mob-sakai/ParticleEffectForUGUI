using Coffee.UIExtensions;
using Coffee.UIParticleInternal.AssetModification;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIParticleInternal
{
    internal class UIParticleRendererComponentModifier : ComponentModifier<UIParticleRenderer>
    {
        protected override bool ModifyComponent(UIParticleRenderer c, bool dryRun)
        {
            if (c.hideFlags.HasFlag(HideFlags.DontSave | HideFlags.NotEditable)) return false;

            if (!dryRun)
            {
                var go = c.gameObject;
                Object.DestroyImmediate(c);
                EditorUtility.SetDirty(go);
            }

            return true;
        }

        public override string Report()
        {
            return "  -> UIParticleRenderer component is now auto-generated object. Remove them.\n";
        }
    }
}
