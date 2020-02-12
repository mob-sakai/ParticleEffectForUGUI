using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Coffee.UIExtensions.Demo
{
	public class UIParticle_Demo : MonoBehaviour
	{
		[SerializeField] ParticleSystem [] m_ParticleSystems = new ParticleSystem [0];
		[SerializeField] List<Transform> m_ScalingByTransforms = new List<Transform> ();
		[SerializeField] List<UIParticle> m_ScalingByUIParticles = new List<UIParticle> ();

		public void SetTimeScale (float scale)
		{
			Time.timeScale = scale;
		}

		public void EnableTrailRibbon (bool ribbonMode)
		{
			foreach (var p in m_ParticleSystems)
			{
				var trails = p.trails;
				trails.mode = ribbonMode ? ParticleSystemTrailMode.Ribbon : ParticleSystemTrailMode.PerParticle;
			}
		}

		public void EnableSprite (bool enabled)
		{
			foreach (var p in m_ParticleSystems)
			{
				var tex = p.textureSheetAnimation;
				tex.enabled = enabled;
			}
		}

		public void EnableMask (bool enabled)
		{
			foreach (var m in FindObjectsOfType<Mask> ())
			{
				m.enabled = enabled;
			}
		}

		public void EnableMask2D (bool enabled)
		{
			foreach (var m in FindObjectsOfType<RectMask2D> ())
			{
				m.enabled = enabled;
			}
		}

		public void SetScale (float scale)
		{
			m_ScalingByTransforms.ForEach (x => x.localScale = Vector3.one * (10 * scale));
			m_ScalingByUIParticles.ForEach (x => x.scale = scale);
		}

		public void SetUIParticleScale (float scale)
		{
			foreach (var uip in FindObjectsOfType<UIParticle> ())
			{
				uip.scale = scale;
			}
		}

		public void LoadScene (string name)
		{
			SceneManager.LoadScene (name);
		}

		public void PlayAllParticleEffect ()
		{
			foreach (var animator in FindObjectsOfType<Animator> ())
			{
				animator.Play ("Play");
			}

			foreach (var particle in FindObjectsOfType<ParticleSystem> ())
			{
				particle.Play ();
			}
		}

		public void SetWorldSpase (bool flag)
		{
			if (flag)
			{
				GetComponent<Canvas> ().renderMode = RenderMode.ScreenSpaceCamera;
				GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
				transform.rotation = Quaternion.Euler (new Vector3 (0, 6, 0));
			}
		}

		public void SetScreenSpase (bool flag)
		{
			if (flag)
			{
				GetComponent<Canvas> ().renderMode = RenderMode.ScreenSpaceCamera;
			}
		}

		public void SetOverlay (bool flag)
		{
			if (flag)
			{
				GetComponent<Canvas> ().renderMode = RenderMode.ScreenSpaceOverlay;
			}
		}
	}
}