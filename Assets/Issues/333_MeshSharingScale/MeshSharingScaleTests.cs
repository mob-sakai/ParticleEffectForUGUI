using System.Collections;
using System.Linq;
using ParticleUI = Coffee.UIExtensions.UIParticle;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coffee.UIParticle.Issue333.Tests
{
    public class MeshSharingScaleTests
    {
        private sealed class Fixture
        {
            public readonly GameObject canvas;
            public readonly ParticleUI primary;
            public readonly ParticleUI replica;
            public readonly ParticleUI reference;
            public readonly Transform primaryParent;
            public readonly Transform replicaParent;
            public readonly Transform referenceParent;

            public Fixture(ParticleUI.AutoScalingMode mode)
            {
                canvas = new GameObject("Canvas", typeof(Canvas));
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                primary = Create("Primary", mode, out primaryParent);
                replica = Create("Replica", mode, out replicaParent);
                reference = Create("Reference", mode, out referenceParent);
                primary.meshSharing = ParticleUI.MeshSharing.Primary;
                replica.meshSharing = ParticleUI.MeshSharing.Replica;
                primary.groupId = replica.groupId = 333;
                primary.groupMaxId = replica.groupMaxId = 333;
            }

            private ParticleUI Create(string name, ParticleUI.AutoScalingMode mode, out Transform parent)
            {
                var parentObject = new GameObject(name + "Parent", typeof(RectTransform));
                parent = parentObject.transform;
                parent.SetParent(canvas.transform, false);
                var root = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
                root.transform.SetParent(parent, false);
                var ui = root.AddComponent<ParticleUI>();
                ui.autoScalingMode = mode;
                ui.scale = 10;
                var child = new GameObject("ParticleSystem");
                child.SetActive(false);
                child.transform.SetParent(root.transform, false);
                var ps = child.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.playOnAwake = false;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                main.startSpeed = 0;
                main.startSize = 1;
                main.startLifetime = 100;
                var emission = ps.emission;
                emission.enabled = false;
                var shape = ps.shape;
                shape.enabled = false;
                child.SetActive(true);
                ui.RefreshParticles();
                return ui;
            }

            public void SetScales(Vector3 primaryScale, Vector3 replicaScale)
            {
                primaryParent.localScale = primaryScale;
                replicaParent.localScale = replicaScale;
                referenceParent.localScale = replicaScale;
            }

            public void Emit()
            {
                primary.Emit(1);
                reference.Emit(1);
            }

            public void Dispose()
            {
                Object.Destroy(canvas);
            }
        }

        private static Vector2 Size(ParticleUI ui)
        {
            var renderer = ui.GetComponentsInChildren<CanvasRenderer>()
                .First(r => r.gameObject != ui.gameObject && r.GetMesh() != null && r.GetMesh().vertexCount > 0);
            var bounds = renderer.GetMesh().bounds;
            var x = renderer.transform.TransformVector(new Vector3(bounds.size.x, 0, 0)).magnitude;
            var y = renderer.transform.TransformVector(new Vector3(0, bounds.size.y, 0)).magnitude;
            return new Vector2(x, y);
        }

        [Explicit("共有メッシュは異なる親スケールで独立エフェクトと一致しません。")]
        [UnityTest]
        public IEnumerator ReplicaMatchesIndependentEffectAcrossParentScales()
        {
            var f = new Fixture(ParticleUI.AutoScalingMode.Transform);
            try
            {
                f.SetScales(Vector3.one, new Vector3(2, 2, 2));
                yield return null;
                f.Emit();
                yield return null;
                yield return new WaitForEndOfFrame();
                var expected = Size(f.reference);
                var actual = Size(f.replica);
                Assert.That(actual.x, Is.EqualTo(expected.x).Within(0.1f), $"Replica {actual}, reference {expected}");
                Assert.That(actual.y, Is.EqualTo(expected.y).Within(0.1f));
            }
            finally { f.Dispose(); }
        }

        [UnityTest]
        public IEnumerator EqualParentScalesMatchIndependentEffect()
        {
            var f = new Fixture(ParticleUI.AutoScalingMode.Transform);
            try
            {
                f.SetScales(Vector3.one * 2, Vector3.one * 2);
                yield return null;
                f.Emit();
                yield return null;
                yield return new WaitForEndOfFrame();
                var expected = Size(f.reference);
                var actual = Size(f.replica);
                Assert.That(actual.x, Is.EqualTo(expected.x).Within(0.1f));
                Assert.That(actual.y, Is.EqualTo(expected.y).Within(0.1f));
            }
            finally { f.Dispose(); }
        }

        [Explicit("共有メッシュは異なる親スケールで独立エフェクトと一致しません。")]
        [UnityTest]
        public IEnumerator ReplicaMatchesIndependentEffectWhenPrimaryIsLarger()
        {
            var f = new Fixture(ParticleUI.AutoScalingMode.Transform);
            try
            {
                f.SetScales(Vector3.one * 2, Vector3.one);
                yield return null;
                f.Emit();
                yield return null;
                yield return new WaitForEndOfFrame();
                var expected = Size(f.reference);
                var actual = Size(f.replica);
                Assert.That(actual.x, Is.EqualTo(expected.x).Within(0.1f), $"Replica {actual}, reference {expected}");
                Assert.That(actual.y, Is.EqualTo(expected.y).Within(0.1f));
            }
            finally { f.Dispose(); }
        }

        [Explicit("共有メッシュは異なる親スケールで独立エフェクトと一致しません。")]
        [UnityTest]
        public IEnumerator ReplicaMatchesIndependentEffectAfterParentScaleChanges()
        {
            var f = new Fixture(ParticleUI.AutoScalingMode.Transform);
            try
            {
                f.SetScales(Vector3.one, Vector3.one);
                yield return null;
                f.Emit();
                yield return null;
                f.SetScales(Vector3.one, new Vector3(2, 3, 1));
                yield return null;
                yield return new WaitForEndOfFrame();
                var expected = Size(f.reference);
                var actual = Size(f.replica);
                Assert.That(actual.x, Is.EqualTo(expected.x).Within(0.1f), $"Replica {actual}, reference {expected}");
                Assert.That(actual.y, Is.EqualTo(expected.y).Within(0.1f));
            }
            finally { f.Dispose(); }
        }
    }
}
