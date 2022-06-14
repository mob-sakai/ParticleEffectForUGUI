using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Coffee.MiniProfiler
{
    internal abstract class ScriptableSettings : ScriptableObject
    {
#if UNITY_EDITOR
        public virtual string GetSettingPath()
        {
            return GetSettingPath(GetType());
        }

        internal static string GetSettingPath(Type type)
        {
            return $"Project/{ObjectNames.NicifyVariableName(type.Name.Replace("ProjectSettings", ""))}";
        }
#endif

        public virtual string GetDefaultAssetPath()
        {
            return $"Assets/ProjectSettings/{GetType().Name}.asset";
        }
    }

    internal abstract class ScriptableSettings<T> : ScriptableSettings
        where T : ScriptableSettings<T>
    {
        //################################
        // Public Members.
        //################################
        public static T Instance => _instance ? _instance : _instance = GetOrCreate();

        private static T _instance;

        protected virtual bool bootOnEditorMode => true;
        protected virtual bool bootOnPlayMode => true;

        protected abstract void OnBoot();

#if UNITY_EDITOR
        //################################
        // Private Members.
        //################################
        private bool enabled;

        private static T GetOrCreate()
        {
            return PlayerSettings.GetPreloadedAssets()
                .OfType<T>()
                .FirstOrDefault() ?? CreateInstance<T>();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    enabled = true;
                    Boot();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    enabled = false;
                    break;
            }
        }

        private void Boot()
        {
            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            var assets = preloadedAssets.OfType<T>().ToArray();
            var first = assets.FirstOrDefault() ?? this as T;

            // If there are no preloaded assets, registry the first asset.
            // If there are multiple preloaded assets, unregisters all but the first one.
            if (assets.Length != 1)
            {
                // The first asset is not saved.
                if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(first)))
                {
                    if (!AssetDatabase.IsValidFolder("Assets/ProjectSettings"))
                        AssetDatabase.CreateFolder("Assets", "ProjectSettings");

                    var assetName = ObjectNames.NicifyVariableName(first.GetType().Name.Replace("ProjectSettings", ""));
                    var assetPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/ProjectSettings/{assetName}.asset");
                    AssetDatabase.CreateAsset(first, assetPath);
                }

                PlayerSettings.SetPreloadedAssets(preloadedAssets
                    .Where(x => x)
                    .Except(assets)
                    .Concat(new[] {first})
                    .ToArray());
            }

            // Another asset is registered as a preload asset.
            if (first != this)
            {
                UnityEngine.Debug.LogError($"Another asset '{first}' is registered as a preload '{typeof(T).Name}' asset." +
                                           $"\nThis instance ('{this}') will be ignored. Check 'Project Settings > Player > Preload Assets'", this);
                return;
            }

            _instance = first;

            // Do nothing on editor mode.
            if (!bootOnEditorMode && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            // Do nothing on play mode.
            if (!bootOnPlayMode && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            OnBoot();
        }


        //################################
        // Unity Callbacks.
        //################################
        private void OnEnable()
        {
            enabled = true;
            Boot();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        protected virtual void OnValidate()
        {
            if (enabled)
                Boot();
        }
#else
        private static T GetOrCreate()
        {
            return CreateInstance<T>();
        }

        private void OnEnable()
        {
            if (!bootOnPlayMode) return;

            _instance = this as T;
            OnBoot();
        }
#endif
    }

    [CustomEditor(typeof(ScriptablePreferenceSettings<>), true)]
    internal class ScriptablePreferenceSettingsEditor : Editor
    {
        private static readonly GUIContent _button = new GUIContent("Open Project Settings");

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(_button))
            {
                SettingsService.OpenProjectSettings(((ScriptableSettings) target).GetSettingPath());
            }
        }
    }

    internal abstract class ScriptablePreferenceSettings<T> : ScriptableSingleton<T>
        where T : ScriptablePreferenceSettings<T>
    {
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableSettings), true)]
    internal class ScriptableSettingsEditor : Editor
    {
        private static readonly GUIContent _button = new GUIContent("Open Project Settings");

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(_button))
            {
                SettingsService.OpenProjectSettings(((ScriptableSettings) target).GetSettingPath());
            }
        }
    }

    internal class ScriptableSettingsProvider<T> : SettingsProvider where T : ScriptableSettings<T>
    {
        public ScriptableSettingsProvider(string path) : base(path)
        {
        }

        public ScriptableSettingsProvider() : base(ScriptableSettings.GetSettingPath(typeof(T)))
        {
        }

        protected SerializedObject serializedObject { get; private set; }
        protected ScriptableSettings<T> target { get; private set; }

        public sealed override void OnGUI(string searchContext)
        {
            if (!target)
            {
                target = ScriptableSettings<T>.Instance;
                serializedObject = new SerializedObject(target);
            }

            OnGUI();
        }

        protected virtual void OnGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.propertyPath != "m_Script")
                    EditorGUILayout.PropertyField(iterator, true);

                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    internal class ScriptablePreferenceSettingsProvider<T> : SettingsProvider where T : ScriptableSettings<T>
    {
        public ScriptablePreferenceSettingsProvider(string path) : base(path, SettingsScope.User)
        {
        }

        public ScriptablePreferenceSettingsProvider() : base(ScriptableSettings.GetSettingPath(typeof(T)), SettingsScope.User)
        {
        }

        protected SerializedObject serializedObject { get; private set; }
        protected ScriptableSettings<T> target { get; private set; }

        public sealed override void OnGUI(string searchContext)
        {
            if (!target)
            {
                target = ScriptableSettings<T>.Instance;
                serializedObject = new SerializedObject(target);
            }

            OnGUI();
        }

        protected virtual void OnGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.propertyPath != "m_Script")
                    EditorGUILayout.PropertyField(iterator, true);

                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
