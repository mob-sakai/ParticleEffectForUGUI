using UnityEngine;

namespace Coffee.UIParticleInternal.AssetModification
{
    internal interface IComponentModifier
    {
        bool isModified { get; }
        bool ModifyComponent(GameObject root, bool dryRun);
        string Report();
    }
}
