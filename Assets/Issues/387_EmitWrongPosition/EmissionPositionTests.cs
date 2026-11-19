using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coffee.UIParticle.Issue387.Tests
{
    public class EmissionPositionTests
    {
        private sealed class Fixture
        {
            public readonly GameObject canvas;
            public readonly Coffee.UIExtensions.UIParticle ui;
            public readonly ParticleSystem ps;

            public Fixture(Vector3 rootPosition)
            {
                canvas = new GameObject("Canvas", typeof(Canvas));
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var root = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
                root.transform.SetParent(canvas.transform, false);
                root.transform.localPosition = rootPosition;
                ui = root.AddComponent<Coffee.UIExtensions.UIParticle>();
                ui.scale = 10;
                ps = AddSystem(root.transform, "ParticleSystem", Vector3.zero);
            }

            public static ParticleSystem AddSystem(Transform parent, string name, Vector3 offset)
            {
                var child = new GameObject(name);
                child.SetActive(false);
                child.transform.SetParent(parent, false);
                child.transform.localPosition = offset;
                var ps = child.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.playOnAwake = false;
                main.startSpeed = 0;
                main.startLifetime = 100;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                var emission = ps.emission;
                emission.enabled = false;
                var shape = ps.shape;
                shape.enabled = false;
                child.SetActive(true);
                return ps;
            }

            public void Dispose()
            {
                Object.Destroy(canvas);
            }
        }

        [Explicit("Custom simulation space is unsupported; this test is for reference only.")]
        [UnityTest]
        public IEnumerator UIParticleEmitCustomSpaceAtExternalTransform()
        {
            var f = new Fixture(new Vector3(80, 40, 0));
            try
            {
                var space = new GameObject("CustomSpace");
                space.transform.SetParent(f.ui.transform, false);
                space.transform.localPosition = new Vector3(12, 4, 0);
                var main = f.ps.main;
                main.simulationSpace = ParticleSystemSimulationSpace.Custom;
                main.customSimulationSpace = space.transform;
                f.ui.RefreshParticles();
                yield return null;
                f.ui.Emit(1);
                yield return null;
                yield return new WaitForEndOfFrame();
                AssertParticleCenter(f.ui, f.ps.transform.position);
            }
            finally { f.Dispose(); }
        }

        [Explicit("Custom simulation space is unsupported.")]
        [UnityTest]
        public IEnumerator UIParticlePlayBurstCustomSpaceAtEmitterChild()
        {
            var f = new Fixture(new Vector3(80, 40, 0));
            try
            {
                var space = new GameObject("CustomSpace");
                space.transform.SetParent(f.ps.transform, false);
                space.transform.localPosition = new Vector3(12, 4, 0);
                var main = f.ps.main;
                main.simulationSpace = ParticleSystemSimulationSpace.Custom;
                main.customSimulationSpace = space.transform;
                var emission = f.ps.emission;
                emission.enabled = true;
                emission.rateOverTime = 0;
                emission.SetBursts(new[] { new ParticleSystem.Burst(0, (short)1) });
                f.ui.RefreshParticles();
                yield return null;
                f.ui.Play();
                yield return null;
                yield return new WaitForEndOfFrame();
                AssertParticleCenter(f.ui, f.ps.transform.position);
            }
            finally { f.Dispose(); }
        }

        [Explicit("Custom simulation space is unsupported.")]
        [UnityTest]
        public IEnumerator UIParticleEmitCustomSpaceAtEmitterChild()
        {
            var f = new Fixture(new Vector3(80, 40, 0));
            try
            {
                var space = new GameObject("CustomSpace");
                space.transform.SetParent(f.ps.transform, false);
                space.transform.localPosition = new Vector3(12, 4, 0);
                var main = f.ps.main;
                main.simulationSpace = ParticleSystemSimulationSpace.Custom;
                main.customSimulationSpace = space.transform;
                f.ui.RefreshParticles();
                yield return null;
                f.ui.Emit(1);
                yield return null;
                yield return new WaitForEndOfFrame();
                AssertParticleCenter(f.ui, f.ps.transform.position);
            }
            finally { f.Dispose(); }
        }

        [UnityTest]
        public IEnumerator UIParticleEmitIncludesRegisteredBirthSubEmitter()
        {
            var f = new Fixture(new Vector3(80, 40, 0));
            try
            {
                var sub = Fixture.AddSystem(f.ps.transform, "SubEmitter", Vector3.zero);
                var emission = sub.emission;
                emission.enabled = true;
                emission.rateOverTime = 0;
                emission.SetBursts(new[] { new ParticleSystem.Burst(0, (short)1) });
                var subs = f.ps.subEmitters;
                subs.enabled = true;
                subs.AddSubEmitter(sub, ParticleSystemSubEmitterType.Birth, ParticleSystemSubEmitterProperties.InheritNothing);
                f.ui.RefreshParticles();
                yield return null;
                f.ui.Emit(1);
                Assert.AreEqual(1, f.ps.particleCount);
                Assert.AreEqual(1, sub.particleCount);
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.AreEqual(2, sub.particleCount);
                var renderers = f.ui.GetComponentsInChildren<CanvasRenderer>()
                    .Where(r => r.gameObject != f.ui.gameObject && r.GetMesh() != null && r.GetMesh().vertexCount > 0)
                    .ToArray();
                Assert.AreEqual(2, renderers.Length);
                foreach (var renderer in renderers)
                {
                    var actual = renderer.transform.TransformPoint(renderer.GetMesh().bounds.center);
                    Assert.Less(Vector3.Distance(f.ps.transform.position, actual), 0.1f);
                }
            }
            finally { f.Dispose(); }
        }

        [Explicit("World-space trails do not align with particles in the prototype.")]
        [UnityTest]
        public IEnumerator UIParticleEmitWorldSpaceTrailFollowsMovingParticle()
        {
            var f = new Fixture(new Vector3(80, 40, 0));
            try
            {
                var main = f.ps.main;
                main.startSpeed = 1;
                var trails = f.ps.trails;
                trails.enabled = true;
                trails.worldSpace = true;
                trails.minVertexDistance = 0;
                trails.lifetime = 100;
                var particleRenderer = f.ps.GetComponent<ParticleSystemRenderer>();
                particleRenderer.sharedMaterials = new[] { particleRenderer.sharedMaterial, particleRenderer.sharedMaterial };
                f.ps.transform.rotation = Quaternion.Euler(0, 90, 0);
                f.ui.RefreshParticles();
                yield return null;
                f.ui.Emit(1);
                for (var i = 0; i < 10; i++) yield return null;
                yield return new WaitForEndOfFrame();
                var meshes = f.ui.GetComponentsInChildren<CanvasRenderer>()
                    .Where(r => r.gameObject != f.ui.gameObject && r.GetMesh() != null && r.GetMesh().vertexCount > 0)
                    .ToArray();
                Assert.AreEqual(2, meshes.Length);
                var particleCenter = meshes[0].transform.TransformPoint(meshes[0].GetMesh().bounds.center);
                var trailCenter = meshes[1].transform.TransformPoint(meshes[1].GetMesh().bounds.center);
                Assert.Less(Vector3.Distance(particleCenter, trailCenter), 20);
            }
            finally { f.Dispose(); }
        }

        [Explicit("World-space trails do not align with particles emitted by Play bursts.")]
        [UnityTest]
        public IEnumerator UIParticlePlayWorldSpaceTrailFollowsMovingParticle()
        {
            var f = new Fixture(new Vector3(80, 40, 0));
            try
            {
                var main = f.ps.main;
                main.startSpeed = 1;
                var emission = f.ps.emission;
                emission.enabled = true;
                emission.rateOverTime = 0;
                emission.SetBursts(new[] { new ParticleSystem.Burst(0, (short)1) });
                var trails = f.ps.trails;
                trails.enabled = true;
                trails.worldSpace = true;
                trails.minVertexDistance = 0;
                trails.lifetime = 100;
                var particleRenderer = f.ps.GetComponent<ParticleSystemRenderer>();
                particleRenderer.sharedMaterials = new[] { particleRenderer.sharedMaterial, particleRenderer.sharedMaterial };
                f.ps.transform.rotation = Quaternion.Euler(0, 90, 0);
                f.ui.RefreshParticles();
                yield return null;
                f.ui.Play();
                for (var i = 0; i < 10; i++) yield return null;
                yield return new WaitForEndOfFrame();
                var meshes = f.ui.GetComponentsInChildren<CanvasRenderer>()
                    .Where(r => r.gameObject != f.ui.gameObject && r.GetMesh() != null && r.GetMesh().vertexCount > 0)
                    .ToArray();
                Assert.AreEqual(2, meshes.Length);
                var particleCenter = meshes[0].transform.TransformPoint(meshes[0].GetMesh().bounds.center);
                var trailCenter = meshes[1].transform.TransformPoint(meshes[1].GetMesh().bounds.center);
                Assert.Less(Vector3.Distance(particleCenter, trailCenter), 20);
            }
            finally { f.Dispose(); }
        }

        [UnityTest]
        public IEnumerator UIParticleEmitLocalSpaceTrailFollowsMovingParticle()
        {
            var f = new Fixture(new Vector3(80, 40, 0));
            try
            {
                var main = f.ps.main;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                main.startSpeed = 1;
                var trails = f.ps.trails;
                trails.enabled = true;
                trails.worldSpace = false;
                trails.minVertexDistance = 0;
                trails.lifetime = 100;
                var particleRenderer = f.ps.GetComponent<ParticleSystemRenderer>();
                particleRenderer.sharedMaterials = new[] { particleRenderer.sharedMaterial, particleRenderer.sharedMaterial };
                f.ps.transform.rotation = Quaternion.Euler(0, 90, 0);
                f.ui.RefreshParticles();
                yield return null;
                f.ui.Emit(1);
                for (var i = 0; i < 10; i++) yield return null;
                yield return new WaitForEndOfFrame();
                var meshes = f.ui.GetComponentsInChildren<CanvasRenderer>()
                    .Where(r => r.gameObject != f.ui.gameObject && r.GetMesh() != null && r.GetMesh().vertexCount > 0)
                    .ToArray();
                Assert.AreEqual(2, meshes.Length);
                var particleCenter = meshes[0].transform.TransformPoint(meshes[0].GetMesh().bounds.center);
                var trailCenter = meshes[1].transform.TransformPoint(meshes[1].GetMesh().bounds.center);
                Assert.Less(Vector3.Distance(particleCenter, trailCenter), 20);
            }
            finally { f.Dispose(); }
        }

        [UnityTest]
        public IEnumerator UIParticleEmitPrimarySharesMeshWithReplica()
        {
            var primary = new Fixture(new Vector3(80, 40, 0));
            var replica = new Fixture(new Vector3(-80, -40, 0));
            try
            {
                primary.ui.meshSharing = Coffee.UIExtensions.UIParticle.MeshSharing.Primary;
                replica.ui.meshSharing = Coffee.UIExtensions.UIParticle.MeshSharing.Replica;
                primary.ui.groupId = replica.ui.groupId = 387;
                primary.ui.groupMaxId = replica.ui.groupMaxId = 387;
                Assert.AreEqual(primary.ui.groupId, replica.ui.groupId);
                primary.ui.RefreshParticles();
                replica.ui.RefreshParticles();
                yield return null;
                primary.ui.Emit(1);
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.AreEqual(1, primary.ps.particleCount);
                Assert.AreEqual(0, replica.ps.particleCount);
                AssertParticleCenter(primary.ui, primary.ps.transform.position);
                AssertParticleCenter(replica.ui, replica.ps.transform.position);
                replica.ui.Emit(1);
                Assert.AreEqual(1, replica.ps.particleCount);
                Assert.AreEqual(1, primary.ps.particleCount);
                yield return null;
                yield return new WaitForEndOfFrame();
                AssertParticleCenter(replica.ui, replica.ps.transform.position);
            }
            finally { primary.Dispose(); replica.Dispose(); }
        }

        [UnityTest]
        public IEnumerator UIParticleEmitAfterMovingWorldEmitterPreservesOldParticles()
        {
            var f = new Fixture(new Vector3(80, 40, 0));
            try
            {
                f.ui.RefreshParticles();
                yield return null;
                var first = f.ps.transform.position;
                f.ui.Emit(1);
                yield return null;
                f.ui.transform.localPosition += new Vector3(30, 20, 0);
                var second = f.ps.transform.position;
                f.ui.Emit(1);
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.AreEqual(2, f.ps.particleCount);
                AssertParticleCenter(f.ui, (first + second) / 2);
            }
            finally { f.Dispose(); }
        }

        [UnityTest]
        public IEnumerator UIParticleEmitInactiveSystemDoesNotQueueParticles()
        {
            var f = new Fixture(new Vector3(80, 40, 0));
            try
            {
                f.ps.gameObject.SetActive(false);
                f.ui.RefreshParticles();
                f.ui.Emit(1);
                Assert.AreEqual(0, f.ps.particleCount);
                f.ps.gameObject.SetActive(true);
                yield return null;
                Assert.AreEqual(0, f.ps.particleCount);
                f.ui.Emit(1);
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.AreEqual(1, f.ps.particleCount);
                AssertParticleCenter(f.ui, f.ps.transform.position);
            }
            finally { f.Dispose(); }
        }

        [UnityTest]
        public IEnumerator UIParticleEmitUsesScaledCanvasForEachAutoScalingMode()
        {
            foreach (var mode in new[]
                     {
                         Coffee.UIExtensions.UIParticle.AutoScalingMode.None,
                         Coffee.UIExtensions.UIParticle.AutoScalingMode.Transform,
                         Coffee.UIExtensions.UIParticle.AutoScalingMode.UIParticle
                     })
            {
                var f = new Fixture(new Vector3(80, 40, 0));
                try
                {
                    f.canvas.transform.localScale = new Vector3(2, 2, 2);
                    f.ui.autoScalingMode = mode;
                    f.ui.RefreshParticles();
                    yield return null;
                    f.ui.Emit(1);
                    yield return null;
                    yield return new WaitForEndOfFrame();
                    AssertParticleCenter(f.ui, f.ps.transform.position);
                }
                finally { f.Dispose(); }
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator UIParticleEmitHandlesMultipleSystemsBeforeFirstFrame()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var root = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
                root.transform.SetParent(canvas.transform, false);
                var ui = root.AddComponent<Coffee.UIExtensions.UIParticle>();
                ui.scale = 10;

                var systems = new ParticleSystem[2];
                for (var i = 0; i < systems.Length; i++)
                {
                    var child = new GameObject("ParticleSystem " + i);
                    child.SetActive(false);
                    child.transform.SetParent(root.transform, false);
                    var ps = child.AddComponent<ParticleSystem>();
                    var main = ps.main;
                    main.playOnAwake = false;
                    main.startSpeed = 0;
                    main.startLifetime = 100;
                    main.simulationSpace = i == 0 ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
                    var shape = ps.shape;
                    shape.enabled = false;
                    var emission = ps.emission;
                    emission.enabled = false;
                    child.SetActive(true);
                    systems[i] = ps;
                }

                ui.RefreshParticles();
                ui.Emit(0);
                Assert.AreEqual(0, systems[0].particleCount);
                Assert.AreEqual(0, systems[1].particleCount);
                ui.Emit(2);
                Assert.AreEqual(2, systems[0].particleCount);
                Assert.AreEqual(2, systems[1].particleCount);
                Assert.Throws<System.ArgumentOutOfRangeException>(() => ui.Emit(-1));
                yield return null;
                yield return new WaitForEndOfFrame();
                var renderers = ui.GetComponentsInChildren<CanvasRenderer>()
                    .Where(r => r.gameObject != ui.gameObject && r.GetMesh() != null && r.GetMesh().vertexCount > 0)
                    .ToArray();
                Assert.AreEqual(2, renderers.Length);
                foreach (var renderer in renderers)
                {
                    var actual = renderer.transform.TransformPoint(renderer.GetMesh().bounds.center);
                    Assert.Less(Vector3.Distance(root.transform.position, actual), 0.1f);
                }
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        [UnityTest]
        public IEnumerator UIParticleEmitSamplesShapeInWorldSpace()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var root = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
                root.transform.SetParent(canvas.transform, false);
                var ui = root.AddComponent<Coffee.UIExtensions.UIParticle>();
                ui.scale = 10;

                var child = new GameObject("ParticleSystem");
                child.SetActive(false);
                child.transform.SetParent(root.transform, false);
                var ps = child.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.playOnAwake = false;
                main.startSpeed = 0;
                main.startLifetime = 100;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                var emission = ps.emission;
                emission.enabled = false;
                var shape = ps.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Sphere;
                shape.radius = 1;
                child.SetActive(true);
                ui.RefreshParticles();

                var position = child.transform.position;
                ui.Emit(20);
                var particles = new ParticleSystem.Particle[20];
                Assert.AreEqual(20, ps.GetParticles(particles));
                Assert.IsTrue(particles.Any(p => Vector3.Distance(p.position, particles[0].position) > 0.01f));
                Assert.AreEqual(position, child.transform.position);
                yield return null;
                yield return new WaitForEndOfFrame();
                var renderer = ui.GetComponentsInChildren<CanvasRenderer>()
                    .First(r => r.gameObject != ui.gameObject && r.GetMesh() != null && r.GetMesh().vertexCount > 0);
                var actual = renderer.transform.TransformPoint(renderer.GetMesh().bounds.center);
                Assert.Less(Vector3.Distance(position, actual), 10.1f);
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        [UnityTest]
        public IEnumerator UIParticleEmitPreservesParticlesAndUsesSimulationCoordinates()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var root = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
                root.transform.SetParent(canvas.transform, false);
                var ui = root.AddComponent<Coffee.UIExtensions.UIParticle>();
                ui.scale3D = new Vector3(10, 5, 2);

                var child = new GameObject("ParticleSystem");
                child.SetActive(false);
                child.transform.SetParent(root.transform, false);
                child.transform.localPosition = new Vector3(20, 10, 0);
                var ps = child.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.playOnAwake = false;
                main.startSpeed = 0;
                main.startLifetime = 100;
                var shape = ps.shape;
                shape.enabled = false;
                var emission = ps.emission;
                emission.enabled = false;
                child.SetActive(true);
                ui.RefreshParticles();
                yield return null;

                foreach (var space in new[] { ParticleSystemSimulationSpace.World, ParticleSystemSimulationSpace.Local })
                {
                    ps.Clear();
                    main.simulationSpace = space;
                    var localPosition = child.transform.localPosition;
                    var localRotation = child.transform.localRotation;
                    var expected = root.transform.position + Vector3.Scale(localPosition, ui.scale3D);
                    ui.Emit(2);
                    Assert.AreEqual(localPosition, child.transform.localPosition);
                    Assert.AreEqual(localRotation, child.transform.localRotation);
                    Assert.AreEqual(2, ps.particleCount);
                    var before = new ParticleSystem.Particle[2];
                    Assert.AreEqual(2, ps.GetParticles(before));
                    ui.Emit(1);
                    Assert.AreEqual(3, ps.particleCount);
                    var after = new ParticleSystem.Particle[3];
                    Assert.AreEqual(3, ps.GetParticles(after));
                    Assert.IsTrue(after.Any(p => p.randomSeed == before[0].randomSeed));
                    Assert.IsTrue(after.Any(p => p.randomSeed == before[1].randomSeed));
                    yield return null;
                    yield return new WaitForEndOfFrame();
                    AssertParticleCenter(ui, expected);
                }
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        [UnityTest]
        public IEnumerator UIParticlePlayStartsWorldSpaceBurstAtEmitter()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var root = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
                root.transform.SetParent(canvas.transform, false);
                var ui = root.AddComponent<Coffee.UIExtensions.UIParticle>();
                ui.scale = 10;

                var child = new GameObject("ParticleSystem");
                child.SetActive(false);
                child.transform.SetParent(root.transform, false);
                var ps = child.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.playOnAwake = false;
                main.loop = false;
                main.startSpeed = 0;
                main.startLifetime = 100;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                var shape = ps.shape;
                shape.enabled = false;
                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new[] { new ParticleSystem.Burst(0, (short)1) });
                child.SetActive(true);
                ui.RefreshParticles();
                yield return null;

                ui.Play();
                yield return null;
                yield return new WaitForEndOfFrame();
                Assert.AreEqual(1, ps.particleCount);
                AssertParticleCenter(ui, child.transform.position);
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        [UnityTest]
        public IEnumerator CorrectedWorldEmitAndLocalEmitAppearAtEmitter()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var root = new GameObject("UIParticle", typeof(RectTransform), typeof(CanvasRenderer));
                root.transform.SetParent(canvas.transform, false);
                var ui = root.AddComponent<Coffee.UIExtensions.UIParticle>();
                ui.scale = 10;

                var child = new GameObject("ParticleSystem");
                child.SetActive(false);
                child.transform.SetParent(root.transform, false);
                var ps = child.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.playOnAwake = false;
                main.startSpeed = 0;
                main.startLifetime = 100;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                var shape = ps.shape;
                shape.enabled = false;
                var emission = ps.emission;
                emission.enabled = false;
                child.SetActive(true);
                ui.RefreshParticles();
                yield return null;

                var position = child.transform.position;
                var parameters = new ParticleSystem.EmitParams { position = position / ui.scale };
                ps.Emit(parameters, 1);
                yield return null;
                yield return new WaitForEndOfFrame();
                AssertParticleCenter(ui, position);

                ps.Clear();
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                ps.Emit(1);
                yield return null;
                yield return new WaitForEndOfFrame();
                AssertParticleCenter(ui, child.transform.position);
            }
            finally
            {
                Object.Destroy(canvas);
            }
        }

        private static void AssertParticleCenter(Coffee.UIExtensions.UIParticle ui, Vector3 expected)
        {
            var renderer = ui.GetComponentsInChildren<CanvasRenderer>()
                .First(r => r.gameObject != ui.gameObject && r.GetMesh() != null && r.GetMesh().vertexCount > 0);
            var actual = renderer.transform.TransformPoint(renderer.GetMesh().bounds.center);
            Assert.Less(Vector3.Distance(expected, actual), 0.1f, "The rendered particle position must match the emitter.");
        }
    }
}
