using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;


namespace Coffee.UIExtensions
{
	/// <summary>
	/// Render maskable and sortable particle effect ,without Camera, RenderTexture or Canvas.
	/// </summary>
	[ExecuteInEditMode]
	public class UIParticle : MaskableGraphic
	{
		//################################
		// Constant or Readonly Static Members.
		//################################
		static readonly int s_IdMainTex = Shader.PropertyToID("_MainTex");
		static readonly List<Vector3> s_Vertices = new List<Vector3>();


		//################################
		// Serialize Members.
		//################################
		[Tooltip("The ParticleSystem rendered by CanvasRenderer")]
		[SerializeField] ParticleSystem m_ParticleSystem;
		[Tooltip("The UIParticle to render trail effect")]
		[SerializeField] UIParticle m_TrailParticle;
		[HideInInspector] [SerializeField] bool m_IsTrail = false;


		//################################
		// Public/Protected Members.
		//################################
		public override Texture mainTexture
		{
			get
			{
				var mat = _renderer ? _renderer.sharedMaterial : defaultGraphicMaterial;
				return mat && mat.HasProperty(s_IdMainTex) ? mat.mainTexture : s_WhiteTexture; ;
			}
		}

		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			return base.GetModifiedMaterial(_renderer ? _renderer.sharedMaterial : baseMaterial);
		}

		protected override void OnEnable()
		{
			m_ParticleSystem = m_ParticleSystem ? m_ParticleSystem : GetComponent<ParticleSystem>();
			_renderer = m_ParticleSystem ? m_ParticleSystem.GetComponent<ParticleSystemRenderer>() : null;

			_mesh = new Mesh();
			_mesh.MarkDynamic();
			CheckTrail();
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			DestroyImmediate(_mesh);
			_mesh = null;
			CheckTrail();
			base.OnDisable();
		}

		protected override void UpdateGeometry()
		{
		}

		//################################
		// Private Members.
		//################################
		Mesh _mesh;
		ParticleSystemRenderer _renderer;

		void Update()
		{
			Profiler.BeginSample("CheckTrail");
			CheckTrail();
			Profiler.EndSample();

			if (m_ParticleSystem)
			{
				Profiler.BeginSample("Disable ParticleSystemRenderer");
				if (Application.isPlaying)
				{
					_renderer.enabled = false;
				}
				Profiler.EndSample();

				Profiler.BeginSample("Make Matrix");
				var cam = canvas.worldCamera ?? Camera.main;
				bool useTransform = false;
				Matrix4x4 matrix = default(Matrix4x4);
				switch (m_ParticleSystem.main.simulationSpace)
				{
					case ParticleSystemSimulationSpace.Local:
						matrix =
						Matrix4x4.Rotate(m_ParticleSystem.transform.rotation).inverse
						 * Matrix4x4.Scale(m_ParticleSystem.transform.lossyScale).inverse;
						useTransform = true;
						break;
					case ParticleSystemSimulationSpace.World:
						matrix = m_ParticleSystem.transform.worldToLocalMatrix;
						break;
					case ParticleSystemSimulationSpace.Custom:
						break;
				}
				Profiler.EndSample();

				_mesh.Clear();
				if (0 < m_ParticleSystem.particleCount)
				{
					Profiler.BeginSample("Bake Mesh");
					if (m_IsTrail)
					{
						_renderer.BakeTrailsMesh(_mesh, cam, useTransform);
					}
					else
					{
						_renderer.BakeMesh(_mesh, cam, useTransform);
					}
					Profiler.EndSample();

					// Apply matrix.
					Profiler.BeginSample("Apply matrix to position");
					_mesh.GetVertices(s_Vertices);
					var count = s_Vertices.Count;
					for (int i = 0; i < count; i++)
					{
						s_Vertices[i] = matrix.MultiplyPoint3x4(s_Vertices[i]);
					}
					_mesh.SetVertices(s_Vertices);
					s_Vertices.Clear();
					Profiler.EndSample();
				}


				// Set mesh to CanvasRenderer.
				Profiler.BeginSample("Set mesh to CanvasRenderer");
				canvasRenderer.SetMesh(_mesh);
				Profiler.EndSample();
			}
		}

		void CheckTrail()
		{
			if (isActiveAndEnabled && !m_IsTrail && m_ParticleSystem && m_ParticleSystem.trails.enabled)
			{
				if (!m_TrailParticle)
				{
					m_TrailParticle = new GameObject("[UIParticle] Trail").AddComponent<UIParticle>();
					var trans = m_TrailParticle.transform;
					trans.SetParent(transform);
					trans.localPosition = Vector3.zero;
					trans.localRotation = Quaternion.identity;
					trans.localScale = Vector3.one;

					m_TrailParticle._renderer = GetComponent<ParticleSystemRenderer>();
					m_TrailParticle.m_ParticleSystem = GetComponent<ParticleSystem>();
					m_TrailParticle.m_IsTrail = true;
				}
				m_TrailParticle.enabled = true;
			}
			else if (m_TrailParticle)
			{
				m_TrailParticle.enabled = false;
			}
		}
	}
}