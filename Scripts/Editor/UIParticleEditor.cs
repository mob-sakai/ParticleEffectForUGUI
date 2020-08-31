using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    [CustomEditor(typeof(UIParticle))]
    [CanEditMultipleObjects]
    internal class UIParticleEditor : GraphicEditor
    {
        //################################
        // Constant or Static Members.
        //################################
        private static readonly GUIContent s_ContentRenderingOrder = new GUIContent("Rendering Order");
        private static readonly GUIContent s_ContentRefresh = new GUIContent("Refresh");

        private SerializedProperty _spScale;
        private SerializedProperty _spIgnoreCanvasScaler;
        private SerializedProperty _spAnimatableProperties;

        private ReorderableList _ro;

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
            _spScale = serializedObject.FindProperty("m_Scale");
            _spIgnoreCanvasScaler = serializedObject.FindProperty("m_IgnoreCanvasScaler");
            _spAnimatableProperties = serializedObject.FindProperty("m_AnimatableProperties");

            var sp = serializedObject.FindProperty("m_Particles");
            _ro = new ReorderableList(sp.serializedObject, sp, true, true, false, false);
            _ro.elementHeight = EditorGUIUtility.singleLineHeight + 4;
            _ro.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.y += 1;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.ObjectField(rect, sp.GetArrayElementAtIndex(index), GUIContent.none);
            };
            _ro.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 150, rect.height), s_ContentRenderingOrder);

                if (GUI.Button(new Rect(rect.width - 80, rect.y - 1, 80, rect.height), s_ContentRefresh, EditorStyles.miniButton))
                {
                    foreach (UIParticle t in targets)
                    {
                        t.RefreshParticles();
                    }
                }
            };
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var current = target as UIParticle;
            if (current == null) return;

            serializedObject.Update();

            // IgnoreCanvasScaler
            EditorGUILayout.PropertyField(_spIgnoreCanvasScaler);

            // Scale
            EditorGUILayout.PropertyField(_spScale);

            // AnimatableProperties
            AnimatedPropertiesEditor.DrawAnimatableProperties(_spAnimatableProperties, current.material);

            _ro.DoLayoutList();

            // Does the shader support UI masks?
            if (current.maskable && current.GetComponentInParent<Mask>())
            {
                foreach (var mat in current.materials)
                {
                    if (!mat || !mat.shader) continue;
                    var shader = mat.shader;
                    foreach (var propName in s_MaskablePropertyNames)
                    {
                        if (mat.HasProperty(propName)) continue;

                        EditorGUILayout.HelpBox(string.Format("Shader '{0}' doesn't have '{1}' property. This graphic cannot be masked.", shader.name, propName), MessageType.Warning);
                        break;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
