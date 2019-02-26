using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using System;
using System.Reflection;
using ShaderPropertyType = Coffee.UIExtensions.UIParticle.AnimatableProperty.ShaderPropertyType;

namespace Coffee.UIExtensions
{
	[CustomEditor (typeof (UIParticle))]
	[CanEditMultipleObjects]
	public class UIParticleEditor : GraphicEditor
	{
		class AnimatedPropertiesEditor
		{
			static readonly List<string> s_ActiveNames = new List<string> ();
			static readonly System.Text.StringBuilder s_Sb = new System.Text.StringBuilder ();

			public string name;
			public ShaderPropertyType type;

			static string CollectActiveNames (SerializedProperty sp, List<string> result)
			{
				result.Clear ();
				for (int i = 0; i < sp.arraySize; i++)
				{
					result.Add (sp.GetArrayElementAtIndex (i).FindPropertyRelative ("m_Name").stringValue);
				}

				s_Sb.Length = 0;
				if (result.Count == 0)
				{
					s_Sb.Append ("Nothing");
				}
				else
				{
					result.Aggregate (s_Sb, (a, b) => s_Sb.AppendFormat ("{0}, ", b));
					s_Sb.Length -= 2;
				}

				return s_Sb.ToString ();
			}

			public static void DrawAnimatableProperties (SerializedProperty sp, Material mat)
			{
				if (!mat || !mat.shader)
					return;
				bool isClicked = false;
				using (new EditorGUILayout.HorizontalScope (GUILayout.ExpandWidth (false)))
				{
					var r = EditorGUI.PrefixLabel (EditorGUILayout.GetControlRect (true), new GUIContent (sp.displayName, sp.tooltip));
					isClicked = GUI.Button (r, CollectActiveNames (sp, s_ActiveNames), EditorStyles.popup);
				}

				if (isClicked)
				{
					GenericMenu gm = new GenericMenu ();
					gm.AddItem (new GUIContent ("Nothing"), s_ActiveNames.Count == 0, () =>
					{
						sp.ClearArray ();
						sp.serializedObject.ApplyModifiedProperties ();
					});


					for (int i = 0; i < sp.arraySize; i++)
					{
						var p = sp.GetArrayElementAtIndex (i);
						var name = p.FindPropertyRelative ("m_Name").stringValue;
						var type = (ShaderPropertyType)p.FindPropertyRelative ("m_Type").intValue;
						AddMenu (gm, sp, new AnimatedPropertiesEditor () { name = name, type = type }, false);
					}

					for (int i = 0; i < ShaderUtil.GetPropertyCount (mat.shader); i++)
					{
						var pName = ShaderUtil.GetPropertyName (mat.shader, i);
						var type = (ShaderPropertyType)ShaderUtil.GetPropertyType (mat.shader, i);
						AddMenu (gm, sp, new AnimatedPropertiesEditor () { name = pName, type = type }, true);

						if (type == ShaderPropertyType.Texture)
						{
							AddMenu (gm, sp, new AnimatedPropertiesEditor () { name = pName + "_ST", type = ShaderPropertyType.Vector }, true);
							AddMenu (gm, sp, new AnimatedPropertiesEditor () { name = pName + "_HDR", type = ShaderPropertyType.Vector }, true);
							AddMenu (gm, sp, new AnimatedPropertiesEditor () { name = pName + "_TexelSize", type = ShaderPropertyType.Vector }, true);
						}

					}

					gm.ShowAsContext ();
				}
			}

			public static void AddMenu (GenericMenu menu, SerializedProperty sp, AnimatedPropertiesEditor property, bool add)
			{
				if (add && s_ActiveNames.Contains (property.name))
					return;

				menu.AddItem (new GUIContent (string.Format ("{0} ({1})", property.name, property.type)), s_ActiveNames.Contains (property.name), () =>
				{
					var index = s_ActiveNames.IndexOf (property.name);
					if (0 <= index)
					{
						sp.DeleteArrayElementAtIndex (index);
					}
					else
					{
						sp.InsertArrayElementAtIndex (sp.arraySize);
						var p = sp.GetArrayElementAtIndex (sp.arraySize - 1);
						p.FindPropertyRelative ("m_Name").stringValue = property.name;
						p.FindPropertyRelative ("m_Type").intValue = (int)property.type;
					}
					sp.serializedObject.ApplyModifiedProperties ();
				});
			}
		}

		//################################
		// Constant or Static Members.
		//################################
		static readonly GUIContent s_ContentParticleMaterial = new GUIContent ("Particle Material", "The material for rendering particles");
		static readonly GUIContent s_ContentTrailMaterial = new GUIContent ("Trail Material", "The material for rendering particle trails");
		static readonly List<ParticleSystem> s_ParticleSystems = new List<ParticleSystem> ();
		static readonly Matrix4x4 s_ArcHandleOffsetMatrix = Matrix4x4.TRS (Vector3.zero, Quaternion.AngleAxis (90f, Vector3.right) * Quaternion.AngleAxis (90f, Vector3.up), Vector3.one);
		static readonly Dictionary<string, MethodInfo> s_InternalMethods = new Dictionary<string, MethodInfo> ();
		static readonly Color s_GizmoColor = new Color (1f, 0.7f, 0.7f, 0.9f);
		static readonly Color s_ShapeGizmoThicknessTint = new Color (0.7f, 0.7f, 0.7f, 1.0f);
		static Material s_Material;

		static readonly List<string> s_MaskablePropertyNames = new List<string> ()
		{
			"_Stencil",
			"_StencilComp",
			"_StencilOp",
			"_StencilWriteMask",
			"_StencilReadMask",
			"_ColorMask",
		};

		//################################
		// Public/Protected Members.
		//################################
		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable ()
		{
			base.OnEnable ();
			_spParticleSystem = serializedObject.FindProperty ("m_ParticleSystem");
			_spTrailParticle = serializedObject.FindProperty ("m_TrailParticle");
			_spScale = serializedObject.FindProperty ("m_Scale");
			_spIgnoreParent = serializedObject.FindProperty ("m_IgnoreParent");
			_spAnimatableProperties = serializedObject.FindProperty ("m_AnimatableProperties");

			if (!s_Material)
			{
				s_Material = Call<Material> (typeof (Material), "GetDefaultMaterial");
			}

			_particles = targets.Cast<UIParticle> ().ToArray ();
		}

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			EditorGUILayout.PropertyField (_spParticleSystem);
			EditorGUI.indentLevel++;
			var ps = _spParticleSystem.objectReferenceValue as ParticleSystem;
			if (ps)
			{
				var pr = ps.GetComponent<ParticleSystemRenderer> ();
				var sp = new SerializedObject (pr).FindProperty ("m_Materials");

				EditorGUILayout.PropertyField (sp.GetArrayElementAtIndex (0), s_ContentParticleMaterial);
				EditorGUILayout.PropertyField (sp.GetArrayElementAtIndex (1), s_ContentTrailMaterial);
				sp.serializedObject.ApplyModifiedProperties ();

				if (!Application.isPlaying && pr.enabled)
				{
					EditorGUILayout.HelpBox ("ParticleSystemRenderer will be disable on playing.", MessageType.Info);
				}
			}
			EditorGUI.indentLevel--;

			EditorGUI.BeginDisabledGroup (true);
			EditorGUILayout.PropertyField (_spTrailParticle);
			EditorGUI.EndDisabledGroup ();

			var current = target as UIParticle;

			EditorGUILayout.PropertyField (_spIgnoreParent);

			EditorGUI.BeginDisabledGroup (!current.isRoot);
			EditorGUILayout.PropertyField (_spScale);
			EditorGUI.EndDisabledGroup ();

			// AnimatableProperties
			AnimatedPropertiesEditor.DrawAnimatableProperties (_spAnimatableProperties, current.material);

			current.GetComponentsInChildren<ParticleSystem> (true, s_ParticleSystems);
			if (s_ParticleSystems.Any (x => x.GetComponent<UIParticle> () == null))
			{
				GUILayout.BeginHorizontal ();
				EditorGUILayout.HelpBox ("There are child ParticleSystems that does not have a UIParticle component.\nAdd UIParticle component to them.", MessageType.Warning);
				GUILayout.BeginVertical ();
				if (GUILayout.Button ("Fix"))
				{
					foreach (var p in s_ParticleSystems.Where (x => !x.GetComponent<UIParticle> ()))
					{
						p.gameObject.AddComponent<UIParticle> ();
					}
				}
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal ();
			}
			s_ParticleSystems.Clear ();

			if (current.maskable && current.material && current.material.shader)
			{
				var mat = current.material;
				var shader = mat.shader;
				foreach (var propName in s_MaskablePropertyNames)
				{
					if (!mat.HasProperty (propName))
					{
						EditorGUILayout.HelpBox (string.Format ("Shader {0} doesn't have '{1}' property. This graphic is not maskable.", shader.name, propName), MessageType.Warning);
						break;
					}
				}
			}

			serializedObject.ApplyModifiedProperties ();
		}






		//################################
		// Private Members.
		//################################
		SerializedProperty _spParticleSystem;
		SerializedProperty _spTrailParticle;
		SerializedProperty _spScale;
		SerializedProperty _spIgnoreParent;
		SerializedProperty _spAnimatableProperties;
		UIParticle [] _particles;
		ArcHandle _arcHandle = new ArcHandle ();
		BoxBoundsHandle _boxBoundsHandle = new BoxBoundsHandle ();
		SphereBoundsHandle _sphereBoundsHandle = new SphereBoundsHandle ();
		Mesh _spriteMesh;

		static T Call<T> (Type type, string method, params object [] args)
		{
			MethodInfo mi;
			if (!s_InternalMethods.TryGetValue (method, out mi))
			{
				mi = type.GetMethod (method, BindingFlags.Static | BindingFlags.NonPublic);
				s_InternalMethods.Add (method, mi);
			}
			return (T)mi.Invoke (null, args);
		}

		void OnSceneGUI ()
		{
			Color origCol = Handles.color;
			Handles.color = s_GizmoColor;

			Matrix4x4 orgMatrix = Handles.matrix;

			EditorGUI.BeginChangeCheck ();

			foreach (UIParticle uip in _particles)
			{
				ParticleSystem ps = uip.cachedParticleSystem;
				if (!ps || !uip.canvas)
				{
					continue;
				}

				var shapeModule = ps.shape;
				var mainModule = ps.main;

				ParticleSystemShapeType type = shapeModule.shapeType;

				Matrix4x4 transformMatrix = new Matrix4x4 ();
				if (mainModule.scalingMode == ParticleSystemScalingMode.Local)
				{
					transformMatrix.SetTRS (ps.transform.position, ps.transform.rotation, ps.transform.localScale);
				}
				else if (mainModule.scalingMode == ParticleSystemScalingMode.Hierarchy)
				{
					transformMatrix = ps.transform.localToWorldMatrix;
				}
				else
				{
					transformMatrix.SetTRS (ps.transform.position, ps.transform.rotation, ps.transform.lossyScale);
				}

				bool isBox = (type == ParticleSystemShapeType.Box || type == ParticleSystemShapeType.BoxShell || type == ParticleSystemShapeType.BoxEdge || type == ParticleSystemShapeType.Rectangle);

				Vector3 emitterScale = isBox ? Vector3.one : shapeModule.scale;
				Matrix4x4 emitterMatrix = Matrix4x4.TRS (shapeModule.position, Quaternion.Euler (shapeModule.rotation), emitterScale);
				transformMatrix *= emitterMatrix;
				Handles.matrix = transformMatrix;

				if (uip.canvas.renderMode == RenderMode.ScreenSpaceOverlay || ps.main.scalingMode == ParticleSystemScalingMode.Hierarchy)
				{
					Handles.matrix = Handles.matrix * Matrix4x4.Scale (Vector3.one * uip.scale);
				}
				else
				{
					Handles.matrix = Handles.matrix * Matrix4x4.Scale (uip.canvas.rootCanvas.transform.localScale * uip.scale);
				}

				if (type == ParticleSystemShapeType.Sphere)
				{
					// Thickness
					Handles.color *= s_ShapeGizmoThicknessTint;
					EditorGUI.BeginChangeCheck ();
					//float radiusThickness = Handles.DoSimpleRadiusHandle (Quaternion.identity, Vector3.zero, shapeModule.radius * (1.0f - shapeModule.radiusThickness), false, shapeModule.arc);
					float radiusThickness = Handles.RadiusHandle (Quaternion.identity, Vector3.zero, shapeModule.radius * (1.0f - shapeModule.radiusThickness), false);
					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Sphere Thickness Handle Change");
						shapeModule.radiusThickness = 1.0f - (radiusThickness / shapeModule.radius);
					}

					// Sphere
					Handles.color = s_GizmoColor;
					EditorGUI.BeginChangeCheck ();
					//float radius = Handles.DoSimpleRadiusHandle (Quaternion.identity, Vector3.zero, shapeModule.radius, false, shapeModule.arc);
					float radius = Handles.RadiusHandle (Quaternion.identity, Vector3.zero, shapeModule.radius, false);
					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Sphere Handle Change");
						shapeModule.radius = radius;
					}

					// Texture
					//Matrix4x4 textureTransform = transformMatrix * Matrix4x4.TRS (Vector3.zero, Quaternion.identity, Vector3.one * shapeModule.radius * 2.0f);
					//OnSceneViewTextureGUI (shapeModule, s_SphereMesh, false, s_SphereTextureMaterial, textureTransform);
				}
				else if (type == ParticleSystemShapeType.Circle)
				{
					// Thickness
					EditorGUI.BeginChangeCheck ();

					_arcHandle.angle = shapeModule.arc;
					_arcHandle.radius = shapeModule.radius * (1.0f - shapeModule.radiusThickness);
					_arcHandle.SetColorWithRadiusHandle (s_ShapeGizmoThicknessTint, 0f);
					_arcHandle.angleHandleColor = Color.clear;

					using (new Handles.DrawingScope (Handles.matrix * s_ArcHandleOffsetMatrix))
						_arcHandle.DrawHandle ();

					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Circle Thickness Handle Change");
						shapeModule.radiusThickness = 1.0f - (_arcHandle.radius / shapeModule.radius);
					}

					// Circle
					EditorGUI.BeginChangeCheck ();

					_arcHandle.radius = shapeModule.radius;
					_arcHandle.SetColorWithRadiusHandle (Color.white, 0f);

					using (new Handles.DrawingScope (Handles.matrix * s_ArcHandleOffsetMatrix))
						_arcHandle.DrawHandle ();

					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Circle Handle Change");
						shapeModule.radius = _arcHandle.radius;
						shapeModule.arc = _arcHandle.angle;
					}

					// Texture
					//Matrix4x4 textureTransform = transformMatrix * Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (90.0f, 0.0f, 180.0f), Vector3.one * shapeModule.radius * 2.0f);
					//OnSceneViewTextureGUI (shapeModule, s_CircleMesh, true, s_TextureMaterial, textureTransform);
				}
				else if (type == ParticleSystemShapeType.Hemisphere)
				{
					// Thickness
					Handles.color *= s_ShapeGizmoThicknessTint;
					EditorGUI.BeginChangeCheck ();
					//float radiusThickness = Handles.DoSimpleRadiusHandle (Quaternion.identity, Vector3.zero, shapeModule.radius * (1.0f - shapeModule.radiusThickness), true, shapeModule.arc);
					//float radiusThickness = Call<float> (typeof (Handles), "DoSimpleRadiusHandle", Quaternion.identity, Vector3.zero, shapeModule.radius * (1.0f - shapeModule.radiusThickness), true, shapeModule.arc);
					float radiusThickness = Handles.RadiusHandle (Quaternion.identity, Vector3.zero, shapeModule.radius * (1.0f - shapeModule.radiusThickness), true);
					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Hemisphere Thickness Handle Change");
						shapeModule.radiusThickness = 1.0f - (radiusThickness / shapeModule.radius);
					}

					// Hemisphere
					Handles.color = s_GizmoColor;
					EditorGUI.BeginChangeCheck ();
					//float radius = Handles.DoSimpleRadiusHandle (Quaternion.identity, Vector3.zero, shapeModule.radius, true, shapeModule.arc);
					float radius = Handles.RadiusHandle (Quaternion.identity, Vector3.zero, shapeModule.radius, true);
					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Hemisphere Handle Change");
						shapeModule.radius = radius;
					}

					// Texture
					//Matrix4x4 textureTransform = transformMatrix * Matrix4x4.TRS (Vector3.zero, Quaternion.identity, Vector3.one * shapeModule.radius * 2.0f);
					//OnSceneViewTextureGUI (shapeModule, s_HemisphereMesh, false, s_SphereTextureMaterial, textureTransform);
				}
				else if (type == ParticleSystemShapeType.Cone)
				{
					// Thickness
					Handles.color *= s_ShapeGizmoThicknessTint;
					EditorGUI.BeginChangeCheck ();
					float angleThickness = Mathf.Lerp (shapeModule.angle, 0.0f, shapeModule.radiusThickness);
					Vector3 radiusThicknessAngleRange = new Vector3 (shapeModule.radius * (1.0f - shapeModule.radiusThickness), angleThickness, mainModule.startSpeedMultiplier);
					//radiusThicknessAngleRange = Handles.ConeFrustrumHandle (Quaternion.identity, Vector3.zero, radiusThicknessAngleRange, Handles.ConeHandles.Radius);
#if UNITY_2018_3_OR_NEWER
					radiusThicknessAngleRange = Call<Vector3> (typeof (Handles), "ConeFrustrumHandle", Quaternion.identity, Vector3.zero, radiusThicknessAngleRange, 1);
#else
					radiusThicknessAngleRange = Call<Vector3> (typeof (Handles), "ConeFrustrumHandle", Quaternion.identity, Vector3.zero, radiusThicknessAngleRange);
#endif
					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Cone Thickness Handle Change");
						shapeModule.radiusThickness = 1.0f - (radiusThicknessAngleRange.x / shapeModule.radius);
					}

					// Cone
					Handles.color = s_GizmoColor;
					EditorGUI.BeginChangeCheck ();
					Vector3 radiusAngleRange = new Vector3 (shapeModule.radius, shapeModule.angle, mainModule.startSpeedMultiplier);
					//radiusAngleRange = Handles.ConeFrustrumHandle (Quaternion.identity, Vector3.zero, radiusAngleRange);
#if UNITY_2018_3_OR_NEWER
					radiusAngleRange = Call<Vector3> (typeof (Handles), "ConeFrustrumHandle", Quaternion.identity, Vector3.zero, radiusAngleRange, 7);
#else
					radiusAngleRange = Call<Vector3> (typeof (Handles), "ConeFrustrumHandle", Quaternion.identity, Vector3.zero, radiusAngleRange);
#endif
					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Cone Handle Change");
						shapeModule.radius = radiusAngleRange.x;
						shapeModule.angle = radiusAngleRange.y;
						mainModule.startSpeedMultiplier = radiusAngleRange.z;
					}

					// Texture
					//Matrix4x4 textureTransform = transformMatrix * Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (90.0f, 0.0f, 180.0f), Vector3.one * shapeModule.radius * 2.0f);
					//OnSceneViewTextureGUI (shapeModule, s_CircleMesh, true, s_TextureMaterial, textureTransform);
				}
				else if (type == ParticleSystemShapeType.ConeVolume)
				{
					// Thickness
					Handles.color *= s_ShapeGizmoThicknessTint;
					EditorGUI.BeginChangeCheck ();
					float angleThickness = Mathf.Lerp (shapeModule.angle, 0.0f, shapeModule.radiusThickness);
					Vector3 radiusThicknessAngleLength = new Vector3 (shapeModule.radius * (1.0f - shapeModule.radiusThickness), angleThickness, shapeModule.length);
					//radiusThicknessAngleLength = Handles.ConeFrustrumHandle (Quaternion.identity, Vector3.zero, radiusThicknessAngleLength, Handles.ConeHandles.Radius);
#if UNITY_2018_3_OR_NEWER
					radiusThicknessAngleLength = Call<Vector3> (typeof (Handles), "ConeFrustrumHandle", Quaternion.identity, Vector3.zero, radiusThicknessAngleLength, 1);
#else
					radiusThicknessAngleLength = Call<Vector3> (typeof (Handles), "ConeFrustrumHandle", Quaternion.identity, Vector3.zero, radiusThicknessAngleLength);
#endif
					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Cone Volume Thickness Handle Change");
						shapeModule.radiusThickness = 1.0f - (radiusThicknessAngleLength.x / shapeModule.radius);
					}

					// Cone
					Handles.color = s_GizmoColor;
					EditorGUI.BeginChangeCheck ();
					Vector3 radiusAngleLength = new Vector3 (shapeModule.radius, shapeModule.angle, shapeModule.length);
					//radiusAngleLength = Handles.ConeFrustrumHandle (Quaternion.identity, Vector3.zero, radiusAngleLength);
#if UNITY_2018_3_OR_NEWER
					radiusAngleLength = Call<Vector3> (typeof (Handles), "ConeFrustrumHandle", Quaternion.identity, Vector3.zero, radiusAngleLength, 7);
#else
					radiusAngleLength = Call<Vector3> (typeof (Handles), "ConeFrustrumHandle", Quaternion.identity, Vector3.zero, radiusAngleLength);
#endif
					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Cone Volume Handle Change");
						shapeModule.radius = radiusAngleLength.x;
						shapeModule.angle = radiusAngleLength.y;
						shapeModule.length = radiusAngleLength.z;
					}

					// Texture
					//Matrix4x4 textureTransform = transformMatrix * Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (90.0f, 0.0f, 180.0f), Vector3.one * shapeModule.radius * 2.0f);
					//OnSceneViewTextureGUI (shapeModule, s_CircleMesh, true, s_TextureMaterial, textureTransform);
				}
				else if (type == ParticleSystemShapeType.Box || type == ParticleSystemShapeType.BoxShell || type == ParticleSystemShapeType.BoxEdge)
				{
					EditorGUI.BeginChangeCheck ();

					_boxBoundsHandle.center = Vector3.zero;
					_boxBoundsHandle.size = shapeModule.scale;
					_boxBoundsHandle.DrawHandle ();

					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Box Handle Change");
						shapeModule.scale = _boxBoundsHandle.size;
					}

					//Matrix4x4 textureTransform = transformMatrix * Matrix4x4.TRS (new Vector3 (0.0f, 0.0f, -m_BoxBoundsHandle.size.z * 0.5f), Quaternion.identity, m_BoxBoundsHandle.size);
					//OnSceneViewTextureGUI (shapeModule, s_QuadMesh, true, s_TextureMaterial, textureTransform);
				}
				else if (type == ParticleSystemShapeType.Donut)
				{
					// Radius
					EditorGUI.BeginChangeCheck ();

					_arcHandle.radius = shapeModule.radius;
					_arcHandle.angle = shapeModule.arc;
					_arcHandle.SetColorWithRadiusHandle (Color.white, 0f);
					_arcHandle.wireframeColor = Color.clear;

					using (new Handles.DrawingScope (Handles.matrix * s_ArcHandleOffsetMatrix))
						_arcHandle.DrawHandle ();

					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Donut Handle Change");
						shapeModule.radius = _arcHandle.radius;
						shapeModule.arc = _arcHandle.angle;
					}

					// Donut extents
					using (new Handles.DrawingScope (Handles.matrix * s_ArcHandleOffsetMatrix))
					{
						float excessAngle = shapeModule.arc % 360f;
						float angle = Mathf.Abs (shapeModule.arc) >= 360f ? 360f : excessAngle;

						Handles.DrawWireArc (new Vector3 (0.0f, shapeModule.donutRadius, 0.0f), Vector3.up, Vector3.forward, angle, shapeModule.radius);
						Handles.DrawWireArc (new Vector3 (0.0f, -shapeModule.donutRadius, 0.0f), Vector3.up, Vector3.forward, angle, shapeModule.radius);
						Handles.DrawWireArc (Vector3.zero, Vector3.up, Vector3.forward, angle, shapeModule.radius + shapeModule.donutRadius);
						Handles.DrawWireArc (Vector3.zero, Vector3.up, Vector3.forward, angle, shapeModule.radius - shapeModule.donutRadius);

						if (shapeModule.arc != 360.0f)
						{
							Quaternion arcRotation = Quaternion.AngleAxis (shapeModule.arc, Vector3.up);
							Vector3 capCenter = arcRotation * Vector3.forward * shapeModule.radius;
							Handles.DrawWireDisc (capCenter, arcRotation * Vector3.right, shapeModule.donutRadius);
						}
					}

					// Donut thickness
					_sphereBoundsHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;
					_sphereBoundsHandle.radius = shapeModule.donutRadius * (1.0f - shapeModule.radiusThickness);
					_sphereBoundsHandle.center = Vector3.zero;
					_sphereBoundsHandle.SetColor (s_ShapeGizmoThicknessTint);

					const float handleInterval = 90.0f;
					int numOuterRadii = Mathf.Max (1, (int)Mathf.Ceil (shapeModule.arc / handleInterval));
					Matrix4x4 donutRadiusStartMatrix = Matrix4x4.TRS (new Vector3 (shapeModule.radius, 0.0f, 0.0f), Quaternion.Euler (90.0f, 0.0f, 0.0f), Vector3.one);

					for (int i = 0; i < numOuterRadii; i++)
					{
						EditorGUI.BeginChangeCheck ();
						using (new Handles.DrawingScope (Handles.matrix * (Matrix4x4.Rotate (Quaternion.Euler (0.0f, 0.0f, handleInterval * i)) * donutRadiusStartMatrix)))
							_sphereBoundsHandle.DrawHandle ();
						if (EditorGUI.EndChangeCheck ())
						{
							Undo.RecordObject (ps, "Donut Radius Thickness Handle Change");
							shapeModule.radiusThickness = 1.0f - (_sphereBoundsHandle.radius / shapeModule.donutRadius);
						}
					}

					// Donut radius
					_sphereBoundsHandle.radius = shapeModule.donutRadius;
					_sphereBoundsHandle.SetColor (Color.white);

					for (int i = 0; i < numOuterRadii; i++)
					{
						EditorGUI.BeginChangeCheck ();
						using (new Handles.DrawingScope (Handles.matrix * (Matrix4x4.Rotate (Quaternion.Euler (0.0f, 0.0f, handleInterval * i)) * donutRadiusStartMatrix)))
							_sphereBoundsHandle.DrawHandle ();
						if (EditorGUI.EndChangeCheck ())
						{
							Undo.RecordObject (ps, "Donut Radius Handle Change");
							shapeModule.donutRadius = _sphereBoundsHandle.radius;
						}
					}

					// Texture
					//Matrix4x4 textureTransform = transformMatrix * Matrix4x4.TRS (new Vector3 (shapeModule.radius, 0.0f, 0.0f), Quaternion.Euler (180.0f, 0.0f, 180.0f), Vector3.one * shapeModule.donutRadius * 2.0f);
					//OnSceneViewTextureGUI (shapeModule, s_CircleMesh, true, s_TextureMaterial, textureTransform);
				}
				else if (type == ParticleSystemShapeType.SingleSidedEdge)
				{
					EditorGUI.BeginChangeCheck ();
					//float radius = Handles.DoSimpleEdgeHandle (Quaternion.identity, Vector3.zero, shapeModule.radius);
					float radius = Call<float> (typeof (Handles), "DoSimpleEdgeHandle", Quaternion.identity, Vector3.zero, shapeModule.radius);
					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Edge Handle Change");
						shapeModule.radius = radius;
					}
				}
				else if (type == ParticleSystemShapeType.Mesh)
				{
					Mesh mesh = shapeModule.mesh;
					if (mesh)
					{
						bool orgWireframeMode = GL.wireframe;
						GL.wireframe = true;
						s_Material.SetPass (0);
						Graphics.DrawMeshNow (mesh, transformMatrix);
						GL.wireframe = orgWireframeMode;

						//OnSceneViewTextureGUI (shapeModule, mesh, false, s_TextureMaterial, transformMatrix);
					}
				}
				else if (type == ParticleSystemShapeType.Rectangle)
				{
					EditorGUI.BeginChangeCheck ();

					_boxBoundsHandle.center = Vector3.zero;
					_boxBoundsHandle.size = new Vector3 (shapeModule.scale.x, shapeModule.scale.y, 0.0f);
					_boxBoundsHandle.DrawHandle ();

					if (EditorGUI.EndChangeCheck ())
					{
						Undo.RecordObject (ps, "Rectangle Handle Change");
						shapeModule.scale = new Vector3 (_boxBoundsHandle.size.x, _boxBoundsHandle.size.y, 0.0f);
					}

					//OnSceneViewTextureGUI (shapeModule, s_QuadMesh, true, s_TextureMaterial, transformMatrix * Matrix4x4.Scale (m_BoxBoundsHandle.size));
				}
				else if (type == ParticleSystemShapeType.Sprite)
				{
					Sprite sprite = shapeModule.sprite;
					if (sprite)
					{
						if (!_spriteMesh)
						{
							_spriteMesh = new Mesh ();
							_spriteMesh.name = "ParticleSpritePreview";
							_spriteMesh.hideFlags |= HideFlags.HideAndDontSave;
						}

						_spriteMesh.vertices = Array.ConvertAll (sprite.vertices, i => (Vector3)i);
						_spriteMesh.uv = sprite.uv;
						_spriteMesh.triangles = Array.ConvertAll (sprite.triangles, i => (int)i);

						bool orgWireframeMode = GL.wireframe;
						GL.wireframe = true;
						s_Material.SetPass (0);
						Graphics.DrawMeshNow (_spriteMesh, transformMatrix);
						GL.wireframe = orgWireframeMode;

						//OnSceneViewTextureGUI (shapeModule, m_SpriteMesh, false, s_TextureMaterial, transformMatrix);
					}
				}
			}
			Handles.color = origCol;
			Handles.matrix = orgMatrix;
		}
	}
}