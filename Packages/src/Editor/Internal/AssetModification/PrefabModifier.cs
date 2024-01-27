using System;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIParticleInternal.AssetModification
{
    internal class PrefabModifier : GameObjectModifier
    {
        protected override string id => "Prefab";

        protected override bool RunModify(bool dryRun)
        {
            using (var scope = new EditScope(path))
            {
                var changed = ModifyGameObject(scope.root, dryRun);

                if (!dryRun && changed)
                {
                    scope.Save();
                }

                return changed;
            }
        }

        private readonly struct EditScope : IDisposable
        {
            private readonly string _path;
            public readonly GameObject root;

            public EditScope(string path)
            {
                _path = path;
                root = PrefabUtility.LoadPrefabContents(path);
            }

            public void Dispose()
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            public void Save()
            {
                PrefabUtility.SaveAsPrefabAsset(root, _path);
            }
        }
    }
}
