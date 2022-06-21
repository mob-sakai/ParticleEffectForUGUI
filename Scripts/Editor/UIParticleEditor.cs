#if UNITY_2019_3_11 || UNITY_2019_3_12 || UNITY_2019_3_13 || UNITY_2019_3_14 || UNITY_2019_3_15 || UNITY_2019_4_OR_NEWER
#define SERIALIZE_FIELD_MASKABLE
#endif
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine.UI;
using System;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.Overlays;
#else
using System.Reflection;
#endif

namespace Coffee.UIExtensions
{
    [CustomEditor(typeof(UIParticle))]
    [CanEditMultipleObjects]
    internal class UIParticleEditor : GraphicEditor
    {
#if UNITY_2021_2_OR_NEWER
#if UNITY_2022_1_OR_NEWER
        [Overlay(typeof(SceneView), "Scene View/UI Particles", "UI Particles", true, defaultDockPosition = DockPosition.Bottom, defaultDockZone = DockZone.Floating, defaultLayout = Layout.Panel)]
#else
        [Overlay(typeof(SceneView), "Scene View/UI Particles", "UI Particles", true)]
#endif
        private class UIParticleOverlay : IMGUIOverlay, ITransientOverlay
        {
            public bool visible => s_SerializedObject != null;

            public override void OnGUI()
            {
                if (visible)
                {
                    WindowFunction(null, null);
                }
            }
        }
#endif

        //################################
        // Constant or Static Members.
        //################################
        private static readonly GUIContent s_ContentRenderingOrder = new GUIContent("Rendering Order");
        private static readonly GUIContent s_ContentRefresh = new GUIContent("Refresh");
        private static readonly GUIContent s_ContentFix = new GUIContent("Fix");
        private static readonly GUIContent s_ContentMaterial = new GUIContent("Material");
        private static readonly GUIContent s_ContentTrailMaterial = new GUIContent("Trail Material");
        private static readonly GUIContent s_Content3D = new GUIContent("3D");
        private static readonly GUIContent s_ContentRandom = new GUIContent("Random");
        private static readonly GUIContent s_ContentScale = new GUIContent("Scale");
        private static SerializedObject s_SerializedObject;

#if !SERIALIZE_FIELD_MASKABLE
        private SerializedProperty m_Maskable;
#endif
        private SerializedProperty m_Scale3D;
        private SerializedProperty m_AnimatableProperties;
        private SerializedProperty m_MeshSharing;
        private SerializedProperty m_GroupId;
        private SerializedProperty m_GroupMaxId;


        private ReorderableList _ro;
        static private bool _xyzMode;
        private bool _showMax;

        private static readonly HashSet<Shader> s_Shaders = new HashSet<Shader>();
        private static readonly List<string> s_MaskablePropertyNames = new List<string>
        {
            "_Stencil",
            "_StencilComp",
            "_StencilOp",
            "_StencilWriteMask",
            "_StencilReadMask",
            "_ColorMask",
        };


        [InitializeOnLoadMethod]
        static void Init()
        {
#if !UNITY_2021_2_OR_NEWER
            // static void Window(GUIContent title, WindowFunction sceneViewFunc, int order, UnityEngine.Object target, WindowDisplayOption option)
            var miSceneViewOverlayWindow = Type.GetType("UnityEditor.SceneViewOverlay, UnityEditor")
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(x => x.Name == "Window" && x.GetParameters().Length == 5);
            var windowFunction = (Action<UnityEngine.Object, SceneView>)WindowFunction;
            var windowFunctionType = Type.GetType("UnityEditor.SceneViewOverlay+WindowFunction, UnityEditor");
            var windowFunctionDelegate = Delegate.CreateDelegate(windowFunctionType, windowFunction.Method);
            var windowTitle = new GUIContent(ObjectNames.NicifyVariableName(typeof(UIParticle).Name));
            var sceneViewArgs = new object[] { windowTitle, windowFunctionDelegate, 599, null, 2 };
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += _ => miSceneViewOverlayWindow.Invoke(null, sceneViewArgs);
#else
            SceneView.onSceneGUIDelegate += _ =>
#endif
            {
                if (s_SerializedObject != null)
                {
                    miSceneViewOverlayWindow.Invoke(null, sceneViewArgs);
                }
            };
#endif

            Func<SerializedObject> createSerializeObject = () =>
            {
                var uiParticles = Selection.gameObjects
                        .Select(x => x.GetComponent<ParticleSystem>())
                        .Where(x => x)
                        .Select(x => x.GetComponentInParent<UIParticle>())
                        .Where(x => x)
                        .Concat(
                            Selection.gameObjects
                                .Select(x => x.GetComponent<UIParticle>())
                                .Where(x => x)
                        )
                        .Distinct()
                        .ToArray();
                return uiParticles.Any() ? new SerializedObject(uiParticles) : null;
            };

            s_SerializedObject = createSerializeObject();
            Selection.selectionChanged += () => s_SerializedObject = createSerializeObject();
        }

        //################################
        // Public/Protected Members.
        //################################
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            m_Maskable = serializedObject.FindProperty("m_Maskable");
            m_Scale3D = serializedObject.FindProperty("m_Scale3D");
            m_AnimatableProperties = serializedObject.FindProperty("m_AnimatableProperties");
            m_MeshSharing = serializedObject.FindProperty("m_MeshSharing");
            m_GroupId = serializedObject.FindProperty("m_GroupId");
            m_GroupMaxId = serializedObject.FindProperty("m_GroupMaxId");

            var sp = serializedObject.FindProperty("m_Particles");
            _ro = new ReorderableList(sp.serializedObject, sp, true, true, true, true);
            _ro.elementHeight = EditorGUIUtility.singleLineHeight * 3 + 4;
            _ro.elementHeightCallback = _ => 3 * (EditorGUIUtility.singleLineHeight + 2);
            _ro.drawElementCallback = (rect, index, active, focused) =>
            {
                EditorGUI.BeginDisabledGroup(sp.hasMultipleDifferentValues);
                rect.y += 1;
                rect.height = EditorGUIUtility.singleLineHeight;
                var p = sp.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(rect, p, GUIContent.none);
                rect.x += 15;
                rect.width -= 15;
                var ps = p.objectReferenceValue as ParticleSystem;
                var materials = ps
                    ? new SerializedObject(ps.GetComponent<ParticleSystemRenderer>()).FindProperty("m_Materials")
                    : null;
                rect.y += rect.height + 1;
                MaterialField(rect, s_ContentMaterial, materials, 0);
                rect.y += rect.height + 1;
                MaterialField(rect, s_ContentTrailMaterial, materials, 1);
                EditorGUI.EndDisabledGroup();
                if (materials != null)
                {
                    materials.serializedObject.ApplyModifiedProperties();
                }
            };
            _ro.drawHeaderCallback = rect =>
            {
#if !UNITY_2019_3_OR_NEWER
                rect.y -= 1;
#endif
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 150, rect.height), s_ContentRenderingOrder);

                if (GUI.Button(new Rect(rect.width - 35, rect.y, 60, rect.height), s_ContentRefresh, EditorStyles.miniButton))
                {
                    foreach (UIParticle t in targets)
                    {
                        t.RefreshParticles();
                    }
                }
            };
            _ro.onReorderCallback = _ =>
            {
                foreach (UIParticle t in targets)
                {
                    t.RefreshParticles(t.particles);
                }
            };

            // On select UIParticle, refresh particles.
            if (!Application.isPlaying)
            {
                foreach (UIParticle t in targets)
                {
                    t.RefreshParticles(t.particles);
                }
            }
        }

        private static void MaterialField(Rect rect, GUIContent label, SerializedProperty sp, int index)
        {
            if (sp == null || sp.arraySize <= index)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(rect, label, null, typeof(Material), true);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUI.PropertyField(rect, sp.GetArrayElementAtIndex(index), label);
            }
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var current = target as UIParticle;
            if (current == null) return;

            serializedObject.Update();

            // Maskable
            EditorGUILayout.PropertyField(m_Maskable);

            // Scale
            EditorGUI.BeginDisabledGroup(!m_MeshSharing.hasMultipleDifferentValues && m_MeshSharing.intValue == 4);
            _xyzMode = DrawFloatOrVector3Field(m_Scale3D, _xyzMode);
            EditorGUI.EndDisabledGroup();

            // AnimatableProperties
            var mats = current.particles
                .Where(x => x)
                .Select(x => x.GetComponent<ParticleSystemRenderer>().sharedMaterial)
                .Where(x => x)
                .ToArray();

            // Animated properties
            AnimatedPropertiesEditor.DrawAnimatableProperties(m_AnimatableProperties, mats);

            // Mesh sharing
            EditorGUI.BeginChangeCheck();
            _showMax = DrawMeshSharing(m_MeshSharing, m_GroupId, m_GroupMaxId, _showMax);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                foreach (var uip in targets.OfType<UIParticle>())
                {
                    uip.ResetGroupId();
                }
            }

            // Target ParticleSystems.
            _ro.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            // Does the shader support UI masks?
            if (current.maskable && current.GetComponentInParent<Mask>())
            {
                foreach (var mat in current.materials)
                {
                    if (!mat || !mat.shader) continue;
                    var shader = mat.shader;
                    if (s_Shaders.Contains(shader)) continue;
                    s_Shaders.Add(shader);
                    foreach (var propName in s_MaskablePropertyNames)
                    {
                        if (mat.HasProperty(propName)) continue;

                        EditorGUILayout.HelpBox(string.Format("Shader '{0}' doesn't have '{1}' property. This graphic cannot be masked.", shader.name, propName), MessageType.Warning);
                        break;
                    }
                }
            }
            s_Shaders.Clear();

            // UIParticle for trail should be removed.
            if (FixButton(current.m_IsTrail, "This UIParticle component should be removed. The UIParticle for trails is no longer needed."))
            {
                DestroyUIParticle(current);
            }

            // #203: When using linear color space, the particle colors are not output correctly.
            // To fix, set 'Apply Active Color Space' in renderer module to false.
            var allPsRenderers = targets.OfType<UIParticle>()
                .SelectMany(x => x.particles)
                .Where(x => x)
                .Select(x => x.GetComponent<ParticleSystemRenderer>())
                .ToArray();
            if (0 < allPsRenderers.Length)
            {
                var so = new SerializedObject(allPsRenderers);
                var sp = so.FindProperty("m_ApplyActiveColorSpace");
                if (FixButton(sp.boolValue || sp.hasMultipleDifferentValues, "When using linear color space, the particle colors are not output correctly.\nTo fix, set 'Apply Active Color Space' in renderer module to false."))
                {
                    sp.boolValue = false;
                    so.ApplyModifiedProperties();
                }
            }
        }

        private static bool DrawMeshSharing(SerializedProperty spMeshSharing, SerializedProperty spGroupId, SerializedProperty spGroupMaxId, bool showMax)
        {
            showMax |= spGroupId.intValue != spGroupMaxId.intValue ||
                       spGroupId.hasMultipleDifferentValues ||
                       spGroupMaxId.hasMultipleDifferentValues;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(spMeshSharing);

            EditorGUI.BeginChangeCheck();
            showMax = GUILayout.Toggle(showMax, s_ContentRandom, EditorStyles.miniButton, GUILayout.Width(60));
            if (EditorGUI.EndChangeCheck() && !showMax)
                spGroupMaxId.intValue = spGroupId.intValue;
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(spMeshSharing.intValue == 0);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spGroupId);
            if (showMax)
            {
                EditorGUILayout.PropertyField(spGroupMaxId);
            }
            else if (spMeshSharing.intValue == 1 || spMeshSharing.intValue == 4)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Primary", UIParticleUpdater.GetPrimary(spGroupId.intValue), typeof(UIParticle), false);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();

            return showMax;
        }

        private static void WindowFunction(UnityEngine.Object target, SceneView sceneView)
        {
            try
            {
                if (s_SerializedObject.targetObjects.Any(x => !x)) return;

                s_SerializedObject.Update();
                GUILayout.BeginHorizontal(GUILayout.Width(220f));
                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 60;
                _xyzMode = DrawFloatOrVector3Field(s_SerializedObject.FindProperty("m_Scale3D"), _xyzMode);
                EditorGUIUtility.labelWidth = labelWidth;
                GUILayout.EndHorizontal();
                s_SerializedObject.ApplyModifiedProperties();
            }
            catch
            {
            }
        }

        private void DestroyUIParticle(UIParticle p, bool ignoreCurrent = false)
        {
            if (!p || ignoreCurrent && target == p) return;

            var cr = p.canvasRenderer;
            DestroyImmediate(p);
            DestroyImmediate(cr);

#if UNITY_2021_2_OR_NEWER
            var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#elif UNITY_2018_3_OR_NEWER
            var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
#if UNITY_2018_3_OR_NEWER
            if (stage != null && stage.scene.isLoaded)
            {
#if UNITY_2020_1_OR_NEWER
                string prefabAssetPath = stage.assetPath;
#else
                string prefabAssetPath = stage.prefabAssetPath;
#endif
                PrefabUtility.SaveAsPrefabAsset(stage.prefabContentsRoot, prefabAssetPath);
            }
#endif
        }

        private static bool FixButton(bool show, string text)
        {
            if (!show) return false;
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
            {
                EditorGUILayout.HelpBox(text, MessageType.Warning, true);
                using (new EditorGUILayout.VerticalScope())
                {
                    return GUILayout.Button(s_ContentFix, GUILayout.Width(30));
                }
            }
        }

        private static bool DrawFloatOrVector3Field(SerializedProperty sp, bool showXyz)
        {
            var x = sp.FindPropertyRelative("x");
            var y = sp.FindPropertyRelative("y");
            var z = sp.FindPropertyRelative("z");

            showXyz |= !Mathf.Approximately(x.floatValue, y.floatValue) ||
                       !Mathf.Approximately(y.floatValue, z.floatValue) ||
                       y.hasMultipleDifferentValues ||
                       z.hasMultipleDifferentValues;

            EditorGUILayout.BeginHorizontal();
            if (showXyz)
            {
                EditorGUILayout.PropertyField(sp);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(x, s_ContentScale);
                if (EditorGUI.EndChangeCheck())
                {
                    y.floatValue = z.floatValue = x.floatValue;
                }
            }

            EditorGUI.BeginChangeCheck();
            showXyz = GUILayout.Toggle(showXyz, s_Content3D, EditorStyles.miniButton, GUILayout.Width(30));
            if (EditorGUI.EndChangeCheck() && !showXyz)
                z.floatValue = y.floatValue = x.floatValue;
            EditorGUILayout.EndHorizontal();

            return showXyz;
        }
    }
}
