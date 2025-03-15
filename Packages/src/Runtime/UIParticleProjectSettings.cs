#pragma warning disable CS0414
using Coffee.UIParticleInternal;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
    public class UIParticleProjectSettings : PreloadedProjectSettings<UIParticleProjectSettings>
    {
        [Header("Setting")]
        [SerializeField]
        internal bool m_EnableLinearToGamma = true;

        public static bool enableLinearToGamma
        {
            get => instance.m_EnableLinearToGamma;
            set => instance.m_EnableLinearToGamma = value;
        }


        [Header("Editor")]
        [Tooltip("Hide the automatically generated objects.\n" +
                 "  - UIParticleRenderer\n" +
                 "  - UIParticle BakingCamera")]
        [SerializeField]
        private bool m_HideGeneratedObjects = true;

        public static HideFlags globalHideFlags => instance.m_HideGeneratedObjects
            ? HideFlags.DontSave | HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector
            : HideFlags.DontSave | HideFlags.NotEditable;

#if UNITY_EDITOR
        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new PreloadedProjectSettingsProvider("Project/UI/UI Particle");
        }
#endif
    }
}
