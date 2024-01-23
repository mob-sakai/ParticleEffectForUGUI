using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Coffee.NanoMonitor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CustomMonitorItem))]
    internal sealed class CustomMonitorItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect p, SerializedProperty property, GUIContent label)
        {
            var pos = new Rect(p.x, p.y + 18 * 0, p.width, 16);
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("m_Text"));
            pos.y += 18;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("m_Format"));
            pos.y += 18;
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("m_Arg0"));
            pos.y += 18;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("m_Arg1"));
            pos.y += 18;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("m_Arg2"));
            EditorGUI.indentLevel--;

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * 5;
        }
    }
#endif

    [Serializable]
    public class CustomMonitorItem
    {
        [SerializeField] private MonitorUI m_Text;
        [SerializeField] private string m_Format = "";
        [SerializeField] private NumericProperty m_Arg0;
        [SerializeField] private NumericProperty m_Arg1;
        [SerializeField] private NumericProperty m_Arg2;

        public void UpdateText()
        {
            if (m_Text)
            {
                m_Text.SetText(m_Format, m_Arg0.Get(), m_Arg1.Get(), m_Arg2.Get());
            }
        }
    }
}
