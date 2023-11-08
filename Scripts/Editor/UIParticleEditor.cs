#if UNITY_2021_2_OR_NEWER
using UnityEditor.Overlays;
#else
using System.Reflection;
#endif
#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#elif UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Coffee.UIParticleExtensions;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Coffee.UIExtensions
{
    [CustomEditor(typeof(UIParticle))]
    [CanEditMultipleObjects]
    internal class UIParticleEditor : GraphicEditor
    {
#if UNITY_2021_2_OR_NEWER
#if UNITY_2022_1_OR_NEWER
        [Overlay(typeof(SceneView), "Scene View/UI Particles", "UI Particles", true,
         defaultDockPosition = DockPosition.Bottom,
         defaultDockZone = DockZone.Floating,
         defaultLayout = Layout.Panel)]
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
        private static bool s_XYZMode;

        private SerializedProperty _maskable;
        private SerializedProperty _scale3D;
        private SerializedProperty _animatableProperties;
        private SerializedProperty _meshSharing;
        private SerializedProperty _groupId;
        private SerializedProperty _groupMaxId;
        private SerializedProperty _positionMode;
        private SerializedProperty _autoScalingMode;
        private ReorderableList _ro;
        private bool _showMax;

        private static readonly HashSet<Shader> s_Shaders = new HashSet<Shader>();
        private static readonly List<ParticleSystemVertexStream> s_Streams = new List<ParticleSystemVertexStream>();

        private static readonly List<string> s_MaskablePropertyNames = new List<string>
        {
            "_Stencil",
            "_StencilComp",
            "_StencilOp",
            "_StencilWriteMask",
            "_StencilReadMask",
            "_ColorMask"
        };

        [InitializeOnLoadMethod]
        private static void Init()
        {
#if !UNITY_2021_2_OR_NEWER
            var miSceneViewOverlayWindow = Type.GetType("UnityEditor.SceneViewOverlay, UnityEditor")
                ?.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(x => x.Name == "Window" && 5 <= x.GetParameters().Length);
            var windowFunction = (Action<Object, SceneView>)WindowFunction;
            var windowFunctionType = Type.GetType("UnityEditor.SceneViewOverlay+WindowFunction, UnityEditor");
            var windowFunctionDelegate = Delegate.CreateDelegate(windowFunctionType, windowFunction.Method);
            var windowTitle = new GUIContent(ObjectNames.NicifyVariableName(nameof(UIParticle)));
#if UNITY_2019_2_OR_NEWER
            //public static void Window(GUIContent title, WindowFunction sceneViewFunc, int order, Object target, WindowDisplayOption option, EditorWindow window = null)
            var sceneViewArgs = new object[] { windowTitle, windowFunctionDelegate, 599, null, 2, null };
#else
            //public static void Window(GUIContent title, WindowFunction sceneViewFunc, int order, Object target, WindowDisplayOption option)
            var sceneViewArgs = new object[] { windowTitle, windowFunctionDelegate, 599, null, 2 };
#endif

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += _ =>
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

            SerializedObject CreateSerializeObject()
            {
                var uiParticles = Selection.gameObjects.Select(x => x.GetComponent<ParticleSystem>())
                    .Where(x => x)
                    .Select(x => x.GetComponentInParent<UIParticle>(true))
                    .Where(x => x && x.canvas)
                    .Concat(Selection.gameObjects.Select(x => x.GetComponent<UIParticle>())
                        .Where(x => x && x.canvas))
                    .Distinct()
                    .ToArray();
                return 0 < uiParticles.Length ? new SerializedObject(uiParticles) : null;
            }

            s_SerializedObject = CreateSerializeObject();
            Selection.selectionChanged += () => s_SerializedObject = CreateSerializeObject();
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

            _maskable = serializedObject.FindProperty("m_Maskable");
            _scale3D = serializedObject.FindProperty("m_Scale3D");
            _animatableProperties = serializedObject.FindProperty("m_AnimatableProperties");
            _meshSharing = serializedObject.FindProperty("m_MeshSharing");
            _groupId = serializedObject.FindProperty("m_GroupId");
            _groupMaxId = serializedObject.FindProperty("m_GroupMaxId");
            _positionMode = serializedObject.FindProperty("m_PositionMode");
            _autoScalingMode = serializedObject.FindProperty("m_AutoScalingMode");

            var sp = serializedObject.FindProperty("m_Particles");
            _ro = new ReorderableList(sp.serializedObject, sp, true, true, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 3 + 4,
                elementHeightCallback = _ => 3 * (EditorGUIUtility.singleLineHeight + 2),
                drawElementCallback = (rect, index, _, __) =>
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
                    if (materials != null && materials.serializedObject.hasModifiedProperties)
                    {
                        materials.serializedObject.ApplyModifiedProperties();
                    }
                },
                drawHeaderCallback = rect =>
                {
#if !UNITY_2019_3_OR_NEWER
                    rect.y -= 1;
#endif
                    var pos = new Rect(rect.x, rect.y, 150, rect.height);
                    EditorGUI.LabelField(pos, s_ContentRenderingOrder);

                    pos = new Rect(rect.width - 35, rect.y, 60, rect.height);
                    if (GUI.Button(pos, s_ContentRefresh, EditorStyles.miniButton))
                    {
                        foreach (var uip in targets.OfType<UIParticle>())
                        {
                            uip.RefreshParticles();
                            EditorUtility.SetDirty(uip);
                        }
                    }
                }
            };

            // On select UIParticle, refresh particles.
            if (!Application.isPlaying)
            {
                foreach (var uip in targets.OfType<UIParticle>())
                {
                    if (PrefabUtility.GetPrefabAssetType(uip) != PrefabAssetType.NotAPrefab) continue;
                    uip.RefreshParticles(uip.particles);
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
            if (!current) return;

            serializedObject.Update();

            // Maskable
            EditorGUILayout.PropertyField(_maskable);

            // Scale
            EditorGUI.BeginDisabledGroup(!_meshSharing.hasMultipleDifferentValues && _meshSharing.intValue == 4);
            s_XYZMode = DrawFloatOrVector3Field(_scale3D, s_XYZMode);
            EditorGUI.EndDisabledGroup();

            // AnimatableProperties
            var mats = current.particles
                .Where(x => x)
                .Select(x => x.GetComponent<ParticleSystemRenderer>().sharedMaterial)
                .Where(x => x)
                .ToArray();

            AnimatablePropertyEditor.Draw(_animatableProperties, mats);

            // Mesh sharing
            EditorGUI.BeginChangeCheck();
            _showMax = DrawMeshSharing(_meshSharing, _groupId, _groupMaxId, _showMax);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                foreach (var uip in targets.OfType<UIParticle>())
                {
                    uip.ResetGroupId();
                }
            }

            // Position Mode
            EditorGUILayout.PropertyField(_positionMode);

            // Auto Scaling
            DrawAutoScaling(_autoScalingMode, targets.OfType<UIParticle>());

            // Target ParticleSystems.
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(targets.OfType<UIParticle>().Any(x => !x.canvas));
            _ro.DoLayoutList();
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var uip in targets.OfType<UIParticle>())
                {
                    uip.RefreshParticles(uip.particles);
                }
            }

            // Non-UI built-in shader is not supported.
            foreach (var mat in current.materials)
            {
                if (!mat || !mat.shader) continue;
                var shader = mat.shader;
                if (IsBuiltInObject(shader) && !shader.name.StartsWith("UI/"))
                {
                    EditorGUILayout.HelpBox(
                        $"Built-in shader '{shader.name}' in '{mat.name}' is not supported.\n" +
                        "Use UI shaders instead.",
                        MessageType.Error);
                }
            }

            // Does the shader support UI masks?
            if (current.maskable && current.GetComponentInParent<Mask>(false))
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

                        EditorGUILayout.HelpBox(
                            $"Shader '{shader.name}' doesn't have '{propName}' property." +
                            "\nThis graphic cannot be masked.",
                            MessageType.Warning);
                        break;
                    }
                }
            }

            s_Shaders.Clear();

            // UIParticle for trail should be removed.
            var label = "This UIParticle component should be removed. The UIParticle for trails is no longer needed.";
            if (FixButton(current.m_IsTrail, label))
            {
                DestroyUIParticle(current);
            }

            var allPsRenderers = targets.OfType<UIParticle>()
                .SelectMany(x => x.particles)
                .Where(x => x)
                .Select(x => x.GetComponent<ParticleSystemRenderer>())
                .ToArray();
            if (0 < allPsRenderers.Length)
            {
                // Check to use 'TEXCOORD*.zw' components as custom vertex stream.
                foreach (var psr in allPsRenderers)
                {
                    if (!new SerializedObject(psr).FindProperty("m_UseCustomVertexStreams").boolValue) continue;
                    if (psr.activeVertexStreamsCount == 0) continue;
                    psr.GetActiveVertexStreams(s_Streams);

                    if (2 < s_Streams.Select(GetUsedComponentCount).Sum())
                    {
                        EditorGUILayout.HelpBox(
                            $"ParticleSystem '{psr.name}' uses 'TEXCOORD*.zw' components as custom vertex stream.\n" +
                            "UIParticle does not support it (See README.md).",
                            MessageType.Warning);
                    }

                    s_Streams.Clear();
                }
            }
        }

        private bool IsBuiltInObject(Object obj)
        {
            return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out long _)
                   && Regex.IsMatch(guid, "^0{16}.0{15}$", RegexOptions.Compiled);
        }

        private static int GetUsedComponentCount(ParticleSystemVertexStream s)
        {
            switch (s)
            {
                case ParticleSystemVertexStream.Position:
                case ParticleSystemVertexStream.Normal:
                case ParticleSystemVertexStream.Tangent:
                case ParticleSystemVertexStream.Color:
                    return 0;
                case ParticleSystemVertexStream.UV:
                case ParticleSystemVertexStream.UV2:
                case ParticleSystemVertexStream.UV3:
                case ParticleSystemVertexStream.UV4:
                case ParticleSystemVertexStream.SizeXY:
                case ParticleSystemVertexStream.StableRandomXY:
                case ParticleSystemVertexStream.VaryingRandomXY:
                case ParticleSystemVertexStream.Custom1XY:
                case ParticleSystemVertexStream.Custom2XY:
                case ParticleSystemVertexStream.NoiseSumXY:
                case ParticleSystemVertexStream.NoiseImpulseXY:
                    return 2;
                case ParticleSystemVertexStream.AnimBlend:
                case ParticleSystemVertexStream.AnimFrame:
                case ParticleSystemVertexStream.VertexID:
                case ParticleSystemVertexStream.SizeX:
                case ParticleSystemVertexStream.Rotation:
                case ParticleSystemVertexStream.RotationSpeed:
                case ParticleSystemVertexStream.Velocity:
                case ParticleSystemVertexStream.Speed:
                case ParticleSystemVertexStream.AgePercent:
                case ParticleSystemVertexStream.InvStartLifetime:
                case ParticleSystemVertexStream.StableRandomX:
                case ParticleSystemVertexStream.VaryingRandomX:
                case ParticleSystemVertexStream.Custom1X:
                case ParticleSystemVertexStream.Custom2X:
                case ParticleSystemVertexStream.NoiseSumX:
                case ParticleSystemVertexStream.NoiseImpulseX:
                    return 1;
                case ParticleSystemVertexStream.Center:
                case ParticleSystemVertexStream.SizeXYZ:
                case ParticleSystemVertexStream.Rotation3D:
                case ParticleSystemVertexStream.RotationSpeed3D:
                case ParticleSystemVertexStream.StableRandomXYZ:
                case ParticleSystemVertexStream.VaryingRandomXYZ:
                case ParticleSystemVertexStream.Custom1XYZ:
                case ParticleSystemVertexStream.Custom2XYZ:
                case ParticleSystemVertexStream.NoiseSumXYZ:
                case ParticleSystemVertexStream.NoiseImpulseXYZ:
                    return 3;
                case ParticleSystemVertexStream.StableRandomXYZW:
                case ParticleSystemVertexStream.VaryingRandomXYZW:
                case ParticleSystemVertexStream.Custom1XYZW:
                case ParticleSystemVertexStream.Custom2XYZW:
                    return 4;
            }

            return 3;
        }

        private static bool DrawMeshSharing(SerializedProperty spMeshSharing, SerializedProperty spGroupId,
            SerializedProperty spGroupMaxId, bool showMax)
        {
            showMax |= spGroupId.intValue != spGroupMaxId.intValue ||
                       spGroupId.hasMultipleDifferentValues ||
                       spGroupMaxId.hasMultipleDifferentValues;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(spMeshSharing);

            EditorGUI.BeginChangeCheck();
            showMax = GUILayout.Toggle(showMax, s_ContentRandom, EditorStyles.miniButton, GUILayout.Width(60));
            if (EditorGUI.EndChangeCheck() && !showMax)
            {
                spGroupMaxId.intValue = spGroupId.intValue;
            }

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
                var obj = UIParticleUpdater.GetPrimary(spGroupId.intValue);
                EditorGUILayout.ObjectField("Primary", obj, typeof(UIParticle), false);
                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();

            return showMax;
        }

        private static void DrawAutoScaling(SerializedProperty prop, IEnumerable<UIParticle> uiParticles)
        {
            var isTransformMode = prop.intValue == (int)UIParticle.AutoScalingMode.Transform;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(prop);
            if (!EditorGUI.EndChangeCheck() || !isTransformMode) return;

            // on changed true->false, reset scale.
            EditorApplication.delayCall += () =>
            {
                foreach (var uip in uiParticles)
                {
                    if (!uip) continue;
                    uip.transform.localScale = Vector3.one;
                }
            };
        }

        private static void WindowFunction(Object target, SceneView sceneView)
        {
            try
            {
                if (s_SerializedObject == null || !s_SerializedObject.targetObject) return;
                var uiParticles = s_SerializedObject.targetObjects.OfType<UIParticle>();
                if (uiParticles.Any(x => !x || !x.canvas)) return;

                s_SerializedObject.Update();
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(220f)))
                {
                    var labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 100;
                    EditorGUILayout.PropertyField(s_SerializedObject.FindProperty("m_Enabled"));
                    s_XYZMode = DrawFloatOrVector3Field(s_SerializedObject.FindProperty("m_Scale3D"), s_XYZMode);
                    EditorGUILayout.PropertyField(s_SerializedObject.FindProperty("m_PositionMode"));
                    DrawAutoScaling(s_SerializedObject.FindProperty("m_AutoScalingMode"), uiParticles);
                    EditorGUIUtility.labelWidth = labelWidth;
                }

                s_SerializedObject.ApplyModifiedProperties();
            }
            catch
            {
                // ignored
            }
        }

        private void DestroyUIParticle(UIParticle p, bool ignoreCurrent = false)
        {
            if (!p || (ignoreCurrent && target == p)) return;

            var cr = p.canvasRenderer;
            DestroyImmediate(p);
            DestroyImmediate(cr);

#if UNITY_2018_3_OR_NEWER
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null && stage.scene.isLoaded)
            {
#if UNITY_2020_1_OR_NEWER
                string prefabAssetPath = stage.assetPath;
#else
                var prefabAssetPath = stage.prefabAssetPath;
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
            {
                z.floatValue = y.floatValue = x.floatValue;
            }

            EditorGUILayout.EndHorizontal();

            return showXyz;
        }
    }
}
