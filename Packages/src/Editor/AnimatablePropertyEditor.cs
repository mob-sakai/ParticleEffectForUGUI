using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using ShaderPropertyType = Coffee.UIExtensions.AnimatableProperty.ShaderPropertyType;

namespace Coffee.UIExtensions
{
    internal static class AnimatablePropertyEditor
    {
        private static readonly GUIContent s_ContentNothing = new GUIContent("Nothing");
        private static readonly GUIContent s_ContentCustom = new GUIContent("Add Custom...");
        private static readonly List<string> s_ActiveNames = new List<string>();
        private static readonly StringBuilder s_Sb = new StringBuilder();
        private static readonly HashSet<string> s_Names = new HashSet<string>();
        private static ShaderProperty s_CustomProperty = new ShaderProperty("", ShaderPropertyType.None);
        private static bool s_ShowCustomProperty = false;

        private static string CollectActiveNames(SerializedProperty sp, List<string> result)
        {
            result.Clear();
            for (var i = 0; i < sp.arraySize; i++)
            {
                var spName = sp.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name");
                if (spName == null) continue;

                result.Add(spName.stringValue);
            }

            s_Sb.Length = 0;
            if (result.Count == 0)
            {
                s_Sb.Append("Nothing");
            }
            else
            {
                result.Aggregate(s_Sb, (a, b) =>
                {
                    s_Sb.Append(b);
                    return s_Sb.Append(", ");
                });
                s_Sb.Length -= 2;
            }

            return s_Sb.ToString();
        }

        public static void Draw(SerializedProperty sp, List<Material> mats)
        {
            var pos = EditorGUILayout.GetControlRect(true);
            var label = new GUIContent(sp.displayName, sp.tooltip);
            var rect = EditorGUI.PrefixLabel(pos, label);
            var text = sp.hasMultipleDifferentValues
                ? "-"
                : CollectActiveNames(sp, s_ActiveNames);

            if (GUI.Button(rect, text, EditorStyles.popup))
            {
                ShowMenu(sp, mats);
            }

            if (s_ShowCustomProperty)
            {
                DrawCustomProperty(sp, ref s_CustomProperty);
            }
        }

        private static void ShowMenu(SerializedProperty sp, List<Material> mats)
        {
            var gm = new GenericMenu();
            gm.AddItem(s_ContentNothing, s_ActiveNames.Count == 0, x =>
            {
                var current = (SerializedProperty)x;
                current.ClearArray();
                current.serializedObject.ApplyModifiedProperties();
            }, sp);

            gm.AddItem(s_ContentCustom, s_ShowCustomProperty, () =>
            {
                s_ShowCustomProperty = !s_ShowCustomProperty;
                s_CustomProperty.Reset();
            });
            gm.AddSeparator("");

            if (!sp.hasMultipleDifferentValues)
            {
                for (var i = 0; i < sp.arraySize; i++)
                {
                    var p = sp.GetArrayElementAtIndex(i);
                    var name = p.FindPropertyRelative("m_Name").stringValue;
                    var type = (ShaderPropertyType)p.FindPropertyRelative("m_Type").intValue;
                    AddMenu(gm, sp, new ShaderProperty(name, type), false);
                }
            }

            s_Names.Clear();
            for (var j = 0; j < mats.Count; j++)
            {
                var mat = mats[j];
                if (mat == null || mat.shader == null) continue;

#if UNITY_6000_5_OR_NEWER
                for (var i = 0; i < mat.shader.GetPropertyCount(); i++)
#else
                for (var i = 0; i < ShaderUtil.GetPropertyCount(mat.shader); i++)
#endif
                {
#if UNITY_6000_5_OR_NEWER
                    var name = mat.shader.GetPropertyName(i);
                    var type = (ShaderPropertyType)mat.shader.GetPropertyType(i);
#else
                    var name = ShaderUtil.GetPropertyName(mat.shader, i);
                    var type = (ShaderPropertyType)ShaderUtil.GetPropertyType(mat.shader, i);
#endif
                    if (!s_Names.Add(name)) continue;

                    AddMenu(gm, sp, new ShaderProperty(name, type), true);

                    if (type != ShaderPropertyType.Texture) continue;

                    AddMenu(gm, sp, new ShaderProperty($"{name}_ST"), true);
                    AddMenu(gm, sp, new ShaderProperty($"{name}_HDR"), true);
                    AddMenu(gm, sp, new ShaderProperty($"{name}_TexelSize"), true);
                }
            }

            gm.ShowAsContext();
        }

        private static void AddMenu(GenericMenu menu, SerializedProperty sp, ShaderProperty prop, bool add)
        {
            if (add && s_ActiveNames.Contains(prop.name)) return;

            var label = new GUIContent($"{prop.name} ({prop.type})");
            menu.AddItem(label, s_ActiveNames.Contains(prop.name), () => AddProp(sp, prop));
        }

        private static void DrawCustomProperty(SerializedProperty sp, ref ShaderProperty prop)
        {
            var r = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight + 8);
            r.xMin += 60;
            GUI.Label(r, (Texture)null, EditorStyles.helpBox);

            r = new Rect(r.x + 4, r.y + 4, r.width - 8 - 100 - 16, r.height - 8);
            prop.name = EditorGUI.TextField(r, prop.name);
            r.x += r.width + 2;
            r.width = 100 - 2;
            prop.type = (ShaderPropertyType)EditorGUI.EnumPopup(r, prop.type);
            r.x += r.width;
            r.width = 16;

            EditorGUI.BeginDisabledGroup(!prop.IsValid(s_ActiveNames));
            if (GUI.Button(r, EditorGUIUtility.IconContent("Toolbar Plus"), EditorStyles.label))
            {
                GUI.FocusControl("");
                AddProp(sp, prop);
                prop.Reset();
            }

            EditorGUI.EndDisabledGroup();
        }

        private static void AddProp(SerializedProperty sp, ShaderProperty prop)
        {
            var index = s_ActiveNames.IndexOf(prop.name);
            if (0 <= index)
            {
                sp.DeleteArrayElementAtIndex(index);
            }
            else
            {
                sp.InsertArrayElementAtIndex(sp.arraySize);
                var p = sp.GetArrayElementAtIndex(sp.arraySize - 1);
                p.FindPropertyRelative("m_Name").stringValue = prop.name;
                p.FindPropertyRelative("m_Type").intValue = (int)prop.type;
            }

            sp.serializedObject.ApplyModifiedProperties();
        }

        private struct ShaderProperty
        {
            public string name;
            public ShaderPropertyType type;

            public ShaderProperty(string name)
            {
                this.name = name;
                type = ShaderPropertyType.Vector;
            }

            public ShaderProperty(string name, ShaderPropertyType type)
            {
                this.name = name;
                this.type = type;
            }

            public void Reset()
            {
                name = "";
                type = ShaderPropertyType.None;
            }

            public bool IsValid(List<string> activeNames)
            {
                if (string.IsNullOrEmpty(name)) return false;
                if (type == ShaderPropertyType.None) return false;
                if (activeNames.Contains(name)) return false;
                return true;
            }
        }
    }
}
