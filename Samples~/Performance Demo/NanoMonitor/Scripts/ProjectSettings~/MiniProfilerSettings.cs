using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coffee.MiniProfiler
{
    internal class MiniProfilerSettings : ScriptableSettings<MiniProfilerSettings>
    {
        [Header("UI")]
        [SerializeField] private Font m_Font = null;

        [Header("Instantiate On Boot")]
        [SerializeField] private GameObject miniProfilerPrefab = null;
        [SerializeField] private string[] m_SceneNames = new string[0];

        public Font font => m_Font;
        public event Action onFontChanged;

        protected override void OnBoot()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode __)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (!miniProfilerPrefab || !m_SceneNames.Contains(scene.name)) return;

            var go = Instantiate(miniProfilerPrefab);
            if (Application.isPlaying)
                DontDestroyOnLoad(go);
        }

        protected override bool bootOnPlayMode => true;
        protected override bool bootOnEditorMode => false;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            onFontChanged?.Invoke();
            base.OnValidate();
        }

        private void Reset()
        {
            m_Font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        [UnityEditor.SettingsProvider]
        private static UnityEditor.SettingsProvider CreateSettingsProvider() => new ScriptableSettingsProvider<MiniProfilerSettings>();
#endif
    }
}
