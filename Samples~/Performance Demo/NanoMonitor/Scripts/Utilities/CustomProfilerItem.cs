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
            EditorGUI.PropertyField(new Rect(p.x, p.y + 18 * 0, p.width, 16), property.FindPropertyRelative("m_Text"));
            EditorGUI.PropertyField(new Rect(p.x, p.y + 18 * 1, p.width, 16), property.FindPropertyRelative("m_Format"));
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(new Rect(p.x, p.y + 18 * 2, p.width, 16), property.FindPropertyRelative("m_Arg0"));
            EditorGUI.PropertyField(new Rect(p.x, p.y + 18 * 3, p.width, 16), property.FindPropertyRelative("m_Arg1"));
            EditorGUI.PropertyField(new Rect(p.x, p.y + 18 * 4, p.width, 16), property.FindPropertyRelative("m_Arg2"));
            EditorGUI.indentLevel--;

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * 5;
        }
    }
#endif

    [System.Serializable]
    public class CustomMonitorItem
    {
        [SerializeField] private MonitorUI m_Text = null;
        [SerializeField] private string m_Format = "";
        [SerializeField] private NumericProperty m_Arg0 = null;
        [SerializeField] private NumericProperty m_Arg1 = null;
        [SerializeField] private NumericProperty m_Arg2 = null;

        public void UpdateText()
        {
            if (m_Text)
                m_Text.SetText(m_Format, m_Arg0.Get(), m_Arg1.Get(), m_Arg2.Get());
        }
    }
}
