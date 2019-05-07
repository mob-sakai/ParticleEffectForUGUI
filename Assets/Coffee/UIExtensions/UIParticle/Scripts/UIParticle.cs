using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using ShaderPropertyType = Coffee.UIExtensions.UIParticle.AnimatableProperty.ShaderPropertyType;


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
		static readonly int s_IdMainTex = Shader.PropertyToID ("_MainTex");
		static readonly List<Vector3> s_Vertices = new List<Vector3> ();
		static readonly List<UIParticle> s_TempRelatables = new List<UIParticle> ();
		static readonly List<UIParticle> s_ActiveParticles = new List<UIParticle> ();


		//################################
		// Serialize Members.
		//################################
		[Tooltip ("The ParticleSystem rendered by CanvasRenderer")]
		[SerializeField] ParticleSystem m_ParticleSystem;
		[Tooltip ("The UIParticle to render trail effect")]
		[SerializeField] UIParticle m_TrailParticle;
		[HideInInspector] [SerializeField] bool m_IsTrail = false;
		[Tooltip ("Particle effect scale")]
		[SerializeField] float m_Scale = 1;
		[Tooltip ("Ignore parent scale")]
		[SerializeField] bool m_IgnoreParent = false;

		[Tooltip ("Animatable material properties. AnimationでParticleSystemのマテリアルプロパティを変更する場合、有効にしてください。")]
		[SerializeField] AnimatableProperty [] m_AnimatableProperties = new AnimatableProperty [0];

		static MaterialPropertyBlock s_Mpb;

		[System.Serializable]
		public class AnimatableProperty : ISerializationCallbackReceiver
		{
			public enum ShaderPropertyType
			{
				Color,
				Vector,
				Float,
				Range,
				Texture,
			};

			[SerializeField]
			string m_Name = "";
			[SerializeField]
			ShaderPropertyType m_Type = ShaderPropertyType.Vector;
			public int id { get; private set; }
			public ShaderPropertyType type { get { return m_Type; } }


			public void OnBeforeSerialize ()
			{
			}

			public void OnAfterDeserialize ()
			{
				id = Shader.PropertyToID (m_Name);
			}
		}



		//################################
		// Public/Protected Members.
		//################################
		public override Texture mainTexture
		{
			get
			{
				Texture tex = null;
				if (!m_IsTrail && cachedParticleSystem)
				{
					Profiler.BeginSample ("Check TextureSheetAnimation module");
					var textureSheet = cachedParticleSystem.textureSheetAnimation;
					if (textureSheet.enabled && textureSheet.mode == ParticleSystemAnimationMode.Sprites && 0 < textureSheet.spriteCount)
					{
						tex = textureSheet.GetSprite (0).texture;
					}
					Profiler.EndSample ();
				}
				if (!tex && _renderer)
				{
					Profiler.BeginSample ("Check material");
					var mat = material;
					if (mat && mat.HasProperty (s_IdMainTex))
					{
						tex = mat.mainTexture;
					}
					Profiler.EndSample ();
				}
				return tex ?? s_WhiteTexture;
			}
		}

		public override Material material
		{
			get
			{
				return _renderer
						? m_IsTrail
							? _renderer.trailMaterial
							: _renderer.sharedMaterial
						: null;
			}

			set
			{
				if (!_renderer)
				{
				}
				else if (m_IsTrail && _renderer.trailMaterial != value)
				{
					_renderer.trailMaterial = value;
					SetMaterialDirty ();
				}
				else if (!m_IsTrail && _renderer.sharedMaterial != value)
				{
					_renderer.sharedMaterial = value;
					SetMaterialDirty ();
				}
			}
		}

		/// <summary>
		/// Particle effect scale.
		/// </summary>
		public float scale { get { return _parent ? _parent.scale : m_Scale; } set { m_Scale = value; } }

		/// <summary>
		/// Should the soft mask ignore parent soft masks?
		/// </summary>
		/// <value>If set to true the soft mask will ignore any parent soft mask settings.</value>
		public bool ignoreParent
		{
			get { return m_IgnoreParent; }
			set
			{
				if (m_IgnoreParent != value)
				{
					m_IgnoreParent = value;
					OnTransformParentChanged ();
				}
			}
		}

		/// <summary>
		/// Is this the root UIParticle?
		/// </summary>
		public bool isRoot
		{
			get { return !_parent; }
		}

		/// <summary>
		/// Should this graphic be considered a target for raycasting?
		/// </summary>
		public override bool raycastTarget { get { return false; } set { base.raycastTarget = value; } }

		/// <summary>
		/// ParticleSystem.
		/// </summary>
		public ParticleSystem cachedParticleSystem { get { return m_ParticleSystem ? m_ParticleSystem : (m_ParticleSystem = GetComponent<ParticleSystem> ()); } }

		/// <summary>
		/// Perform material modification in this function.
		/// </summary>
		/// <returns>Modified material.</returns>
		/// <param name="baseMaterial">Configured Material.</param>
		public override Material GetModifiedMaterial (Material baseMaterial)
		{
			Material mat = null;
			if (!_renderer)
				mat = baseMaterial;
			else if (m_AnimatableProperties.Length == 0)
				mat = _renderer.sharedMaterial;
			else
				mat = new Material (material);

			return base.GetModifiedMaterial (mat);
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable ()
		{
			// Register.
			if (s_ActiveParticles.Count == 0)
			{
				Canvas.willRenderCanvases += UpdateMeshes;
				s_Mpb = new MaterialPropertyBlock ();
			}
			s_ActiveParticles.Add (this);

			// Reset the parent-child relation.
			GetComponentsInChildren<UIParticle> (false, s_TempRelatables);
			for (int i = s_TempRelatables.Count - 1; 0 <= i; i--)
			{
				s_TempRelatables [i].OnTransformParentChanged ();
			}
			s_TempRelatables.Clear ();

			_renderer = cachedParticleSystem ? cachedParticleSystem.GetComponent<ParticleSystemRenderer> () : null;
			if (_renderer && Application.isPlaying)
			{
				_renderer.enabled = false;
			}

			// Create objects.
			_mesh = new Mesh ();
			_mesh.MarkDynamic ();
			CheckTrail ();

			if (cachedParticleSystem)
			{
				_oldPos = cachedParticleSystem.main.scalingMode == ParticleSystemScalingMode.Local
					? rectTransform.localPosition
					: rectTransform.position;
			}

			base.OnEnable ();
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled.
		/// </summary>
		protected override void OnDisable ()
		{
			// Unregister.
			s_ActiveParticles.Remove (this);
			if (s_ActiveParticles.Count == 0)
			{
				Canvas.willRenderCanvases -= UpdateMeshes;
			}

			// Reset the parent-child relation.
			for (int i = _children.Count - 1; 0 <= i; i--)
			{
				_children [i].SetParent (_parent);
			}
			_children.Clear ();
			SetParent (null);

			// Destroy objects.
			DestroyImmediate (_mesh);
			_mesh = null;
			CheckTrail ();

			base.OnDisable ();
		}

#if UNITY_EDITOR
		/// <summary>
		/// Reset to default values.
		/// </summary>
		protected override void Reset ()
		{
			// Disable ParticleSystemRenderer on reset.
			if (cachedParticleSystem)
			{
				cachedParticleSystem.GetComponent<ParticleSystemRenderer> ().enabled = false;
			}
			base.Reset ();
		}
#endif

		/// <summary>
		/// Call to update the geometry of the Graphic onto the CanvasRenderer.
		/// </summary>
		protected override void UpdateGeometry ()
		{
		}

		/// <summary>
		/// This function is called when the parent property of the transform of the GameObject has changed.
		/// </summary>
		protected override void OnTransformParentChanged ()
		{
			UIParticle newParent = null;
			if (isActiveAndEnabled && !m_IgnoreParent)
			{
				var parentTransform = transform.parent;
				while (parentTransform && (!newParent || !newParent.enabled))
				{
					newParent = parentTransform.GetComponent<UIParticle> ();
					parentTransform = parentTransform.parent;
				}
			}
			SetParent (newParent);

			base.OnTransformParentChanged ();
		}

		/// <summary>
		/// Callback for when properties have been changed by animation.
		/// </summary>
		protected override void OnDidApplyAnimationProperties ()
		{
		}

#if UNITY_EDITOR
		/// <summary>
		/// This function is called when the script is loaded or a value is changed in the inspector(Called in the editor only).
		/// </summary>
		protected override void OnValidate ()
		{
			OnTransformParentChanged ();
			base.OnValidate ();
		}
#endif


		//################################
		// Private Members.
		//################################
		Mesh _mesh;
		ParticleSystemRenderer _renderer;
		UIParticle _parent;
		List<UIParticle> _children = new List<UIParticle> ();
		Matrix4x4 scaleaMatrix = default (Matrix4x4);
		Vector3 _oldPos;
		static ParticleSystem.Particle [] s_Particles = new ParticleSystem.Particle [4096];

		/// <summary>
		/// Update meshes.
		/// </summary>
		static void UpdateMeshes ()
		{
			for (int i = 0; i < s_ActiveParticles.Count; i++)
			{
				if (s_ActiveParticles [i])
				{
					s_ActiveParticles [i].UpdateMesh ();
				}
			}
		}

		/// <summary>
		/// Update meshe.
		/// </summary>
		void UpdateMesh ()
		{
			try
			{
				Profiler.BeginSample ("CheckTrail");
				CheckTrail ();
				Profiler.EndSample ();

				if (m_ParticleSystem && canvas)
				{
					// I do not know why, but it worked fine when setting `transform.localPosition.z` to `0.01`. (#34, #39)
					{
						Vector3 pos = rectTransform.localPosition;
						if (Mathf.Abs (pos.z) < 0.01f)
						{
							pos.z = 0.01f;
							rectTransform.localPosition = pos;
						}
					}

					var rootCanvas = canvas.rootCanvas;
					Profiler.BeginSample ("Disable ParticleSystemRenderer");
					if (Application.isPlaying)
					{
						_renderer.enabled = false;
					}
					Profiler.EndSample ();

					Profiler.BeginSample ("Make Matrix");
					ParticleSystem.MainModule main = m_ParticleSystem.main;
					scaleaMatrix = main.scalingMode == ParticleSystemScalingMode.Hierarchy
												   ? Matrix4x4.Scale (scale * Vector3.one)
												   : Matrix4x4.Scale (scale * rootCanvas.transform.localScale);
					Matrix4x4 matrix = default (Matrix4x4);
					switch (main.simulationSpace)
					{
						case ParticleSystemSimulationSpace.Local:
							matrix =
								scaleaMatrix
								* Matrix4x4.Rotate (rectTransform.rotation).inverse
								* Matrix4x4.Scale (rectTransform.lossyScale).inverse;
							break;
						case ParticleSystemSimulationSpace.World:
							matrix =
								scaleaMatrix
								* rectTransform.worldToLocalMatrix;

							bool isLocalScaling = main.scalingMode == ParticleSystemScalingMode.Local;
							Vector3 newPos = rectTransform.position;
							Vector3 delta = (newPos - _oldPos);
							_oldPos = newPos;

							if (!Mathf.Approximately (scale, 0) && 0 < delta.sqrMagnitude)
							{
								if(isLocalScaling)
								{
									var s = rootCanvas.transform.localScale * scale;
									delta.x *= 1f - 1f / s.x;
									delta.y *= 1f - 1f / s.y;
									delta.z *= 1f - 1f / s.z;
								}
								else
								{
									delta = delta * (1 - 1 / scale);
								}

								int count = m_ParticleSystem.particleCount;
								if (s_Particles.Length < count)
								{
									s_Particles = new ParticleSystem.Particle [s_Particles.Length * 2];
								}

								m_ParticleSystem.GetParticles (s_Particles);
								for (int i = 0; i < count; i++)
								{
									var p = s_Particles [i];
									p.position = p.position + delta;
									s_Particles [i] = p;
								}
								m_ParticleSystem.SetParticles (s_Particles, count);
							}
							break;
						case ParticleSystemSimulationSpace.Custom:
							break;
					}
					Profiler.EndSample ();

					_mesh.Clear ();
					if (0 < m_ParticleSystem.particleCount)
					{
						Profiler.BeginSample ("Bake Mesh");
						var cam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
							? UIParticleOverlayCamera.GetCameraForOvrelay (rootCanvas)
							: canvas.worldCamera ?? Camera.main;

						if (!cam)
						{
							return;
						}
						if (m_IsTrail)
						{
							_renderer.BakeTrailsMesh (_mesh, cam, true);
						}
						else
						{
							_renderer.BakeMesh (_mesh, cam, true);
						}
						Profiler.EndSample ();

						// Apply matrix.
						Profiler.BeginSample ("Apply matrix to position");
						_mesh.GetVertices (s_Vertices);
						var count = s_Vertices.Count;
						for (int i = 0; i < count; i++)
						{
							s_Vertices [i] = matrix.MultiplyPoint3x4 (s_Vertices [i]);
						}
						_mesh.SetVertices (s_Vertices);
						s_Vertices.Clear ();
						Profiler.EndSample ();
					}


					// Set mesh to CanvasRenderer.
					Profiler.BeginSample ("Set mesh and texture to CanvasRenderer");
					canvasRenderer.SetMesh (_mesh);
					canvasRenderer.SetTexture (mainTexture);

					// Copy the value from MaterialPropertyBlock to CanvasRenderer (#41)
					UpdateAnimatableMaterialProperties ();

					Profiler.EndSample ();
				}
			}
			catch (System.Exception e)
			{
				Debug.LogException (e);
			}
		}

		/// <summary>
		/// Checks the trail.
		/// </summary>
		void CheckTrail ()
		{
			if (isActiveAndEnabled && !m_IsTrail && m_ParticleSystem && m_ParticleSystem.trails.enabled)
			{
				if (!m_TrailParticle)
				{
					m_TrailParticle = new GameObject ("[UIParticle] Trail").AddComponent<UIParticle> ();
					var trans = m_TrailParticle.transform;
					trans.SetParent (transform);
					trans.localPosition = Vector3.zero;
					trans.localRotation = Quaternion.identity;
					trans.localScale = Vector3.one;

					m_TrailParticle._renderer = GetComponent<ParticleSystemRenderer> ();
					m_TrailParticle.m_ParticleSystem = GetComponent<ParticleSystem> ();
					m_TrailParticle.m_IsTrail = true;
				}
				m_TrailParticle.enabled = true;
			}
			else if (m_TrailParticle)
			{
				m_TrailParticle.enabled = false;
			}
		}

		/// <summary>
		/// Set the parent of the soft mask.
		/// </summary>
		/// <param name="newParent">The parent soft mask to use.</param>
		void SetParent (UIParticle newParent)
		{
			if (_parent != newParent && this != newParent)
			{
				if (_parent && _parent._children.Contains (this))
				{
					_parent._children.Remove (this);
					_parent._children.RemoveAll (x => x == null);
				}
				_parent = newParent;
			}

			if (_parent && !_parent._children.Contains (this))
			{
				_parent._children.Add (this);
			}
		}

		/// <summary>
		/// Copy the value from MaterialPropertyBlock to CanvasRenderer (#41)
		/// </summary>
		void UpdateAnimatableMaterialProperties ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (0 == m_AnimatableProperties.Length)
				return;

			_renderer.GetPropertyBlock (s_Mpb);
			for (int i = 0; i < canvasRenderer.materialCount; i++)
			{
				var mat = canvasRenderer.GetMaterial (i);
				foreach (var ap in m_AnimatableProperties)
				{
					switch (ap.type)
					{
						case ShaderPropertyType.Color:
							mat.SetColor (ap.id, s_Mpb.GetColor (ap.id));
							break;
						case ShaderPropertyType.Vector:
							mat.SetVector (ap.id, s_Mpb.GetVector (ap.id));
							break;
						case ShaderPropertyType.Float:
						case ShaderPropertyType.Range:
							mat.SetFloat (ap.id, s_Mpb.GetFloat (ap.id));
							break;
						case ShaderPropertyType.Texture:
							mat.SetTexture (ap.id, s_Mpb.GetTexture (ap.id));
							break;
					}
				}
			}
		}
	}
}