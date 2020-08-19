using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Coffee.UIExtensions
{
    [CustomEditor(typeof(UIParticle))]
    [CanEditMultipleObjects]
    internal class UIParticleEditor : GraphicEditor
    {
        //################################
        // Constant or Static Members.
        //################################
        private static readonly GUIContent s_ContentParticleMaterial = new GUIContent("Particle Material", "The material for rendering particles");
        private static readonly GUIContent s_ContentTrailMaterial = new GUIContent("Trail Material", "The material for rendering particle trails");
        private static readonly GUIContent s_ContentAdvancedOptions = new GUIContent("Advanced Options");
        private static readonly GUIContent s_Content3D = new GUIContent("3D");
        private static readonly GUIContent s_ContentScale = new GUIContent("Scale");
        private static readonly List<ParticleSystem> s_ParticleSystems = new List<ParticleSystem>();

        private SerializedProperty _spParticleSystem;
        private SerializedProperty _spScale3D;
        private SerializedProperty _spIgnoreCanvasScaler;
        private SerializedProperty _spAnimatableProperties;
        private bool _xyzMode;

        private static readonly List<string> s_MaskablePropertyNames = new List<string>
        {
            "_Stencil",
            "_StencilComp",
            "_StencilOp",
            "_StencilWriteMask",
            "_StencilReadMask",
            "_ColorMask",
        };


        //################################
        // Public/Protected Members.
        //################################
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _spParticleSystem = serializedObject.FindProperty("m_ParticleSystem");
            _spScale3D = serializedObject.FindProperty("m_Scale3D");
            _spIgnoreCanvasScaler = serializedObject.FindProperty("m_IgnoreCanvasScaler");
            _spAnimatableProperties = serializedObject.FindProperty("m_AnimatableProperties");
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var current = target as UIParticle;
            if (current == null) return;

            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_spParticleSystem);
            EditorGUI.EndDisabledGroup();

            // Draw materials.
            EditorGUI.indentLevel++;
            var ps = _spParticleSystem.objectReferenceValue as ParticleSystem;
            if (ps != null)
            {
                var pr = ps.GetComponent<ParticleSystemRenderer>();
                var sp = new SerializedObject(pr).FindProperty("m_Materials");

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(0), s_ContentParticleMaterial);
                    if (2 <= sp.arraySize)
                    {
                        EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(1), s_ContentTrailMaterial);
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    sp.serializedObject.ApplyModifiedProperties();
                }

                if (!Application.isPlaying && pr.enabled)
                {
                    EditorGUILayout.HelpBox("UIParticles disable the RendererModule in ParticleSystem at runtime to prevent double rendering.", MessageType.Warning);
                }
            }

            EditorGUI.indentLevel--;

            // Advanced Options
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(s_ContentAdvancedOptions, EditorStyles.boldLabel);

            _xyzMode = DrawFloatOrVector3Field(_spScale3D, _xyzMode);

            EditorGUILayout.PropertyField(_spIgnoreCanvasScaler);

            // AnimatableProperties
            AnimatedPropertiesEditor.DrawAnimatableProperties(_spAnimatableProperties, current.material);

            // Fix
            current.GetComponentsInChildren(true, s_ParticleSystems);
            if (s_ParticleSystems.Any(x => x.GetComponent<UIParticle>() == null))
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("There are child ParticleSystems that does not have a UIParticle component.\nAdd UIParticle component to them.", MessageType.Warning);
                GUILayout.BeginVertical();
                if (GUILayout.Button("Fix"))
                {
                    foreach (var p in s_ParticleSystems.Where(x => !x.GetComponent<UIParticle>()))
                    {
                        p.gameObject.AddComponent<UIParticle>();
                    }
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            s_ParticleSystems.Clear();

            // Does the shader support UI masks?
            if (current.maskable && current.material && current.material.shader)
            {
                var mat = current.material;
                var shader = mat.shader;
                foreach (var propName in s_MaskablePropertyNames)
                {
                    if (mat.HasProperty(propName)) continue;

                    EditorGUILayout.HelpBox(string.Format("Shader '{0}' doesn't have '{1}' property. This graphic cannot be masked.", shader.name, propName), MessageType.Warning);
                    break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static bool DrawFloatOrVector3Field(SerializedProperty sp, bool showXyz)
        {
            var x = sp.FindPropertyRelative("x");
            var y = sp.FindPropertyRelative("y");
            var z = sp.FindPropertyRelative("z");

            showXyz |= !Mathf.Approximately(x.floatValue, y.floatValue) ||
                       !Mathf.Approximately(y.floatValue, z.floatValue) ||
                       y.hasMultipleDifferentValues ||
                       z.hasMultipleDifferentValues;

            EditorGUILayout.BeginHorizontal();
            if (showXyz)
            {
                EditorGUILayout.PropertyField(sp);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(x, s_ContentScale);
                if (EditorGUI.EndChangeCheck())
                    z.floatValue = y.floatValue = x.floatValue;
            }

            EditorGUI.BeginChangeCheck();
            showXyz = GUILayout.Toggle(showXyz, s_Content3D, EditorStyles.miniButton, GUILayout.Width(30));
            if (EditorGUI.EndChangeCheck() && !showXyz)
                z.floatValue = y.floatValue = x.floatValue;
            EditorGUILayout.EndHorizontal();

            return showXyz;
        }
    }
}
