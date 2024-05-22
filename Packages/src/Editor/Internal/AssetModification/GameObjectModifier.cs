using System.Linq;
using System.Text;
using UnityEngine;

namespace Coffee.UIParticleInternal.AssetModification
{
    internal abstract class GameObjectModifier : Modifier
    {
        private static readonly StringBuilder s_ReportLog = new StringBuilder();
        public IComponentModifier[] componentModifiers;

        protected bool ModifyGameObject(GameObject root, bool dryRun)
        {
            foreach (var modifier in componentModifiers)
            {
                modifier.ModifyComponent(root, dryRun);
            }

            return componentModifiers.Any(x => x.isModified);
        }

        protected override string ModificationReport()
        {
            if (!hasUpgraded) return string.Empty;

            s_ReportLog.Length = 0;
            foreach (var componentModifier in componentModifiers)
            {
                if (componentModifier.isModified)
                {
                    s_ReportLog.Append(componentModifier.Report());
                }
            }

            if (0 < s_ReportLog.Length)
            {
                s_ReportLog.Length--;
            }

            return s_ReportLog.ToString();
        }
    }
}
