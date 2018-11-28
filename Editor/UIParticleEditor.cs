using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Coffee.UIExtensions
{
	[CustomEditor (typeof (UIParticle))]
	[CanEditMultipleObjects]
	public class UIParticleEditor : GraphicEditor
	{
		//################################
		// Constant or Static Members.
		//################################
		static readonly GUIContent contentParticleMaterial = new GUIContent ("Particle Material", "The material for rendering particles");
		static readonly GUIContent contentTrailMaterial = new GUIContent ("Trail Material", "The material for rendering particle trails");
		static readonly List<UIParticle> s_UIParticles = new List<UIParticle> ();
		static readonly List<ParticleSystem> s_ParticleSystems = new List<ParticleSystem> ();

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

				EditorGUILayout.PropertyField (sp.GetArrayElementAtIndex (0), contentParticleMaterial);
				EditorGUILayout.PropertyField (sp.GetArrayElementAtIndex (1), contentTrailMaterial);
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

			serializedObject.ApplyModifiedProperties ();
		}

		//################################
		// Private Members.
		//################################
		SerializedProperty _spParticleSystem;
		SerializedProperty _spTrailParticle;
		SerializedProperty _spScale;
		SerializedProperty _spIgnoreParent;
	}
}