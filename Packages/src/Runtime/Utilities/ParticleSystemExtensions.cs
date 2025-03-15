using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coffee.UIParticleInternal
{
    internal static class ParticleSystemExtensions
    {
        private static ParticleSystem.Particle[] s_TmpParticles = new ParticleSystem.Particle[2048];

        public static ParticleSystem.Particle[] GetParticleArray(int size)
        {
            if (s_TmpParticles.Length < size)
            {
                while (s_TmpParticles.Length < size)
                {
                    size = Mathf.NextPowerOfTwo(size);
                }

                s_TmpParticles = new ParticleSystem.Particle[size];
            }

            return s_TmpParticles;
        }

        public static void ValidateShape(this ParticleSystem self)
        {
            var shape = self.shape;
            if (shape.enabled && shape.alignToDirection)
            {
                if (Mathf.Approximately(shape.scale.x * shape.scale.y * shape.scale.z, 0))
                {
                    if (Mathf.Approximately(shape.scale.x, 0))
                    {
                        shape.scale.Set(0.0001f, shape.scale.y, shape.scale.z);
                    }
                    else if (Mathf.Approximately(shape.scale.y, 0))
                    {
                        shape.scale.Set(shape.scale.x, 0.0001f, shape.scale.z);
                    }
                    else if (Mathf.Approximately(shape.scale.z, 0))
                    {
                        shape.scale.Set(shape.scale.x, shape.scale.y, 0.0001f);
                    }
                }
            }
        }

        public static bool CanBakeMesh(this ParticleSystemRenderer self)
        {
            // #69: Editor crashes when mesh is set to null when `ParticleSystem.RenderMode = Mesh`
            if (self.renderMode == ParticleSystemRenderMode.Mesh && self.mesh == null) return false;

            // #61: When `ParticleSystem.RenderMode = None`, an error occurs
            if (self.renderMode == ParticleSystemRenderMode.None) return false;

            return true;
        }

        public static ParticleSystemSimulationSpace GetActualSimulationSpace(this ParticleSystem self)
        {
            var main = self.main;
            var space = main.simulationSpace;
            if (space == ParticleSystemSimulationSpace.Custom && !main.customSimulationSpace)
            {
                space = ParticleSystemSimulationSpace.Local;
            }

            return space;
        }

        public static bool IsLocalSpace(this ParticleSystem self)
        {
            return GetActualSimulationSpace(self) == ParticleSystemSimulationSpace.Local;
        }

        public static bool IsWorldSpace(this ParticleSystem self)
        {
            return GetActualSimulationSpace(self) == ParticleSystemSimulationSpace.World;
        }

        public static void SortForRendering(this List<ParticleSystem> self, Transform transform, bool sortByMaterial)
        {
            self.Sort((a, b) =>
            {
                var aRenderer = a.GetComponent<ParticleSystemRenderer>();
                var bRenderer = b.GetComponent<ParticleSystemRenderer>();

                // Render queue: ascending
                var aMat = aRenderer.sharedMaterial ? aRenderer.sharedMaterial : aRenderer.trailMaterial;
                var bMat = bRenderer.sharedMaterial ? bRenderer.sharedMaterial : bRenderer.trailMaterial;
                if (!aMat && !bMat) return 0;
                if (!aMat) return -1;
                if (!bMat) return 1;

                if (sortByMaterial)
                {
                    return aMat.GetInstanceID() - bMat.GetInstanceID();
                }

                if (aMat.renderQueue != bMat.renderQueue)
                {
                    return aMat.renderQueue - bMat.renderQueue;
                }

                // Sorting layer: ascending
                if (aRenderer.sortingLayerID != bRenderer.sortingLayerID)
                {
                    return SortingLayer.GetLayerValueFromID(aRenderer.sortingLayerID) -
                           SortingLayer.GetLayerValueFromID(bRenderer.sortingLayerID);
                }

                // Sorting order: ascending
                if (aRenderer.sortingOrder != bRenderer.sortingOrder)
                {
                    return aRenderer.sortingOrder - bRenderer.sortingOrder;
                }

                // Z position & sortingFudge: descending
                var aTransform = a.transform;
                var bTransform = b.transform;
                var aPos = transform.InverseTransformPoint(aTransform.position).z + aRenderer.sortingFudge;
                var bPos = transform.InverseTransformPoint(bTransform.position).z + bRenderer.sortingFudge;
                if (!Mathf.Approximately(aPos, bPos))
                {
                    return (int)Mathf.Sign(bPos - aPos);
                }

                return (int)Mathf.Sign(GetIndex(self, a) - GetIndex(self, b));
            });
        }

        private static int GetIndex(IList<ParticleSystem> list, Object ps)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].GetInstanceID() == ps.GetInstanceID())
                {
                    return i;
                }
            }

            return 0;
        }

        public static Texture2D GetTextureForSprite(this ParticleSystem self)
        {
            if (!self) return null;

            // Get sprite's texture.
            var tsaModule = self.textureSheetAnimation;
            if (!tsaModule.enabled || tsaModule.mode != ParticleSystemAnimationMode.Sprites) return null;

            for (var i = 0; i < tsaModule.spriteCount; i++)
            {
                var sprite = tsaModule.GetSprite(i);
                if (!sprite) continue;

                return sprite.GetActualTexture();
            }

            return null;
        }

        public static void Exec(this List<ParticleSystem> self, Action<ParticleSystem> action)
        {
            foreach (var p in self)
            {
                if (!p) continue;
                action.Invoke(p);
            }
        }

        public static ParticleSystem GetMainEmitter(this ParticleSystem self, List<ParticleSystem> list)
        {
            if (!self || list == null || list.Count == 0) return null;

            for (var i = 0; i < list.Count; i++)
            {
                var parent = list[i];
                if (parent != self && IsSubEmitterOf(self, parent)) return parent;
            }

            return null;
        }

        public static bool IsSubEmitterOf(this ParticleSystem self, ParticleSystem parent)
        {
            if (!self || !parent) return false;

            var subEmitters = parent.subEmitters;
            var count = subEmitters.subEmittersCount;
            for (var i = 0; i < count; i++)
            {
                if (subEmitters.GetSubEmitterSystem(i) == self) return true;
            }

            return false;
        }
    }
}
