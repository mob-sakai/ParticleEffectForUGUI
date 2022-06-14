using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.NanoMonitor{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MonitorUI))]
    public class MonitorTextEditor : Editor
    {
        private SerializedProperty m_Mode;
        private SerializedProperty m_TextAnchor;
        private SerializedProperty m_FontSize;
        private SerializedProperty m_Font;
        private SerializedProperty m_Text;
        private SerializedProperty m_Color;

        private void OnEnable()
        {
            m_Mode = serializedObject.FindProperty("m_Mode");

            m_Text = serializedObject.FindProperty("m_Text");
            m_Color = serializedObject.FindProperty("m_Color");
            var fontData = serializedObject.FindProperty("m_FontData");
            m_FontSize = fontData.FindPropertyRelative("m_FontSize");
            m_Font = fontData.FindPropertyRelative("m_Font");
            m_TextAnchor = serializedObject.FindProperty("m_TextAnchor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Mode);
            if ((MonitorUI.Mode) m_Mode.intValue == MonitorUI.Mode.Text)
            {
                EditorGUILayout.PropertyField(m_Text);
                EditorGUILayout.PropertyField(m_FontSize);
                EditorGUILayout.PropertyField(m_TextAnchor);

                //EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(m_Font);
                //EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.PropertyField(m_Color);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    public class MonitorUI : Text
    {
        public enum Mode
        {
            Text,
            Fill,
        }

        public enum TextAnchor
        {
            Left,
            Center,
            Right
        }


        //################################
        // Serialize Members.
        //################################
        [SerializeField] private Mode m_Mode = Mode.Text;
        [SerializeField] private TextAnchor m_TextAnchor;


        //################################
        // Public Members.
        //################################
        public override string text
        {
            get => m_StringBuilder.ToString();
            set
            {
                m_Text = value;
                if (m_StringBuilder.IsEqual(m_Text)) return;

                m_StringBuilder.Length = 0;
                m_StringBuilder.Append(m_Text);
                SetVerticesDirty();
            }
        }

        public TextAnchor textAnchor
        {
            get => m_TextAnchor;
            set
            {
                if (m_TextAnchor == value) return;

                m_TextAnchor = value;
                SetVerticesDirty();
            }
        }

        public override bool raycastTarget
        {
            get => m_Mode == Mode.Fill;
            set { }
        }

        public void SetText(string format, double arg0 = 0, double arg1 = 0, double arg2 = 0, double arg3 = 0)
        {
            m_StringBuilder.Length = 0;
            m_StringBuilder.AppendFormatNoAlloc(format, arg0, arg1, arg2, arg3);
            SetVerticesDirty();
        }

        public void SetText(StringBuilder builder)
        {
            m_StringBuilder.Length = 0;
            m_StringBuilder.Append(builder);
            SetVerticesDirty();
        }


        //################################
        // Private Members.
        //################################
        private readonly StringBuilder m_StringBuilder = new StringBuilder(64);

        private void UpdateFont()
        {
            //var globalFont = NNanoMonitorttings.Instance.font;
            //if (globalFont)
            //{
            //    font = globalFont;
            //}

            var fontData = FixedFont.GetOrCreate(font);
            if (fontData != null)
            {
                fontData.Invalidate();
                fontData.UpdateFont();
            }
        }


        //################################
        // Unity Callbacks.
        //################################
        protected override void OnEnable()
        {
            //NaNanoMonitortings.Instance.onFontChanged += UpdateFont;
            RegisterDirtyMaterialCallback(UpdateFont);

            base.OnEnable();
            raycastTarget = false;
            maskable = false;
            m_StringBuilder.Length = 0;
            m_StringBuilder.Append(m_Text);
        }

        protected override void OnDisable()
        {
            //NanNanoMonitorings.Instance.onFontChanged -= UpdateFont;
            UnregisterDirtyMaterialCallback(UpdateFont);

            base.OnDisable();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!m_StringBuilder.IsEqual(m_Text))
            {
                m_StringBuilder.Length = 0;
                m_StringBuilder.Append(m_Text);
            }
        }
#endif

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();

            var fontData = FixedFont.GetOrCreate(font);
            if (fontData == null) return;

            fontData.UpdateFont();

            if (m_Mode == Mode.Fill)
            {
                fontData.Fill(toFill, color, rectTransform);
                return;
            }

            var scale = (float) fontSize / fontData.fontSize;
            float offset = 0;
            switch (textAnchor)
            {
                case TextAnchor.Left:
                    offset = rectTransform.rect.xMin;
                    break;
                case TextAnchor.Center:
                    for (var i = 0; i < m_StringBuilder.Length; i++)
                        offset = fontData.Layout(m_StringBuilder[i], offset, scale);
                    offset = -offset / 2;
                    break;
                case TextAnchor.Right:
                    for (var i = 0; i < m_StringBuilder.Length; i++)
                        offset = fontData.Layout(m_StringBuilder[i], offset, scale);
                    offset = rectTransform.rect.xMax - offset;
                    break;
            }

            for (var i = 0; i < m_StringBuilder.Length; i++)
            {
                offset = fontData.Append(toFill, m_StringBuilder[i], offset, scale, color);
            }
        }

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();

            var fontData = FixedFont.GetOrCreate(font);
            if (fontData != null)
            {
                fontData.UpdateFont();
            }
        }
    }
}
