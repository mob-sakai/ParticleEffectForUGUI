#if IGNORE_ACCESS_CHECKS // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using ShaderPropertyType = Coffee.UIExtensions.UIParticle.AnimatableProperty.ShaderPropertyType;

namespace Coffee.UIExtensions
{
    [CustomEditor(typeof(UIParticle))]
    [CanEditMultipleObjects]
    public class UIParticleEditor : GraphicEditor
    {
        class AnimatedPropertiesEditor
        {
            static readonly List<string> s_ActiveNames = new List<string>();
            static readonly System.Text.StringBuilder s_Sb = new System.Text.StringBuilder();

            public string name;
            public ShaderPropertyType type;

            static string CollectActiveNames(SerializedProperty sp, List<string> result)
            {
                result.Clear();
                for (int i = 0; i < sp.arraySize; i++)
                {
                    result.Add(sp.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue);
                }

                s_Sb.Length = 0;
                if (result.Count == 0)
                {
                    s_Sb.Append("Nothing");
                }
                else
                {
                    result.Aggregate(s_Sb, (a, b) => s_Sb.AppendFormat("{0}, ", b));
                    s_Sb.Length -= 2;
                }

                return s_Sb.ToString();
            }

            public static void DrawAnimatableProperties(SerializedProperty sp, Material mat)
            {
                if (!mat || !mat.shader)
                    return;
                bool isClicked = false;
                using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
                {
                    var r = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(true), new GUIContent(sp.displayName, sp.tooltip));
                    isClicked = GUI.Button(r, CollectActiveNames(sp, s_ActiveNames), EditorStyles.popup);
                }

                if (isClicked)
                {
                    GenericMenu gm = new GenericMenu();
                    gm.AddItem(new GUIContent("Nothing"), s_ActiveNames.Count == 0, () =>
                    {
                        sp.ClearArray();
                        sp.serializedObject.ApplyModifiedProperties();
                    });


                    for (int i = 0; i < sp.arraySize; i++)
                    {
                        var p = sp.GetArrayElementAtIndex(i);
                        var name = p.FindPropertyRelative("m_Name").stringValue;
                        var type = (ShaderPropertyType)p.FindPropertyRelative("m_Type").intValue;
                        AddMenu(gm, sp, new AnimatedPropertiesEditor() { name = name, type = type }, false);
                    }

                    for (int i = 0; i < ShaderUtil.GetPropertyCount(mat.shader); i++)
                    {
                        var pName = ShaderUtil.GetPropertyName(mat.shader, i);
                        var type = (ShaderPropertyType)ShaderUtil.GetPropertyType(mat.shader, i);
                        AddMenu(gm, sp, new AnimatedPropertiesEditor() { name = pName, type = type }, true);

                        if (type == ShaderPropertyType.Texture)
                        {
                            AddMenu(gm, sp, new AnimatedPropertiesEditor() { name = pName + "_ST", type = ShaderPropertyType.Vector }, true);
                            AddMenu(gm, sp, new AnimatedPropertiesEditor() { name = pName + "_HDR", type = ShaderPropertyType.Vector }, true);
                            AddMenu(gm, sp, new AnimatedPropertiesEditor() { name = pName + "_TexelSize", type = ShaderPropertyType.Vector }, true);
                        }

                    }

                    gm.ShowAsContext();
                }
            }

            public static void AddMenu(GenericMenu menu, SerializedProperty sp, AnimatedPropertiesEditor property, bool add)
            {
                if (add && s_ActiveNames.Contains(property.name))
                    return;

                menu.AddItem(new GUIContent(string.Format("{0} ({1})", property.name, property.type)), s_ActiveNames.Contains(property.name), () =>
            {
                var index = s_ActiveNames.IndexOf(property.name);
                if (0 <= index)
                {
                    sp.DeleteArrayElementAtIndex(index);
                }
                else
                {
                    sp.InsertArrayElementAtIndex(sp.arraySize);
                    var p = sp.GetArrayElementAtIndex(sp.arraySize - 1);
                    p.FindPropertyRelative("m_Name").stringValue = property.name;
                    p.FindPropertyRelative("m_Type").intValue = (int)property.type;
                }
                sp.serializedObject.ApplyModifiedProperties();
            });
            }
        }

        //################################
        // Constant or Static Members.
        //################################
        static readonly GUIContent s_ContentParticleMaterial = new GUIContent("Particle Material", "The material for rendering particles");
        static readonly GUIContent s_ContentTrailMaterial = new GUIContent("Trail Material", "The material for rendering particle trails");
        static readonly List<ParticleSystem> s_ParticleSystems = new List<ParticleSystem>();
        static readonly Color s_GizmoColor = new Color(1f, 0.7f, 0.7f, 0.9f);

        static readonly List<string> s_MaskablePropertyNames = new List<string>()
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
            _spTrailParticle = serializedObject.FindProperty("m_TrailParticle");
            _spScale = serializedObject.FindProperty("m_Scale");
            _spIgnoreParent = serializedObject.FindProperty("m_IgnoreParent");
            _spAnimatableProperties = serializedObject.FindProperty("m_AnimatableProperties");
            _particles = targets.Cast<UIParticle>().ToArray();
            _shapeModuleUIs = null;

            var targetsGos = targets.Cast<UIParticle>().Select(x => x.gameObject).ToArray();
            _inspector = Resources.FindObjectsOfTypeAll<ParticleSystemInspector>()
                .FirstOrDefault(x => x.targets.Cast<ParticleSystem>().Select(x => x.gameObject).SequenceEqual(targetsGos));
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_spParticleSystem);
            EditorGUI.indentLevel++;
            var ps = _spParticleSystem.objectReferenceValue as ParticleSystem;
            if (ps)
            {
                var pr = ps.GetComponent<ParticleSystemRenderer>();
                var sp = new SerializedObject(pr).FindProperty("m_Materials");

                EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(0), s_ContentParticleMaterial);
                EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(1), s_ContentTrailMaterial);
                sp.serializedObject.ApplyModifiedProperties();

                if (!Application.isPlaying && pr.enabled)
                {
                    EditorGUILayout.HelpBox("UIParticles disable the RendererModule in ParticleSystem at runtime to prevent double rendering.", MessageType.Warning);
                }
            }
            EditorGUI.indentLevel--;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_spTrailParticle);
            EditorGUI.EndDisabledGroup();

            var current = target as UIParticle;

            EditorGUILayout.PropertyField(_spIgnoreParent);

            EditorGUI.BeginDisabledGroup(!current.isRoot);
            EditorGUILayout.PropertyField(_spScale);
            EditorGUI.EndDisabledGroup();

            // AnimatableProperties
            AnimatedPropertiesEditor.DrawAnimatableProperties(_spAnimatableProperties, current.material);

            current.GetComponentsInChildren<ParticleSystem>(true, s_ParticleSystems);
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

            if (current.maskable && current.material && current.material.shader)
            {
                var mat = current.material;
                var shader = mat.shader;
                foreach (var propName in s_MaskablePropertyNames)
                {
                    if (!mat.HasProperty(propName))
                    {
                        EditorGUILayout.HelpBox(string.Format("Shader {0} doesn't have '{1}' property. This graphic is not maskable.", shader.name, propName), MessageType.Warning);
                        break;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


        //################################
        // Private Members.
        //################################
        SerializedProperty _spParticleSystem;
        SerializedProperty _spTrailParticle;
        SerializedProperty _spScale;
        SerializedProperty _spIgnoreParent;
        SerializedProperty _spAnimatableProperties;
        UIParticle[] _particles;
        ShapeModuleUI[] _shapeModuleUIs;
        ParticleSystemInspector _inspector;

        void OnSceneGUI()
        {
            _shapeModuleUIs = _shapeModuleUIs ?? _inspector?.m_ParticleEffectUI?.m_Emitters?.SelectMany(x => x.m_Modules).OfType<ShapeModuleUI>()?.ToArray();
            if (_shapeModuleUIs == null || _shapeModuleUIs.Length == 0 || _shapeModuleUIs[0].GetParticleSystem() != (target as UIParticle).cachedParticleSystem)
                return;

            Action postAction = () => { };
            Color origin = ShapeModuleUI.s_GizmoColor.m_Color;
            Color originDark = ShapeModuleUI.s_GizmoColor.m_Color;
            ShapeModuleUI.s_GizmoColor.m_Color = s_GizmoColor;
            ShapeModuleUI.s_GizmoColor.m_OptionalDarkColor = s_GizmoColor;

            _particles
                .Distinct()
                .Select(x => new { canvas = x.canvas, ps = x.cachedParticleSystem, scale = x.scale })
                .Where(x => x.ps && x.canvas)
                .ToList()
                .ForEach(x =>
                {
                    var trans = x.ps.transform;
                    var hasChanged = trans.hasChanged;
                    var localScale = trans.localScale;
                    postAction += () => trans.localScale = localScale;
                    trans.localScale = Vector3.Scale(localScale, x.canvas.rootCanvas.transform.localScale * x.scale);
                });

            foreach (var ui in _shapeModuleUIs)
                ui.OnSceneViewGUI();

            postAction();
            ShapeModuleUI.s_GizmoColor.m_Color = origin;
            ShapeModuleUI.s_GizmoColor.m_OptionalDarkColor = originDark;
        }
    }
}
#endif // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.