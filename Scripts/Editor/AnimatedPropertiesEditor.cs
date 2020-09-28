using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ShaderPropertyType = Coffee.UIExtensions.AnimatableProperty.ShaderPropertyType;

namespace Coffee.UIExtensions
{
    internal class AnimatedPropertiesEditor
    {
        private static readonly List<string> s_ActiveNames = new List<string>();
        private static readonly System.Text.StringBuilder s_Sb = new System.Text.StringBuilder();
        private static readonly HashSet<string> s_Names = new HashSet<string>();

        private string _name;
        private ShaderPropertyType _type;

        static string CollectActiveNames(SerializedProperty sp, List<string> result)
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

        public static void DrawAnimatableProperties(SerializedProperty sp, Material[] mats)
        {
            bool isClicked;
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                var r = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(true), new GUIContent(sp.displayName, sp.tooltip));
                var text = sp.hasMultipleDifferentValues ? "-" : CollectActiveNames(sp, s_ActiveNames);
                isClicked = GUI.Button(r, text, EditorStyles.popup);
            }

            if (!isClicked) return;

            var gm = new GenericMenu();
            gm.AddItem(new GUIContent("Nothing"), s_ActiveNames.Count == 0, () =>
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
                    var type = (ShaderPropertyType) p.FindPropertyRelative("m_Type").intValue;
                    AddMenu(gm, sp, new AnimatedPropertiesEditor {_name = name, _type = type}, false);
                }
            }

            s_Names.Clear();
            foreach (var mat in mats)
            {
                if (!mat || !mat.shader) continue;

                for (var i = 0; i < ShaderUtil.GetPropertyCount(mat.shader); i++)
                {
                    var pName = ShaderUtil.GetPropertyName(mat.shader, i);
                    var type = (ShaderPropertyType) ShaderUtil.GetPropertyType(mat.shader, i);
                    var name = string.Format("{0} ({1})", pName, type);
                    if (s_Names.Contains(name)) continue;
                    s_Names.Add(name);

                    AddMenu(gm, sp, new AnimatedPropertiesEditor {_name = pName, _type = type}, true);

                    if (type != ShaderPropertyType.Texture) continue;

                    AddMenu(gm, sp, new AnimatedPropertiesEditor {_name = pName + "_ST", _type = ShaderPropertyType.Vector}, true);
                    AddMenu(gm, sp, new AnimatedPropertiesEditor {_name = pName + "_HDR", _type = ShaderPropertyType.Vector}, true);
                    AddMenu(gm, sp, new AnimatedPropertiesEditor {_name = pName + "_TexelSize", _type = ShaderPropertyType.Vector}, true);
                }
            }

            gm.ShowAsContext();
        }

        private static void AddMenu(GenericMenu menu, SerializedProperty sp, AnimatedPropertiesEditor property, bool add)
        {
            if (add && s_ActiveNames.Contains(property._name)) return;

            menu.AddItem(new GUIContent(string.Format("{0} ({1})", property._name, property._type)), s_ActiveNames.Contains(property._name), () =>
            {
                var index = s_ActiveNames.IndexOf(property._name);
                if (0 <= index)
                {
                    sp.DeleteArrayElementAtIndex(index);
                }
                else
                {
                    sp.InsertArrayElementAtIndex(sp.arraySize);
                    var p = sp.GetArrayElementAtIndex(sp.arraySize - 1);
                    p.FindPropertyRelative("m_Name").stringValue = property._name;
                    p.FindPropertyRelative("m_Type").intValue = (int) property._type;
                }

                sp.serializedObject.ApplyModifiedProperties();
            });
        }
    }
}
