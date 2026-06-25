using System.Collections.Generic;
using System.Linq;
using Coffee.UIParticleInternal;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
    [Icon("Packages/com.coffee.ui-particle/Editor/UIParticleIcon.png")]
    [ExecuteAlways]
    internal class ParticleSystemPreviewer : MonoBehaviour
    {
        // Do nothing.
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ParticleSystemPreviewer))]
    [CanEditMultipleObjects]
    internal class ParticleSystemPreviewerEditor : Editor
    {
        private GameObject[] _gameObjects;

        private void OnEnable()
        {
            _gameObjects = targets.OfType<ParticleSystemPreviewer>().Select(x => x.gameObject).ToArray();
            ParticleSystemPreviewSystem.Register(_gameObjects);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ParticleSystemPreviewSystem.DrawWarningForTemporary(_gameObjects);
            ParticleSystemPreviewSystem.DrawWarningForPermanent(_gameObjects);
        }
    }

    /// <summary>
    /// This class manages temporary ParticleSystems for preview purposes.
    /// When previewing in the editor, it is common to place an empty ParticleSystem as the root, but it consumes memory at runtime if included in the build.
    /// The temporary ParticleSystems created by this class only exist when the specified GameObject is selected, and are automatically deleted when the selection is cleared.
    /// </summary>
    internal class ParticleSystemPreviewSystem : ScriptableSingleton<ParticleSystemPreviewSystem>
    {
        private const HideFlags k_TemporaryHideFlags = HideFlags.DontSave | HideFlags.NotEditable;

        [SerializeField]
        private List<GameObject> m_PreviewObjects = new List<GameObject>();

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            instance.OnSelectionChanged();

            Selection.selectionChanged -= instance.OnSelectionChanged;
            Selection.selectionChanged += instance.OnSelectionChanged;
        }

        /// <summary>
        /// Adds a temporary ParticleSystem to the specified GameObject for preview purposes.
        /// </summary>
        public static void Register(GameObject[] targets)
        {
            foreach (var target in targets)
            {
                Register(target);
            }
        }

        /// <summary>
        /// Adds a temporary ParticleSystem to the specified GameObject for preview purposes.
        /// </summary>
        public static void Register(GameObject target)
        {
            if (!target) return;
            if (EditorApplication.isPlaying) return;
            if (instance.m_PreviewObjects.Contains(target)) return;
            if (target.TryGetComponent<ParticleSystem>(out var ps))
            {
                if (ps.hideFlags == k_TemporaryHideFlags)
                {
                    RegisterParticleSystem(ps);
                }

                return;
            }

            // Create temporary ParticleSystem for preview.
            RegisterParticleSystem(target.AddComponent<ParticleSystem>());
        }

        /// <summary>
        /// Removes the temporary ParticleSystem associated with the specified GameObject.
        /// </summary>
        /// <param name="target"></param>
        public static void Unregister(GameObject target)
        {
            if (!target) return;

            var index = instance.m_PreviewObjects.IndexOf(target);
            if (index < 0) return;

            instance.m_PreviewObjects.RemoveAt(index);
            if (HasTemporaryParticleSystem(target))
            {
                RemoveParticleSystem(target);
            }
        }

        private static void RegisterParticleSystem(ParticleSystem ps)
        {
            if (!ps) return;
            if (EditorApplication.isPlaying) return;

            ps.hideFlags = k_TemporaryHideFlags;

            var emission = ps.emission;
            emission.enabled = false;
            var shape = ps.shape;
            shape.enabled = false;

            if (ps.TryGetComponent<ParticleSystemRenderer>(out var psr))
            {
                psr.enabled = false;
                psr.hideFlags = k_TemporaryHideFlags;
            }

            instance.m_PreviewObjects.Add(ps.gameObject);
            EditorUtility.SetDirty(ps.gameObject);
        }

        /// <summary>
        /// Removes the temporary ParticleSystem associated with the specified GameObject.
        /// </summary>
        /// <param name="target"></param>
        private static void RemoveParticleSystem(GameObject target)
        {
            if (target.TryGetComponent<ParticleSystem>(out var ps))
            {
                Misc.DestroyImmediate(ps);
                EditorUtility.SetDirty(target);
            }

            if (target.TryGetComponent<ParticleSystem>(out var psr))
            {
                Misc.DestroyImmediate(psr);
                EditorUtility.SetDirty(target);
            }
        }

        /// <summary>
        /// Checks if the specified GameObject has a temporary ParticleSystem.
        /// </summary>
        private static bool HasTemporaryParticleSystem(GameObject target)
        {
            return target
                   && instance.m_PreviewObjects.Contains(target)
                   && target.TryGetComponent<ParticleSystem>(out var ps)
                   && ps.hideFlags == k_TemporaryHideFlags;
        }

        /// <summary>
        /// Checks if the specified GameObject has a permanent ParticleSystem.
        /// </summary>
        private static bool HasPermanentParticleSystem(GameObject target)
        {
            return target
                   && target.TryGetComponent<ParticleSystem>(out var ps)
                   && ps.hideFlags != k_TemporaryHideFlags;
        }

        private void OnSelectionChanged()
        {
            var selectedGameObjects = Selection.gameObjects;
            for (var i = m_PreviewObjects.Count - 1; 0 <= i; i--)
            {
                var go = m_PreviewObjects[i];
                if (!go)
                {
                    m_PreviewObjects.RemoveAt(i);
                }
                else if (EditorApplication.isPlaying && !selectedGameObjects.Contains(go))
                {
                    Unregister(go);
                }
            }
        }

        public static void DrawWarningForTemporary(GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0 || !gameObjects.Any(HasTemporaryParticleSystem)) return;

            if (WarningButton("The temporary ParticleSystem for preview is attached.\n" +
                              "It will be removed when exiting edit mode.", "Remove"))
            {
                foreach (var go in gameObjects)
                {
                    if (HasTemporaryParticleSystem(go))
                    {
                        RemoveParticleSystem(go);
                    }
                }
            }
        }

        public static void DrawWarningForPermanent(GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0 || !gameObjects.Any(HasPermanentParticleSystem)) return;

            if (WarningButton("The permanent ParticleSystem is attached.\n" +
                              "It will be included in build.", "Remove"))
            {
                foreach (var go in gameObjects)
                {
                    if (HasPermanentParticleSystem(go))
                    {
                        RemoveParticleSystem(go);
                        Unregister(go);
                        Register(go);
                    }
                }
            }
        }

        private static bool WarningButton(string message, string buttonText)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(message, MessageType.Warning, true);
            var clicked = GUILayout.Button(EditorGUIUtility.TrTempContent(buttonText));
            EditorGUILayout.EndHorizontal();
            return clicked;
        }
    }
#endif
}
