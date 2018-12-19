using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// </summary>
	[ExecuteInEditMode]
	public class UIParticleOverlayCamera : MonoBehaviour
	{
		//################################
		// Public/Protected Members.
		//################################
		/// <summary>
		/// Get instance object.
		/// If instance does not exist, Find instance in scene, or create new one.
		/// </summary>
		public static UIParticleOverlayCamera instance
		{
			get
			{
				// Find instance in scene, or create new one.
				if (object.ReferenceEquals (s_Instance, null))
				{
					s_Instance = FindObjectOfType<UIParticleOverlayCamera> () ?? new GameObject (typeof (UIParticleOverlayCamera).Name, typeof (UIParticleOverlayCamera)).GetComponent<UIParticleOverlayCamera> ();
					s_Instance.gameObject.SetActive (true);
					s_Instance.enabled = true;
				}
				return s_Instance;
			}
		}

		public static Camera GetCameraForOvrelay (Canvas canvas)
		{
			var i = instance;
			var rt = canvas.rootCanvas.transform as RectTransform;
			var cam = i.cameraForOvrelay;
			var trans = i.transform;
			cam.enabled = false;

			var pos = rt.localPosition;
			cam.orthographic = true;
			cam.orthographicSize = Mathf.Max (pos.x, pos.y);
			cam.nearClipPlane = 0.3f;
			cam.farClipPlane = 1000f;
			pos.z -= 100;
			trans.localPosition = pos;

			return cam;
		}

		//################################
		// Private Members.
		//################################
		Camera cameraForOvrelay { get { return m_Camera ? m_Camera : (m_Camera = GetComponent<Camera> ()) ? m_Camera : (m_Camera = gameObject.AddComponent<Camera> ()); } }
		Camera m_Camera;
		static UIParticleOverlayCamera s_Instance;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake ()
		{
			// Hold the instance.
			if (s_Instance == null)
			{
				s_Instance = GetComponent<UIParticleOverlayCamera> ();
			}
			// If the instance is duplicated, destroy itself.
			else if (s_Instance != this)
			{
				UnityEngine.Debug.LogWarning ("Multiple " + typeof (UIParticleOverlayCamera).Name + " in scene.", this.gameObject);
				enabled = false;
#if UNITY_EDITOR

				if (!Application.isPlaying)
				{
					DestroyImmediate (gameObject);
				}
				else
#endif
				{
					Destroy (gameObject);
				}
				return;
			}

			cameraForOvrelay.enabled = false;

			// Singleton has DontDestroy flag.
			if (Application.isPlaying)
			{
				DontDestroyOnLoad (gameObject);
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		void OnDestroy ()
		{
			// Clear instance on destroy.
			if (s_Instance == this)
			{
				s_Instance = null;
			}
		}
	}
}