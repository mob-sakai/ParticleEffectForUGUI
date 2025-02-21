#if UNITY_2022_3_0 || UNITY_2022_3_1 || UNITY_2022_3_2 || UNITY_2022_3_3 || UNITY_2022_3_4 || UNITY_2022_3_5 || UNITY_2022_3_6 || UNITY_2022_3_7 || UNITY_2022_3_8 || UNITY_2022_3_9 || UNITY_2022_3_10
#elif UNITY_2023_1_0 || UNITY_2023_1_1 || UNITY_2023_1_2 || UNITY_2023_1_3 || UNITY_2023_1_4 || UNITY_2023_1_5 || UNITY_2023_1_6 || UNITY_2023_1_7 || UNITY_2023_1_8 || UNITY_2023_1_9
#elif UNITY_2023_1_10 || UNITY_2023_1_11 || UNITY_2023_1_12 || UNITY_2023_1_13 || UNITY_2023_1_14 || UNITY_2023_1_15 || UNITY_2023_1_16
#elif UNITY_2022_3_OR_NEWER
#define PS_BAKE_API_V2
#endif
using System;
using System.Collections.Generic;
using Coffee.UIParticleInternal;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    [Icon("Packages/com.coffee.ui-particle/Icons/UIParticleIcon.png")]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("")]
    internal class UIParticleRenderer : MaskableGraphic
    {
        private static readonly CombineInstance[] s_CombineInstances = { new CombineInstance() };
        private static readonly List<Material> s_Materials = new List<Material>(2);
        private static MaterialPropertyBlock s_Mpb;
        private static readonly Vector3[] s_Corners = new Vector3[4];
        private bool _delay;
        private int _index;
        private bool _isPrevStored;
        private bool _isTrail;
        private Bounds _lastBounds;
        private Material _materialForRendering;
        private Material _modifiedMaterial;
        private UIParticle _parent;
        private ParticleSystem _particleSystem;
        private float _prevCanvasScale;
        private Vector3 _prevPsPos;
        private Vector3 _prevScale;
        private Vector2Int _prevScreenSize;
        private bool _preWarm;
        private ParticleSystemRenderer _renderer;
        private ParticleSystem _mainEmitter;

        public override Texture mainTexture => _isTrail ? null : _particleSystem.GetTextureForSprite();

        public override bool raycastTarget => false;

        private Rect rootCanvasRect
        {
            get
            {
                s_Corners[0] = transform.TransformPoint(_lastBounds.min.x, _lastBounds.min.y, 0);
                s_Corners[1] = transform.TransformPoint(_lastBounds.min.x, _lastBounds.max.y, 0);
                s_Corners[2] = transform.TransformPoint(_lastBounds.max.x, _lastBounds.max.y, 0);
                s_Corners[3] = transform.TransformPoint(_lastBounds.max.x, _lastBounds.min.y, 0);
                if (canvas)
                {
                    var worldToLocalMatrix = canvas.rootCanvas.transform.worldToLocalMatrix;
                    for (var i = 0; i < 4; ++i)
                    {
                        s_Corners[i] = worldToLocalMatrix.MultiplyPoint(s_Corners[i]);
                    }
                }

                var corner1 = (Vector2)s_Corners[0];
                var corner2 = (Vector2)s_Corners[0];
                for (var i = 1; i < 4; ++i)
                {
                    if (s_Corners[i].x < corner1.x)
                    {
                        corner1.x = s_Corners[i].x;
                    }
                    else if (s_Corners[i].x > corner2.x)
                    {
                        corner2.x = s_Corners[i].x;
                    }

                    if (s_Corners[i].y < corner1.y)
                    {
                        corner1.y = s_Corners[i].y;
                    }
                    else if (s_Corners[i].y > corner2.y)
                    {
                        corner2.y = s_Corners[i].y;
                    }
                }

                return new Rect(corner1, corner2 - corner1);
            }
        }

        public override Material materialForRendering
        {
            get
            {
                if (!_materialForRendering)
                {
                    _materialForRendering = base.materialForRendering;
                }

                return _materialForRendering;
            }
        }

        public void Reset(int index = -1)
        {
            if (_renderer)
            {
                _renderer.enabled = true;
            }

            _parent = null;
            _particleSystem = null;
            _renderer = null;
            _mainEmitter = null;
            if (0 <= index)
            {
                _index = index;
            }

            //_emitter = null;
            if (this && isActiveAndEnabled)
            {
                material = null;
                canvasRenderer.Clear();
                _lastBounds = new Bounds();
                enabled = false;
            }
            else
            {
                MaterialRepository.Release(ref _modifiedMaterial);
                _materialForRendering = null;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            hideFlags = UIParticleProjectSettings.globalHideFlags;
            if (!s_CombineInstances[0].mesh)
            {
                s_CombineInstances[0].mesh = new Mesh
                {
                    name = "[UIParticleRenderer] Combine Instance Mesh",
                    hideFlags = HideFlags.HideAndDontSave
                };
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MaterialRepository.Release(ref _modifiedMaterial);
            _materialForRendering = null;
            _isPrevStored = false;
        }

        public static UIParticleRenderer AddRenderer(UIParticle parent, int index)
        {
            // Create renderer object.
            var go = new GameObject("[generated] UIParticleRenderer", typeof(UIParticleRenderer))
            {
                hideFlags = UIParticleProjectSettings.globalHideFlags,
                layer = parent.gameObject.layer
            };

            // Set parent.
            var transform = go.transform;
            transform.SetParent(parent.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            // Add renderer component.
            var renderer = go.GetComponent<UIParticleRenderer>();
            renderer._parent = parent;
            renderer._index = index;

            return renderer;
        }

        /// <summary>
        /// Perform material modification in this function.
        /// </summary>
        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!IsActive() || !_parent)
            {
                MaterialRepository.Release(ref _modifiedMaterial);
                return baseMaterial;
            }

            var modifiedMaterial = base.GetModifiedMaterial(baseMaterial);

            //
            var texture = mainTexture;
            if (texture == null && _parent.m_AnimatableProperties.Length == 0)
            {
                MaterialRepository.Release(ref _modifiedMaterial);
                return modifiedMaterial;
            }

            var hash = new Hash128(
                modifiedMaterial ? (uint)modifiedMaterial.GetInstanceID() : 0,
                texture ? (uint)texture.GetInstanceID() : 0,
                0 < _parent.m_AnimatableProperties.Length ? (uint)GetInstanceID() : 0,
#if UNITY_EDITOR
                (uint)EditorJsonUtility.ToJson(modifiedMaterial).GetHashCode()
#else
                0
#endif
            );
            if (!MaterialRepository.Valid(hash, _modifiedMaterial))
            {
                MaterialRepository.Get(hash, ref _modifiedMaterial, x => new Material(x.mat)
                {
                    hideFlags = HideFlags.HideAndDontSave,
                    mainTexture = x.texture ? x.texture : x.mat.mainTexture
                }, (mat: modifiedMaterial, texture));
            }

            return _modifiedMaterial;
        }

        public void Set(UIParticle parent, ParticleSystem ps, bool isTrail, ParticleSystem mainEmitter)
        {
            _parent = parent;
            maskable = parent.maskable;

            gameObject.layer = parent.gameObject.layer;

            _particleSystem = ps;
            _preWarm = _particleSystem.main.prewarm;

#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                if (_particleSystem.isPlaying || _preWarm)
                {
                    _particleSystem.Clear();
                    _particleSystem.Pause();
                }
            }

            ps.TryGetComponent(out _renderer);
            _renderer.enabled = false;
            _isTrail = isTrail;
            _renderer.GetSharedMaterials(s_Materials);
            material = s_Materials[isTrail ? 1 : 0];
            s_Materials.Clear();

            // Support sprite.
            var tsa = ps.textureSheetAnimation;
            if (tsa.mode == ParticleSystemAnimationMode.Sprites && tsa.uvChannelMask == 0)
            {
                tsa.uvChannelMask = UVChannelFlags.UV0;
            }

            _prevScale = GetWorldScale();
            _prevPsPos = _particleSystem.transform.position;
            _prevScreenSize = new Vector2Int(Screen.width, Screen.height);
            _prevCanvasScale = canvas ? canvas.scaleFactor : 1f;
            _delay = true;
            _mainEmitter = mainEmitter;

            canvasRenderer.SetTexture(null);

            enabled = true;
        }

        public void UpdateMesh(Camera bakeCamera)
        {
            // No particle to render: Clear mesh.
            if (
                !isActiveAndEnabled || !_particleSystem || !_parent
                || !canvasRenderer || !canvas || !bakeCamera
                || _parent.meshSharing == UIParticle.MeshSharing.Replica
                || !transform.lossyScale.GetScaled(_parent.scale3DForCalc).IsVisible() // Scale is not visible.
                || (!_particleSystem.IsAlive() && !_particleSystem.isPlaying) // No particle.
                || (_isTrail && !_particleSystem.trails.enabled) // Trail, but it is not enabled.
#if UNITY_2018_3_OR_NEWER
                || canvasRenderer.GetInheritedAlpha() <
                0.01f // #102: Do not bake particle system to mesh when the alpha is zero.
#endif
            )
            {
                Profiler.BeginSample("[UIParticleRenderer] Clear Mesh");
                workerMesh.Clear();
                canvasRenderer.SetMesh(workerMesh);
                _lastBounds = new Bounds();
                Profiler.EndSample();

                return;
            }

            var main = _particleSystem.main;
            var scale = GetWorldScale();
            var psPos = _particleSystem.transform.position;

            // Simulate particles.
            Profiler.BeginSample("[UIParticle] Bake Mesh > Simulate Particles");
            if (!_isTrail && _parent.canSimulate && !_mainEmitter)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    SimulateForEditor(psPos - _prevPsPos, scale);
                }
                else
#endif
                {
                    ResolveResolutionChange(psPos, scale);
                    Simulate(scale, _parent.isPaused || _delay);

                    if (_delay && !_parent.isPaused)
                    {
                        Simulate(scale, _parent.isPaused);
                    }

                    // When the ParticleSystem simulation is complete, stop it.
                    if (!main.loop
                        && main.duration <= _particleSystem.time
                        && (_particleSystem.IsAlive() || _particleSystem.particleCount == 0)
                       )
                    {
                        _particleSystem.Stop(false);
                    }
                }

                _prevScale = scale;
                _prevPsPos = psPos;
                _delay = false;
            }

            Profiler.EndSample();

            // Bake mesh.
            Profiler.BeginSample("[UIParticleRenderer] Bake Mesh");
            s_CombineInstances[0].mesh.Clear(false);

            // Assertion failed on expression: 'ps->array_size()' #278
            var extends = s_CombineInstances[0].mesh.bounds.extents.x;
            if (!float.IsNaN(extends) && !float.IsInfinity(extends) && 0 < extends)
            {
                s_CombineInstances[0].mesh.RecalculateBounds();
            }

            if (_isTrail && _parent.canSimulate && 0 < _particleSystem.particleCount)
            {
#if PS_BAKE_API_V2
                _renderer.BakeTrailsMesh(s_CombineInstances[0].mesh, bakeCamera,
                    ParticleSystemBakeMeshOptions.BakeRotationAndScale);
#else
                _renderer.BakeTrailsMesh(s_CombineInstances[0].mesh, bakeCamera, true);
#endif
            }
            else if (!_isTrail && _renderer.CanBakeMesh())
            {
                _particleSystem.ValidateShape();
#if PS_BAKE_API_V2
                _renderer.BakeMesh(s_CombineInstances[0].mesh, bakeCamera,
                    ParticleSystemBakeMeshOptions.BakeRotationAndScale);
#else
                _renderer.BakeMesh(s_CombineInstances[0].mesh, bakeCamera, true);
#endif
            }

            // Too many vertices to render.
            if (65535 <= s_CombineInstances[0].mesh.vertexCount)
            {
                Debug.LogErrorFormat(this,
                    "Too many vertices to render. index={0}, isTrail={1}, vertexCount={2}(>=65535)",
                    _index,
                    _isTrail,
                    s_CombineInstances[0].mesh.vertexCount
                );
                s_CombineInstances[0].mesh.Clear(false);
            }

            Profiler.EndSample();

            // Combine mesh to transform. ([ParticleSystem local ->] world -> renderer local)
            Profiler.BeginSample("[UIParticleRenderer] Combine Mesh");
            if (_parent.canSimulate)
            {
                if (_parent.positionMode == UIParticle.PositionMode.Absolute)
                {
                    s_CombineInstances[0].transform =
                        canvasRenderer.transform.worldToLocalMatrix
                        * GetWorldMatrix(psPos, scale);
                }
                else
                {
                    var diff = _particleSystem.transform.position - _parent.transform.position;
                    s_CombineInstances[0].transform =
                        canvasRenderer.transform.worldToLocalMatrix
                        * Matrix4x4.Translate(diff.GetScaled(scale - Vector3.one))
                        * GetWorldMatrix(psPos, scale);
                }

                workerMesh.CombineMeshes(s_CombineInstances, true, true);

                workerMesh.RecalculateBounds();
                var bounds = workerMesh.bounds;
                var center = bounds.center;
                center.z = 0;
                bounds.center = center;
                var extents = bounds.extents;
                extents.z = 0;
                bounds.extents = extents;
                workerMesh.bounds = bounds;
                _lastBounds = bounds;

                // Convert linear color to gamma color.
                if (UIParticleProjectSettings.enableLinearToGamma && canvas.ShouldGammaToLinearInMesh())
                {
                    workerMesh.LinearToGamma();
                }

                var components = InternalListPool<Component>.Rent();
                GetComponents(typeof(IMeshModifier), components);
                for (var i = 0; i < components.Count; i++)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    ((IMeshModifier)components[i]).ModifyMesh(workerMesh);
#pragma warning restore CS0618 // Type or member is obsolete
                }

                InternalListPool<Component>.Return(ref components);
            }

            Profiler.EndSample();

            // Update animatable material properties.
            Profiler.BeginSample("[UIParticleRenderer] Update Animatable Material Properties");
            UpdateMaterialProperties();
            Profiler.EndSample();

            // Get grouped renderers.
            Profiler.BeginSample("[UIParticleRenderer] Set Mesh");
            var renderers = InternalListPool<UIParticleRenderer>.Rent();
            if (_parent.useMeshSharing)
            {
                UIParticleUpdater.GetGroupedRenderers(_parent.groupId, _index, renderers);
            }

            for (var i = 0; i < renderers.Count; i++)
            {
                var r = renderers[i];
                if (r == this) continue;

                r.canvasRenderer.SetMesh(workerMesh);
                r._lastBounds = _lastBounds;
                r.canvasRenderer.materialCount = 1;
                r.canvasRenderer.SetMaterial(materialForRendering, 0);
            }

            InternalListPool<UIParticleRenderer>.Return(ref renderers);

            if (_parent.canRender)
            {
                canvasRenderer.SetMesh(workerMesh);
            }
            else
            {
                workerMesh.Clear();
            }

            Profiler.EndSample();
        }

        public override void SetMaterialDirty()
        {
            _materialForRendering = null;
            base.SetMaterialDirty();
        }

        /// <summary>
        /// Call to update the geometry of the Graphic onto the CanvasRenderer.
        /// </summary>
        protected override void UpdateGeometry()
        {
        }

        public override void Cull(Rect clipRect, bool validRect)
        {
            var cull = _lastBounds.extents == Vector3.zero
                       || !validRect
                       || !clipRect.Overlaps(rootCanvasRect, true);
            if (canvasRenderer.cull == cull) return;

            canvasRenderer.cull = cull;
            UISystemProfilerApi.AddMarker("MaskableGraphic.cullingChanged", this);
            onCullStateChanged.Invoke(cull);
            OnCullingChanged();
        }

        private Vector3 GetWorldScale()
        {
            Profiler.BeginSample("[UIParticleRenderer] GetWorldScale");
            var scale = _parent.scale3DForCalc.GetScaled(_parent.parentScale);

            if (_parent.autoScalingMode == UIParticle.AutoScalingMode.UIParticle
                && _particleSystem.main.scalingMode == ParticleSystemScalingMode.Local
                && _parent.canvas)
            {
                scale = scale.GetScaled(_parent.canvas.rootCanvas.transform.localScale);
            }

            Profiler.EndSample();
            return scale;
        }

        private Matrix4x4 GetWorldMatrix(Vector3 psPos, Vector3 scale)
        {
            var space = _particleSystem.GetActualSimulationSpace();
            if (_isTrail && _particleSystem.trails.worldSpace)
            {
                space = ParticleSystemSimulationSpace.World;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                switch (space)
                {
                    case ParticleSystemSimulationSpace.World:
                        return Matrix4x4.Translate(psPos)
                               * Matrix4x4.Scale(scale)
                               * Matrix4x4.Translate(-psPos);
                }
            }
#endif

            switch (space)
            {
                case ParticleSystemSimulationSpace.Local:
                    return Matrix4x4.Translate(psPos)
                           * Matrix4x4.Scale(scale);
                case ParticleSystemSimulationSpace.World:
                    if (_isTrail)
                    {
                        return Matrix4x4.Translate(psPos)
                               * Matrix4x4.Scale(scale)
                               * Matrix4x4.Translate(-psPos);
                    }

                    if (_mainEmitter)
                    {
                        if (_mainEmitter.IsLocalSpace())
                        {
                            return Matrix4x4.Translate(psPos)
                                   * Matrix4x4.Scale(scale)
                                   * Matrix4x4.Translate(-psPos);
                        }
                        else
                        {
                            psPos = _particleSystem.transform.position - _mainEmitter.transform.position;
                            return Matrix4x4.Translate(psPos)
                                   * Matrix4x4.Scale(scale)
                                   * Matrix4x4.Translate(-psPos);
                        }
                    }

                    return Matrix4x4.Scale(scale);
                case ParticleSystemSimulationSpace.Custom:
                    return Matrix4x4.Translate(_particleSystem.main.customSimulationSpace.position.GetScaled(scale))
                           * Matrix4x4.Scale(scale);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// For world simulation, interpolate particle positions when the screen size is changed.
        /// </summary>
        /// <param name="psPos"></param>
        /// <param name="scale"></param>
        private void ResolveResolutionChange(Vector3 psPos, Vector3 scale)
        {
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            var isWorldSpace = _particleSystem.IsWorldSpace();
            var canvasScale = _parent.canvas ? _parent.canvas.scaleFactor : 1f;
            var resolutionChanged = _prevScreenSize != screenSize
                                    || !Mathf.Approximately(_prevCanvasScale, canvasScale);
            if (resolutionChanged && isWorldSpace && _isPrevStored)
            {
                // Update particle array size and get particles.
                var size = _particleSystem.particleCount;
                var particles = ParticleSystemExtensions.GetParticleArray(size);
                _particleSystem.GetParticles(particles, size);

                // Resolution resolver:
                // (psPos / scale) / (prevPsPos / prevScale) -> psPos * scale.inv * prevPsPos.inv * prevScale
                var modifier = psPos.GetScaled(
                    scale.Inverse(),
                    _prevPsPos.Inverse(),
                    _prevScale);
                for (var i = 0; i < size; i++)
                {
                    var particle = particles[i];
                    particle.position = particle.position.GetScaled(modifier);
                    particles[i] = particle;
                }

                _particleSystem.SetParticles(particles, size);

                // Delay: Do not progress in the frame where the resolution has been changed.
                _delay = true;
                _prevScale = scale;
                _prevPsPos = psPos;
                _isPrevStored = true;
            }

            _prevCanvasScale = canvas ? canvas.scaleFactor : 1f;
            _prevScreenSize = screenSize;
        }

        private void Simulate(Vector3 scale, bool paused)
        {
            var main = _particleSystem.main;
            var deltaTime = paused
                ? 0
                : main.useUnscaledTime
                    ? Time.unscaledDeltaTime
                    : Time.deltaTime;
            deltaTime *= _parent.timeScaleMultiplier;

            // Pre-warm:
            if (0 < deltaTime && _preWarm)
            {
                deltaTime += main.duration;
                _preWarm = false;
            }

            // get world position.
            var isLocalSpace = _particleSystem.IsLocalSpace();
            var psTransform = _particleSystem.transform;
            var originLocalPosition = psTransform.localPosition;
            var originLocalRotation = psTransform.localRotation;
            var originWorldPosition = psTransform.position;
            var originWorldRotation = psTransform.rotation;
            var emission = _particleSystem.emission;
            var rateOverDistance = emission.enabled
                                   && 0 < emission.rateOverDistance.constant
                                   && 0 < emission.rateOverDistanceMultiplier;
            if (rateOverDistance && !paused && _isPrevStored)
            {
                // (For rate-over-distance emission,) Move to previous scaled position, simulate (delta = 0).
                var prevScaledPos = isLocalSpace
                    ? _prevPsPos
                    : _prevPsPos.GetScaled(_prevScale.Inverse());
                psTransform.SetPositionAndRotation(prevScaledPos, originWorldRotation);
                _particleSystem.Simulate(0, false, false, false);
            }

            // Move to scaled position, simulate, revert to origin position.
            var scaledPos = isLocalSpace
                ? originWorldPosition
                : originWorldPosition.GetScaled(scale.Inverse());
            psTransform.SetPositionAndRotation(scaledPos, originWorldRotation);
            _particleSystem.Simulate(deltaTime, false, false, false);
            psTransform.localPosition = originLocalPosition;
            psTransform.localRotation = originLocalRotation;
        }

#if UNITY_EDITOR
        private void SimulateForEditor(Vector3 diffPos, Vector3 scale)
        {
            // Extra world simulation.
            var isWorldSpace = _particleSystem.IsWorldSpace();
            if (isWorldSpace && 0 < Vector3.SqrMagnitude(diffPos))
            {
                Profiler.BeginSample("[UIParticle] Bake Mesh > Extra world simulation");
                diffPos.x *= 1f - 1f / Mathf.Max(0.001f, scale.x);
                diffPos.y *= 1f - 1f / Mathf.Max(0.001f, scale.y);
                diffPos.z *= 1f - 1f / Mathf.Max(0.001f, scale.z);

                var size = _particleSystem.particleCount;
                var particles = ParticleSystemExtensions.GetParticleArray(size);
                _particleSystem.GetParticles(particles, size);
                for (var i = 0; i < size; i++)
                {
                    var p = particles[i];
                    p.position += diffPos;
                    particles[i] = p;
                }

                _particleSystem.SetParticles(particles, size);
                Profiler.EndSample();
            }
        }
#endif

        private void UpdateMaterialProperties()
        {
            if (_parent.m_AnimatableProperties.Length == 0) return;

            if (s_Mpb == null)
            {
                s_Mpb = new MaterialPropertyBlock();
            }

            _renderer.GetPropertyBlock(s_Mpb);
            if (s_Mpb.isEmpty) return;

            // #41: Copy the value from MaterialPropertyBlock to CanvasRenderer
            if (!materialForRendering) return;

            for (var i = 0; i < _parent.m_AnimatableProperties.Length; i++)
            {
                var ap = _parent.m_AnimatableProperties[i];
                ap.UpdateMaterialProperties(materialForRendering, s_Mpb);
            }

            s_Mpb.Clear();
        }
    }
}
