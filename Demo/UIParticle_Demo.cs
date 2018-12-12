using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Coffee.UIExtensions.Demo
{
	public class UIParticle_Demo : MonoBehaviour
	{
		[SerializeField] Sprite m_Sprite;
		[SerializeField] ParticleSystem [] m_ParticleSystems;
		[SerializeField] Mask [] m_Masks;
		[SerializeField] List<Transform> m_ScalingByTransforms;
		[SerializeField] List<UIParticle> m_ScalingByUIParticles;

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

		public void SetUIParticleScale(float scale)
		{
			foreach(var uip in FindObjectsOfType<UIParticle>())
			{
				uip.scale = scale;
			}
		}

		public void LoadScene(string name)
		{
			SceneManager.LoadScene (name);
		}
	}
}