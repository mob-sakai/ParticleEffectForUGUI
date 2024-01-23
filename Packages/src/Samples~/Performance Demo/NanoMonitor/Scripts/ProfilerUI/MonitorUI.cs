using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.NanoMonitor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(MonitorUI))]
    public class MonitorTextEditor : Editor
    {
        private SerializedProperty _color;
        private SerializedProperty _font;
        private SerializedProperty _fontSize;
        private SerializedProperty _mode;
        private SerializedProperty _text;
        private SerializedProperty _textAnchor;

        private void OnEnable()
        {
            _mode = serializedObject.FindProperty("m_Mode");
            _text = serializedObject.FindProperty("m_Text");
            _color = serializedObject.FindProperty("m_Color");
            var fontData = serializedObject.FindProperty("m_FontData");
            _fontSize = fontData.FindPropertyRelative("m_FontSize");
            _font = fontData.FindPropertyRelative("m_Font");
            _textAnchor = serializedObject.FindProperty("m_TextAnchor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_mode);
            if ((MonitorUI.Mode)_mode.intValue == MonitorUI.Mode.Text)
            {
                EditorGUILayout.PropertyField(_text);
                EditorGUILayout.PropertyField(_fontSize);
                EditorGUILayout.PropertyField(_textAnchor);
                EditorGUILayout.PropertyField(_font);
            }

            EditorGUILayout.PropertyField(_color);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    public class MonitorUI : Text
    {
        public enum Mode
        {
            Text,
            Fill
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
        // Private Members.
        //################################
        private readonly StringBuilder _sb = new StringBuilder(64);


        //################################
        // Public Members.
        //################################
        public override string text
        {
            get
            {
                return _sb.ToString();
            }
            set
            {
                m_Text = value;
                if (_sb.IsEqual(m_Text)) return;

                _sb.Length = 0;
                _sb.Append(m_Text);
                SetVerticesDirty();
            }
        }

        public TextAnchor textAnchor
        {
            get
            {
                return m_TextAnchor;
            }
            set
            {
                if (m_TextAnchor == value) return;

                m_TextAnchor = value;
                SetVerticesDirty();
            }
        }

        public override bool raycastTarget
        {
            get
            {
                return m_Mode == Mode.Fill;
            }
            set { }
        }


        //################################
        // Unity Callbacks.
        //################################
        protected override void OnEnable()
        {
            RegisterDirtyMaterialCallback(UpdateFont);

            base.OnEnable();
            raycastTarget = false;
            maskable = false;
            _sb.Length = 0;
            _sb.Append(m_Text);
        }

        protected override void OnDisable()
        {
            UnregisterDirtyMaterialCallback(UpdateFont);

            base.OnDisable();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!_sb.IsEqual(m_Text))
            {
                _sb.Length = 0;
                _sb.Append(m_Text);
            }
        }
#endif

        public void SetText(string format, double arg0 = 0, double arg1 = 0, double arg2 = 0, double arg3 = 0)
        {
            _sb.Length = 0;
            _sb.AppendFormatNoAlloc(format, arg0, arg1, arg2, arg3);
            SetVerticesDirty();
        }

        public void SetText(StringBuilder builder)
        {
            _sb.Length = 0;
            _sb.Append(builder);
            SetVerticesDirty();
        }

        private void UpdateFont()
        {
            var fontData = FixedFont.GetOrCreate(font);
            if (fontData != null)
            {
                fontData.Invalidate();
                fontData.UpdateFont();
            }
        }

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

            var scale = (float)fontSize / fontData.fontSize;
            float offset = 0;
            switch (textAnchor)
            {
                case TextAnchor.Left:
                    offset = rectTransform.rect.xMin;
                    break;
                case TextAnchor.Center:
                    for (var i = 0; i < _sb.Length; i++)
                    {
                        offset = fontData.Layout(_sb[i], offset, scale);
                    }

                    offset = -offset / 2;
                    break;
                case TextAnchor.Right:
                    for (var i = 0; i < _sb.Length; i++)
                    {
                        offset = fontData.Layout(_sb[i], offset, scale);
                    }

                    offset = rectTransform.rect.xMax - offset;
                    break;
            }

            for (var i = 0; i < _sb.Length; i++)
            {
                offset = fontData.Append(toFill, _sb[i], offset, scale, color);
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
