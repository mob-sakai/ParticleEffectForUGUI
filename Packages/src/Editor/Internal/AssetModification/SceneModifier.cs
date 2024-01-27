using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Coffee.UIParticleInternal.AssetModification
{
    internal class SceneModifier : GameObjectModifier
    {
        protected override string id => "Scene";

        protected override bool RunModify(bool dryRun)
        {
            using (var scope = new EditScope(path))
            {
                var changed = false;
                foreach (var root in scope.scene.GetRootGameObjects())
                {
                    if (ModifyGameObject(root, dryRun))
                    {
                        changed = true;
                    }
                }

                if (!dryRun && changed)
                {
                    scope.Save();
                }

                return changed;
            }
        }

        private readonly struct EditScope : IDisposable
        {
            public readonly Scene scene;

            public EditScope(string path)
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }

            public void Dispose()
            {
                EditorSceneManager.CloseScene(scene, true);
            }

            public void Save()
            {
                EditorSceneManager.SaveScene(scene);
            }
        }
    }
}
