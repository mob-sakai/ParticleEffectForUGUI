using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIParticleInternal.AssetModification
{
    internal abstract class ComponentModifier<T> : IComponentModifier where T : Component
    {
        private static readonly List<T> s_Components = new List<T>();

        public bool isModified { get; private set; }

        public bool ModifyComponent(GameObject root, bool dryRun)
        {
            root.GetComponentsInChildren(true, s_Components);
            foreach (var c in s_Components)
            {
                if (PrefabUtility.IsPartOfAnyPrefab(c.gameObject)) continue;

                if (ModifyComponent(c, dryRun))
                {
                    isModified = true;
                }
            }

            return isModified;
        }

        public abstract string Report();

        protected abstract bool ModifyComponent(T component, bool dryRun);
    }
}
