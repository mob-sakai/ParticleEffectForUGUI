using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Coffee.UIParticleInternal;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.Overlays;
#else
using System;
using System.Reflection;
using Object = UnityEngine.Object;
#endif
#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;

#elif UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif

namespace Coffee.UIExtensions
{
    [CustomEditor(typeof(UIParticle))]
    [CanEditMultipleObjects]
    internal class UIParticleEditor : GraphicEditor
    {
        internal class State : ScriptableSingleton<State>
        {
            public bool is3DScaleMode;
        }

        //################################
        // Constant or Static Members.
        //################################
        private static readonly GUIContent[] s_ContentMaterials = new[]
        {
            new GUIContent("Material"),
            new GUIContent("Trail Material")
        };

        private static readonly GUIContent s_ContentRenderingOrder = new GUIContent("Rendering Order");
        private static readonly GUIContent s_ContentRefresh = new GUIContent("Refresh");
        private static readonly GUIContent s_ContentFix = new GUIContent("Fix");
        private static readonly GUIContent s_Content3D = new GUIContent("3D");
        private static readonly GUIContent s_ContentRandom = new GUIContent("Random");
        private static readonly GUIContent s_ContentScale = new GUIContent("Scale");
        private static readonly GUIContent s_ContentPrimary = new GUIContent("Primary");
        private static readonly Regex s_RegexBuiltInGuid = new Regex(@"^0{16}.0{15}$", RegexOptions.Compiled);
        private static readonly List<Material> s_TempMaterials = new List<Material>();

        private SerializedProperty _maskable;
        private SerializedProperty _scale3D;
        private SerializedProperty _animatableProperties;
        private SerializedProperty _meshSharing;
        private SerializedProperty _groupId;
        private SerializedProperty _groupMaxId;
        private SerializedProperty _positionMode;
        private SerializedProperty _autoScalingMode;
        private SerializedProperty _useCustomView;
        private SerializedProperty _customViewSize;
        private ReorderableList _ro;
        private bool _showMax;
        private bool _is3DScaleMode;

        private static readonly HashSet<Shader> s_Shaders = new HashSet<Shader>();
#if UNITY_2018 || UNITY_2019
        private static readonly List<ParticleSystemVertexStream> s_Streams = new List<ParticleSystemVertexStream>();
#endif
        private static readonly List<string> s_MaskablePropertyNames = new List<string>
        {
            "_Stencil",
            "_StencilComp",
            "_StencilOp",
            "_StencilWriteMask",
            "_StencilReadMask",
            "_ColorMask"
        };

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
            _useCustomView = serializedObject.FindProperty("m_UseCustomView");
            _customViewSize = serializedObject.FindProperty("m_CustomViewSize");

            var sp = serializedObject.FindProperty("m_Particles");
            _ro = new ReorderableList(sp.serializedObject, sp, true, true, true, true)
            {
                elementHeightCallback = index =>
                {
                    var ps = sp.GetArrayElementAtIndex(index).objectReferenceValue as ParticleSystem;
                    var materialCount = 0;
                    if (ps && ps.TryGetComponent<ParticleSystemRenderer>(out var psr))
                    {
                        materialCount = psr.sharedMaterials.Length;
                    }

                    return (materialCount + 1) * (EditorGUIUtility.singleLineHeight + 2);
                },
                drawElementCallback = (rect, index, _, __) =>
                {
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    var p = sp.GetArrayElementAtIndex(index);
                    EditorGUI.ObjectField(rect, p, GUIContent.none);
                    var ps = p.objectReferenceValue as ParticleSystem;
                    if (!ps || !ps.TryGetComponent<ParticleSystemRenderer>(out var psr)) return;

                    rect.x += 15;
                    rect.width -= 15;
                    var materials = new SerializedObject(psr).FindProperty("m_Materials");
                    var count = Mathf.Min(materials.arraySize, 2);
                    for (var i = 0; i < count; i++)
                    {
                        rect.y += rect.height + 2;
                        EditorGUI.PropertyField(rect, materials.GetArrayElementAtIndex(i), s_ContentMaterials[i]);
                    }

                    if (materials.serializedObject.hasModifiedProperties)
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

            // Initialize 3D scale mode.
            _is3DScaleMode = State.instance.is3DScaleMode;
            if (!_is3DScaleMode)
            {
                var x = _scale3D.FindPropertyRelative("x");
                var y = _scale3D.FindPropertyRelative("y");
                var z = _scale3D.FindPropertyRelative("z");
                _is3DScaleMode = !Mathf.Approximately(x.floatValue, y.floatValue) ||
                                 !Mathf.Approximately(y.floatValue, z.floatValue) ||
                                 y.hasMultipleDifferentValues ||
                                 z.hasMultipleDifferentValues;
            }
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var current = target as UIParticle;
            if (!current) return;

            Profiler.BeginSample("(UIP:E) OnInspectorGUI");
            serializedObject.Update();

            // Maskable
            EditorGUILayout.PropertyField(_maskable);

            // Scale
            EditorGUI.BeginDisabledGroup(!_meshSharing.hasMultipleDifferentValues && _meshSharing.intValue == 4);
            if (DrawFloatOrVector3Field(_scale3D, _is3DScaleMode) != _is3DScaleMode)
            {
                State.instance.is3DScaleMode = _is3DScaleMode = !_is3DScaleMode;
            }

            EditorGUI.EndDisabledGroup();

            // AnimatableProperties
            current.GetMaterials(s_TempMaterials);
            AnimatablePropertyEditor.Draw(_animatableProperties, s_TempMaterials);

            // Mesh sharing
            EditorGUI.BeginChangeCheck();
            _showMax = DrawMeshSharing(_meshSharing, _groupId, _groupMaxId, _showMax);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                foreach (var t in targets)
                {
                    if (t is UIParticle uip)
                    {
                        uip.ResetGroupId();
                    }
                }
            }

            // Position Mode
            EditorGUILayout.PropertyField(_positionMode);

            // Auto Scaling
            EditorGUILayout.PropertyField(_autoScalingMode);

            // Custom View Size
            EditorGUILayout.PropertyField(_useCustomView);
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(!_useCustomView.boolValue);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_customViewSize);
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                _customViewSize.floatValue = Mathf.Max(0.1f, _customViewSize.floatValue);
            }

            // Target ParticleSystems.
            EditorGUI.BeginChangeCheck();
            _ro.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorApplication.QueuePlayerLoopUpdate();
                foreach (var uip in targets.OfType<UIParticle>())
                {
                    uip.RefreshParticles(uip.particles);
                }
            }

            // Non-UI built-in shader is not supported.
            Profiler.BeginSample("(UIP:E) Non-UI built-in shader is not supported.");
            foreach (var mat in s_TempMaterials)
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

            Profiler.EndSample();

            // Does the shader support UI masks?
            Profiler.BeginSample("(UIP:E) Does the shader support UI masks?");
            if (current.maskable && current.GetComponentInParent<Mask>(false))
            {
                foreach (var mat in s_TempMaterials)
                {
                    if (!mat || !mat.shader) continue;
                    var shader = mat.shader;
                    if (!s_Shaders.Add(shader)) continue;

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

            s_TempMaterials.Clear();
            s_Shaders.Clear();
            Profiler.EndSample();

            // UIParticle for trail should be removed.
            var label = "This UIParticle component should be removed. The UIParticle for trails is no longer needed.";
#pragma warning disable CS0612
            if (FixButton(current.m_IsTrail, label))
#pragma warning restore CS0612
            {
                DestroyUIParticle(current);
            }

#if UNITY_2018 || UNITY_2019
            // (2018, 2019) Check to use 'TEXCOORD*.zw' components as custom vertex stream.
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
#endif
            Profiler.EndSample();
        }

        private static bool IsBuiltInObject(Object obj)
        {
            return AssetDatabase.IsMainAsset(obj)
                   && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out long _)
                   && s_RegexBuiltInGuid.IsMatch(guid);
        }

#if UNITY_2018 || UNITY_2019
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
#endif

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
                EditorGUILayout.ObjectField(s_ContentPrimary, obj, typeof(UIParticle), false);
                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();

            return showMax;
        }

        private static void DrawAutoScaling(SerializedProperty prop)
        {
            EditorGUILayout.PropertyField(prop);
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
                var prefabAssetPath = stage.assetPath;
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
