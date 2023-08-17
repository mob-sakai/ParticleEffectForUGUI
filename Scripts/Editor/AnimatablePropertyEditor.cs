using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
    internal static class AnimatablePropertyEditor
    {
        private static readonly GUIContent s_ContentNothing = new GUIContent("Nothing");
        private static readonly List<string> s_ActiveNames = new List<string>();
        private static readonly StringBuilder s_Sb = new StringBuilder();
        private static readonly HashSet<string> s_Names = new HashSet<string>();

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
                result.Aggregate(s_Sb, (a, b) => s_Sb.AppendFormat("{0}, ", b));
                s_Sb.Length -= 2;
            }

            return s_Sb.ToString();
        }

        public static void Draw(SerializedProperty sp, Material[] mats)
        {
            bool isClicked;
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                var pos = EditorGUILayout.GetControlRect(true);
                var label = new GUIContent(sp.displayName, sp.tooltip);
                var rect = EditorGUI.PrefixLabel(pos, label);
                var text = sp.hasMultipleDifferentValues
                    ? "-"
                    : CollectActiveNames(sp, s_ActiveNames);
                isClicked = GUI.Button(rect, text, EditorStyles.popup);
            }

            if (!isClicked) return;

            var gm = new GenericMenu();
            gm.AddItem(s_ContentNothing, s_ActiveNames.Count == 0, () =>
            {
                sp.ClearArray();
                sp.serializedObject.ApplyModifiedProperties();
            });

            if (!sp.hasMultipleDifferentValues)
            {
                for (var i = 0; i < sp.arraySize; i++)
                {
                    var p = sp.GetArrayElementAtIndex(i);
                    var name = p.FindPropertyRelative("m_Name").stringValue;
                    var type = (AnimatableProperty.ShaderPropertyType)p.FindPropertyRelative("m_Type").intValue;
                    AddMenu(gm, sp, new ShaderProperty(name, type), false);
                }
            }

            s_Names.Clear();
            for (var j = 0; j < mats.Length; j++)
            {
                var mat = mats[j];
                if (!mat || !mat.shader) continue;

                for (var i = 0; i < ShaderUtil.GetPropertyCount(mat.shader); i++)
                {
                    var name = ShaderUtil.GetPropertyName(mat.shader, i);
                    var type = (AnimatableProperty.ShaderPropertyType)ShaderUtil.GetPropertyType(mat.shader, i);
                    if (s_Names.Contains(name)) continue;
                    s_Names.Add(name);

                    AddMenu(gm, sp, new ShaderProperty(name, type), true);

                    if (type != AnimatableProperty.ShaderPropertyType.Texture) continue;

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
            menu.AddItem(label, s_ActiveNames.Contains(prop.name), () =>
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
            });
        }

        private struct ShaderProperty
        {
            public readonly string name;
            public readonly AnimatableProperty.ShaderPropertyType type;

            public ShaderProperty(string name)
            {
                this.name = name;
                type = AnimatableProperty.ShaderPropertyType.Vector;
            }

            public ShaderProperty(string name, AnimatableProperty.ShaderPropertyType type)
            {
                this.name = name;
                this.type = type;
            }
        }
    }
}
