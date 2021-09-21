using System;
using System.Collections.Generic;
using Coffee.UIParticleExtensions;
using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIExtensions
{
    internal static class WorldPositionCache
    {
        private static readonly Dictionary<Transform, Vector3> _cache = new Dictionary<Transform, Vector3>();
        private static readonly HashSet<Transform> _tmp = new HashSet<Transform>();

        public static bool TryGetCachedWorldPosition(Transform transform, out Vector3 wp)
        {
            return _cache.TryGetValue(transform, out wp);
        }

        public static void CacheWorldPosition(Transform transform, Vector3 wp)
        {
            _cache[transform] = wp;
        }

        public static void Refresh()
        {
            foreach (var tr in _cache.Keys)
            {
                if (!tr)
                    _tmp.Add(tr);
            }

            foreach (var tr in _tmp)
            {
                _cache.Remove(tr);
            }

            _tmp.Clear();
        }
    }

    internal static class UIParticleUpdater
    {
        static readonly List<UIParticle> s_ActiveParticles = new List<UIParticle>();
        static MaterialPropertyBlock s_Mpb;
        private static int frameCount = 0;


        public static void Register(UIParticle particle)
        {
            if (!particle) return;
            s_ActiveParticles.Add(particle);
        }

        public static void Unregister(UIParticle particle)
        {
            if (!particle) return;
            s_ActiveParticles.Remove(particle);
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            MeshHelper.Init();
            MeshPool.Init();
            CombineInstanceArrayPool.Init();

            Canvas.willRenderCanvases -= Refresh;
            Canvas.willRenderCanvases += Refresh;
        }

        private static void Refresh()
        {
            // Do not allow it to be called in the same frame.
            if (frameCount == Time.frameCount) return;
            frameCount = Time.frameCount;

            Profiler.BeginSample("[UIParticle] Refresh");
            for (var i = 0; i < s_ActiveParticles.Count; i++)
            {
                try
                {
                    Refresh(s_ActiveParticles[i]);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            Profiler.EndSample();
        }

        private static void Refresh(UIParticle particle)
        {
            if (!particle || !particle.bakedMesh || !particle.canvas || !particle.canvasRenderer) return;

            Profiler.BeginSample("[UIParticle] Bake mesh");
            BakeMesh(particle);
            Profiler.EndSample();

            Profiler.BeginSample("[UIParticle] Set mesh to CanvasRenderer");
            particle.canvasRenderer.SetMesh(particle.bakedMesh);
            Profiler.EndSample();

            Profiler.BeginSample("[UIParticle] Update Animatable Material Properties");
            particle.UpdateMaterialProperties();
            Profiler.EndSample();

            WorldPositionCache.Refresh();
        }

        private static void BakeMesh(UIParticle particle)
        {
            // Clear mesh before bake.
            Profiler.BeginSample("[UIParticle] Bake Mesh > Clear mesh before bake");
            MeshHelper.Clear();
            particle.bakedMesh.Clear(false);
            Profiler.EndSample();

#if UNITY_2018_3_OR_NEWER
            // #102: Do not bake particle system to mesh when the alpha is zero.
            if (Mathf.Approximately(particle.canvasRenderer.GetInheritedAlpha(), 0)) return;
#endif

            // No particle to render (scale).
            {
                var scale = Vector3.Scale(particle.transform.lossyScale, particle.scale3D);
                if (Mathf.Approximately(scale.x * scale.y * scale.z, 0)) return;
            }


            var scale3D = particle.scale3D
                .GetScaled(particle.transform.lossyScale)
                .GetScaled(particle.canvas.transform.lossyScale.Inverse());

            // Correct sub-particle systems.
            var subTransMats = new Dictionary<ParticleSystem, Matrix4x4>();
            for (var i = 0; i < particle.particles.Count; i++)
            {
                var currentPs = particle.particles[i];
                if (!currentPs) continue;

                var subEmitters = currentPs.subEmitters;
                if (!subEmitters.enabled || subEmitters.subEmittersCount == 0) continue;

                var currentTr = currentPs.transform;
                for (var j = 0; j < subEmitters.subEmittersCount; j++)
                {
                    var subPs = subEmitters.GetSubEmitterSystem(j);
                    if (!subPs || subPs.main.simulationSpace != ParticleSystemSimulationSpace.Local) continue;

                    var m = particle.transform.worldToLocalMatrix * currentTr.localToWorldMatrix;
                    m = currentTr.localToWorldMatrix;
                    var pos = currentTr.InverseTransformPoint(subPs.transform.position)
                        .GetScaled(Vector3.one - scale3D.Inverse());
                    subTransMats.Add(subPs, Matrix4x4.Translate(m * pos));
                }
            }

            var worldToUip = particle.transform.worldToLocalMatrix;
            var camera = particle.canvas.renderMode != RenderMode.ScreenSpaceOverlay ? particle.canvas.worldCamera : null;
            for (var i = 0; i < particle.particles.Count; i++)
            {
                Profiler.BeginSample("[UIParticle] Bake Mesh > Push index");
                MeshHelper.activeMeshIndices.Add(false);
                MeshHelper.activeMeshIndices.Add(false);
                Profiler.EndSample();

                // No particle to render (active).
                var currentPs = particle.particles[i];
                if (!currentPs || !currentPs.IsAlive()) continue;

                // No particle to render (scale).
                switch (currentPs.main.scalingMode)
                {
                    case ParticleSystemScalingMode.Hierarchy:
                    {
                        var scalePs = currentPs.transform.lossyScale;
                        if (Mathf.Approximately(scalePs.x * scalePs.y * scalePs.z, 0)) continue;
                    }
                        break;
                    case ParticleSystemScalingMode.Local:
                    {
                        var scalePs = currentPs.transform.localScale;
                        if (Mathf.Approximately(scalePs.x * scalePs.y * scalePs.z, 0)) continue;
                    }
                        break;
                    case ParticleSystemScalingMode.Shape:
                        break;
                }

                // Simulate particles
                Profiler.BeginSample("[UIParticle] Bake Mesh > Simulate particles");
                Simulate(currentPs, scale3D);
                Profiler.EndSample();

                // No particle to render (particle count).
                if (currentPs.particleCount == 0)
                {
                    if (currentPs.main.duration <= currentPs.time)
                    {
                        currentPs.Stop(false);
                    }

                    continue;
                }

                // No particle to render (material).
                var r = currentPs.GetComponent<ParticleSystemRenderer>();
                if (!r || (!r.sharedMaterial && !r.trailMaterial)) continue;

                // Calc matrix.
                Profiler.BeginSample("[UIParticle] Bake Mesh > Calc matrix");

                var matrix = worldToUip * GetScaledMatrix(currentPs, scale3D);
                Matrix4x4 extraMat;
                if (subTransMats.TryGetValue(currentPs, out extraMat))
                {
                    matrix *= extraMat;
                }

                Profiler.EndSample();

                // Bake main particles.
                if (CanBakeMesh(r))
                {
                    Profiler.BeginSample("[UIParticle] Bake Mesh > Bake Main Particles");
                    var hash = currentPs.GetMaterialHash(false);
                    if (hash != 0)
                    {
                        var m = MeshHelper.GetTemporaryMesh();
                        if (camera)
                            r.BakeMesh(m, camera, true);
                        else
                            r.BakeMesh(m, true);

                        MeshHelper.Push(i * 2, hash, m, matrix);
                    }

                    Profiler.EndSample();
                }

                // Bake trails particles.
                if (currentPs.trails.enabled)
                {
                    Profiler.BeginSample("[UIParticle] Bake Mesh > Bake Trails Particles");
                    var hash = currentPs.GetMaterialHash(true);
                    if (hash != 0)
                    {
                        if (currentPs.trails.worldSpace)
                        {
                            matrix = worldToUip * Matrix4x4.Scale(scale3D);
                        }

                        var m = MeshHelper.GetTemporaryMesh();
                        try
                        {
                            if (camera)
                                r.BakeTrailsMesh(m, camera, true);
                            else
                                r.BakeTrailsMesh(m, true);

                            MeshHelper.Push(i * 2 + 1, hash, m, matrix);
                        }
                        catch
                        {
                            MeshHelper.DiscardTemporaryMesh(m);
                        }
                    }

                    Profiler.EndSample();
                }
            }

            // Set active indices.
            Profiler.BeginSample("[UIParticle] Bake Mesh > Set active indices");
            particle.activeMeshIndices = MeshHelper.activeMeshIndices;
            Profiler.EndSample();

            // Combine
            Profiler.BeginSample("[UIParticle] Bake Mesh > CombineMesh");
            MeshHelper.CombineMesh(particle.bakedMesh);
            MeshHelper.Clear();
            Profiler.EndSample();
        }

        private static ParticleSystemSimulationSpace GetSimulationSpace(ParticleSystem ps)
        {
            var main = ps.main;
            var space = main.simulationSpace;
            if (space == ParticleSystemSimulationSpace.Custom && !main.customSimulationSpace)
                space = ParticleSystemSimulationSpace.Local;

            return space;
        }

        private static Matrix4x4 GetScaledMatrix(ParticleSystem ps, Vector3 scale)
        {
            switch (GetSimulationSpace(ps))
            {
                case ParticleSystemSimulationSpace.Local:
                    return Matrix4x4.Translate(ps.transform.position)
                           * Matrix4x4.Scale(scale);
                case ParticleSystemSimulationSpace.Custom:
                    return Matrix4x4.Translate(ps.main.customSimulationSpace.position)
                           * Matrix4x4.Scale(scale);
                case ParticleSystemSimulationSpace.World:
                    return Matrix4x4.Scale(scale);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Simulate(ParticleSystem currentPs, Vector3 scale)
        {
            if (!currentPs || !currentPs.IsAlive()) return;

            var main = currentPs.main;
            var deltaTime = main.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            var isScaling = scale != Vector3.one;
            var space = GetSimulationSpace(currentPs);

            // non-scale or local
            if (!isScaling || space == ParticleSystemSimulationSpace.Local)
            {
                currentPs.Simulate(deltaTime, false, false, false);
                return;
            }

            // get world position.
            var rateOverDistance = true;
            var psTransform = currentPs.transform;
            var originWorldPosition = psTransform.position;
            var originWorldRotation = psTransform.rotation;
            var wp = originWorldPosition;
            if (space == ParticleSystemSimulationSpace.Custom)
            {
                var emission = currentPs.emission;
                rateOverDistance = emission.enabled && 0 < emission.rateOverDistance.constant && 0 < emission.rateOverDistanceMultiplier;
                wp += Vector3.Scale(main.customSimulationSpace.position, scale - Vector3.one);
            }

            // inverse scaling
            wp.Scale(scale.Inverse());

            // rateOverDistance issue
            Vector3 oldWp;
            if (rateOverDistance)
            {
                if (WorldPositionCache.TryGetCachedWorldPosition(psTransform, out oldWp))
                    psTransform.SetPositionAndRotation(oldWp, originWorldRotation);
                else
                    psTransform.SetPositionAndRotation(wp, originWorldRotation);

                currentPs.Simulate(0, false, false, false);
            }

            // cache position
            psTransform.SetPositionAndRotation(wp, originWorldRotation);
            WorldPositionCache.CacheWorldPosition(psTransform, wp);
            currentPs.Simulate(deltaTime, false, false, false);
            psTransform.SetPositionAndRotation(originWorldPosition, originWorldRotation);
        }

        private static bool CanBakeMesh(ParticleSystemRenderer renderer)
        {
            // #69: Editor crashes when mesh is set to null when `ParticleSystem.RenderMode = Mesh`
            if (renderer.renderMode == ParticleSystemRenderMode.Mesh && renderer.mesh == null) return false;

            // #61: When `ParticleSystem.RenderMode = None`, an error occurs
            if (renderer.renderMode == ParticleSystemRenderMode.None) return false;

            return true;
        }
    }
}
