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
		[Tooltip("Particle effect scale.")]
		[SerializeField] float m_Scale = 1;


		//################################
		// Public/Protected Members.
		//################################
		public override Texture mainTexture
		{
			get
			{
				Texture tex = null;
				if (!m_IsTrail)
				{
					Profiler.BeginSample("Check TextureSheetAnimation module");
					var textureSheet = m_ParticleSystem.textureSheetAnimation;
					if (textureSheet.enabled && textureSheet.mode == ParticleSystemAnimationMode.Sprites && 0 < textureSheet.spriteCount)
					{
						tex = textureSheet.GetSprite(0).texture;
					}
					Profiler.EndSample();
				}
				if (!tex && _renderer)
				{
					Profiler.BeginSample("Check material");
					var mat = m_IsTrail
						? _renderer.trailMaterial
						: Application.isPlaying
							? _renderer.material
							: _renderer.sharedMaterial;
					if (mat && mat.HasProperty(s_IdMainTex))
					{
						tex = mat.mainTexture;
					}
					Profiler.EndSample();
				}
				return tex ?? s_WhiteTexture;
			}
		}

		/// <summary>
		/// Particle effect scale.
		/// </summary>
		public float scale { get { return _parent ? _parent.scale : m_Scale; } set { m_Scale = value; } }

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

			Canvas.willRenderCanvases += UpdateMesh;

			foreach(var c in Resources.FindObjectsOfTypeAll<Camera>())
			{
				Debug.LogFormat ("{0}, {1}, {2}, {3}, {4}",c,c.orthographic,c.orthographicSize,c.transform.localScale,c.transform.position);
			}
		}

		protected override void OnDisable()
		{
			Canvas.willRenderCanvases -= UpdateMesh;
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
		Matrix4x4 scaleaMatrix = default(Matrix4x4);

		void UpdateMesh()
		{
			try
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
					var s = scale;
					scaleaMatrix = Matrix4x4.Scale(new Vector3(s, s, s));
					Matrix4x4 matrix = default(Matrix4x4);
					switch (m_ParticleSystem.main.simulationSpace)
					{
						case ParticleSystemSimulationSpace.Local:
							matrix =
								scaleaMatrix
								* Matrix4x4.Rotate(m_ParticleSystem.transform.rotation).inverse
								* Matrix4x4.Scale(m_ParticleSystem.transform.lossyScale).inverse;
							break;
						case ParticleSystemSimulationSpace.World:
							matrix =
								scaleaMatrix
								* m_ParticleSystem.transform.worldToLocalMatrix;
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
							_renderer.BakeTrailsMesh(_mesh, cam, m_UseTransform);
						}
						else
						{
							_renderer.BakeMesh(_mesh, cam, m_UseTransform);
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
					Profiler.BeginSample("Set mesh and texture to CanvasRenderer");
					canvasRenderer.SetMesh(_mesh);
					canvasRenderer.SetTexture(mainTexture);
					Profiler.EndSample();
				}
			}
			catch(System.Exception e)
			{
				Debug.LogException(e);
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