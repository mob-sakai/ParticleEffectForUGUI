using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Coffee.NanoMonitor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(NanoMonitor))]
    internal class NanoMonitorEditor : Editor
    {
        private ReorderableList _monitorItemList;

        private void OnEnable()
        {
            var items = serializedObject.FindProperty("m_CustomMonitorItems");
            _monitorItemList = new ReorderableList(serializedObject, items)
            {
                draggable = false,
                drawHeaderCallback = r =>
                {
                    EditorGUI.LabelField(r, new GUIContent("Custom Monitor Items"));
                },
                drawElementCallback = (r, i, _, __) =>
                {
                    EditorGUI.LabelField(new Rect(r.x, r.y, r.width, r.height - 2), GUIContent.none,
                        EditorStyles.textArea);
                    var labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 80;
                    EditorGUI.PropertyField(new Rect(r.x + 2, r.y + 3, r.width - 4, r.height - 4),
                        items.GetArrayElementAtIndex(i), true);
                    EditorGUIUtility.labelWidth = labelWidth;
                },
                elementHeightCallback = i => EditorGUI.GetPropertyHeight(items.GetArrayElementAtIndex(i)) + 6
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _monitorItemList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif


    [DisallowMultipleComponent]
    public sealed class NanoMonitor : MonoBehaviour
    {
        //################################
        // Serialize Members.
        //################################
        // Settings
        [Header("Settings")]
        [SerializeField]
        [Range(0.01f, 2f)]
        private float m_Interval = 1f;

        [SerializeField]
        [Range(0, 3)]
        private int m_Precision = 2;

        [SerializeField]
        private Font m_Font;

        // Foldout
        [Header("Foldout")]
        [SerializeField]
        private bool m_Opened;

        [FormerlySerializedAs("m_Collapse")]
        [SerializeField]
        private GameObject m_FoldoutObject;

        [SerializeField]
        private Button m_OpenButton;

        [SerializeField]
        private Button m_CloseButton;

        // View
        [Header("View")]
        [SerializeField]
        private MonitorUI m_Fps;

        [SerializeField]
        private MonitorUI m_Gc;

        [SerializeField]
        private MonitorUI m_MonoUsage;

        [SerializeField]
        private MonitorUI m_UnityUsage;

        [HideInInspector]
        [SerializeField]
        private CustomMonitorItem[] m_CustomMonitorItems = new CustomMonitorItem[0];


        //################################
        // Private Members.
        //################################
        private double _elapsed;
        private double _fpsElapsed;
        private int _frames;

        private void Update()
        {
            _frames++;
            _elapsed += Time.unscaledDeltaTime;
            _fpsElapsed += Time.unscaledDeltaTime;
            if (_elapsed < m_Interval) return;

            if (m_Fps)
            {
                m_Fps.SetText("FPS: {0}", (int)(_frames / _fpsElapsed));
            }

            if (m_Gc)
            {
                m_Gc.SetText("GC: {0}", GC.CollectionCount(0));
            }

            if (m_MonoUsage)
            {
                var monoUsed = (Profiler.GetMonoUsedSizeLong() >> 10) / 1024f;
                var monoTotal = (Profiler.GetMonoHeapSizeLong() >> 10) / 1024f;
                if (m_Precision == 3)
                {
                    m_MonoUsage.SetText("Mono: {0:N3}/{1:N3}MB", monoUsed, monoTotal);
                }
                else if (m_Precision == 2)
                {
                    m_MonoUsage.SetText("Mono: {0:N2}/{1:N2}MB", monoUsed, monoTotal);
                }
                else if (m_Precision == 1)
                {
                    m_MonoUsage.SetText("Mono: {0:N1}/{1:N1}MB", monoUsed, monoTotal);
                }
                else
                {
                    m_MonoUsage.SetText("Mono: {0:N0}/{1:N0}MB", monoUsed, monoTotal);
                }
            }

            if (m_UnityUsage)
            {
                var unityUsed = (Profiler.GetTotalAllocatedMemoryLong() >> 10) / 1024f;
                var unityTotal = (Profiler.GetTotalReservedMemoryLong() >> 10) / 1024f;
                if (m_Precision == 3)
                {
                    m_UnityUsage.SetText("Unity: {0:N3}/{1:N3}MB", unityUsed, unityTotal);
                }
                else if (m_Precision == 2)
                {
                    m_UnityUsage.SetText("Unity: {0:N2}/{1:N2}MB", unityUsed, unityTotal);
                }
                else if (m_Precision == 1)
                {
                    m_UnityUsage.SetText("Unity: {0:N1}/{1:N1}MB", unityUsed, unityTotal);
                }
                else
                {
                    m_UnityUsage.SetText("Unity: {0:N0}/{1:N0}MB", unityUsed, unityTotal);
                }
            }

            foreach (var item in m_CustomMonitorItems)
            {
                item.UpdateText();
            }

            _frames = 0;
            _elapsed %= m_Interval;
            _fpsElapsed = 0;
        }


        //################################
        // Unity Callbacks.
        //################################
        private void OnEnable()
        {
            if (m_OpenButton)
            {
                m_OpenButton.onClick.AddListener(Open);
            }

            if (m_CloseButton)
            {
                m_CloseButton.onClick.AddListener(Close);
            }

            if (m_Opened)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        private void OnDisable()
        {
            if (m_OpenButton)
            {
                m_OpenButton.onClick.RemoveListener(Open);
            }

            if (m_CloseButton)
            {
                m_CloseButton.onClick.RemoveListener(Close);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_Font)
            {
                foreach (var ui in GetComponentsInChildren<MonitorUI>(true))
                {
                    ui.font = m_Font;
                }
            }

            if (m_Opened)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
#endif


        //################################
        // Public Members.
        //################################
        public void Clean()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        private void Open()
        {
            _frames = 0;
            _elapsed = m_Interval;
            _fpsElapsed = 0;

            if (m_FoldoutObject)
            {
                m_FoldoutObject.SetActive(true);
            }

            if (m_CloseButton)
            {
                m_CloseButton.gameObject.SetActive(true);
            }

            if (m_OpenButton)
            {
                m_OpenButton.gameObject.SetActive(false);
            }
        }

        private void Close()
        {
            if (m_FoldoutObject)
            {
                m_FoldoutObject.SetActive(false);
            }

            if (m_CloseButton)
            {
                m_CloseButton.gameObject.SetActive(false);
            }

            if (m_OpenButton)
            {
                m_OpenButton.gameObject.SetActive(true);
            }
        }
    }
}
