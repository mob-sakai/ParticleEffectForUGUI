using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Coffee.UIExtensions
{
	[CustomEditor(typeof(UIParticle))]
	[CanEditMultipleObjects]
	public class UIParticleEditor : GraphicEditor
	{
		//################################
		// Constant or Static Members.
		//################################
		static readonly GUIContent contentParticleMaterial = new GUIContent("Particle Material", "The material for rendering particles");
		static readonly GUIContent contentTrailMaterial = new GUIContent("Trail Material", "The material for rendering particle trails");


		//################################
		// Public/Protected Members.
		//################################
		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			_spParticleSystem = serializedObject.FindProperty("m_ParticleSystem");
			_spTrailParticle = serializedObject.FindProperty("m_TrailParticle");
		}

		/// <summary>
		/// Implement this function to make a custom inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_spParticleSystem);
			EditorGUI.indentLevel++;
			var ps = _spParticleSystem.objectReferenceValue as ParticleSystem;
			if (ps)
			{
				var pr = ps.GetComponent<ParticleSystemRenderer>();
				var sp = new SerializedObject(pr).FindProperty("m_Materials");

				EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(0), contentParticleMaterial);
				EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(1), contentTrailMaterial);
				sp.serializedObject.ApplyModifiedProperties();

				if(!Application.isPlaying && pr.enabled)
				{
					EditorGUILayout.HelpBox("ParticleSystemRenderer will be disable on playing.", MessageType.Info);
				}
			}
			EditorGUI.indentLevel--;

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(_spTrailParticle);
			EditorGUI.EndDisabledGroup();

			if ((target as UIParticle).GetComponentsInParent<UIParticle> (false).Length == 1)
			{
				EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_Scale"));
			}
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_IgnoreParent"));

			serializedObject.ApplyModifiedProperties();
		}

		//################################
		// Private Members.
		//################################
		SerializedProperty _spParticleSystem;
		SerializedProperty _spTrailParticle;
	}
}