#pragma warning disable CS0414
using Coffee.UIParticleInternal;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Coffee.UIExtensions
{
    public class UIParticleProjectSettings : PreloadedProjectSettings<UIParticleProjectSettings>
    {
        [Header("Setting")]
        [SerializeField]
        [Tooltip("Automatically correct the color space of the mesh.")]
        [FormerlySerializedAs("m_EnableLinearToGamma")]
        private bool m_AutoColorCorrection = true;

        public static bool autoColorCorrection
        {
            get => instance.m_AutoColorCorrection;
            set => instance.m_AutoColorCorrection = value;
        }

        [SerializeField]
        [Tooltip("Default view size for baking particle systems.")]
        private float m_DefaultViewSizeForBaking = 10;

        public static float defaultViewSizeForBaking
        {
            get => instance.m_DefaultViewSizeForBaking;
            set => instance.m_DefaultViewSizeForBaking = value;
        }

        [Header("Editor")]
        [Tooltip("Hide the automatically generated objects.\n" +
                 "  - UIParticleRenderer\n" +
                 "  - UIParticle BakingCamera")]
        [SerializeField]
        private bool m_HideGeneratedObjects = true;

        [Tooltip("When selecting UIParticle, a temporary ParticleSystem is generated for preview.")]
        [SerializeField]
        private bool m_PreviewOnSelect = true;

        internal static HideFlags globalHideFlags => instance.m_HideGeneratedObjects
            ? HideFlags.DontSave | HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector
            : HideFlags.DontSave | HideFlags.NotEditable;

        internal static bool previewOnSelect => instance.m_PreviewOnSelect;

#if UNITY_EDITOR
        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new PreloadedProjectSettingsProvider("Project/UI/UI Particle");
        }

        [CustomEditor(typeof(UIParticleProjectSettings))]
        private class UIParticleProjectSettingsEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                EditorGUIUtility.labelWidth = 180;
                base.OnInspectorGUI();
            }
        }
#endif
    }
}
