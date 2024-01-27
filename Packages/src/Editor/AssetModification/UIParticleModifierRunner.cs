using System;
using System.Collections.Generic;
using Coffee.UIParticleInternal.AssetModification;
using UnityEditor;

namespace Coffee.UIParticleInternal
{
    internal class UIParticleModifierRunner : Runner
    {
        public UIParticleModifierRunner()
            : base("UIParticle v5", new List<(string, Func<string, Modifier>)>
            {
                (".unity", x => new SceneModifier
                {
                    path = x,
                    componentModifiers = new IComponentModifier[]
                    {
                        new UIParticleRendererComponentModifier(),
                        new UIParticleComponentModifier_AutoScaling(),
                        new UIParticleComponentModifier_AbsoluteMode(),
                        new UIParticleComponentModifier_IsTrail()
                    }
                }),
                (".prefab", x => new PrefabModifier
                {
                    path = x,
                    componentModifiers = new IComponentModifier[]
                    {
                        new UIParticleRendererComponentModifier(),
                        new UIParticleComponentModifier_AutoScaling(),
                        new UIParticleComponentModifier_AbsoluteMode(),
                        new UIParticleComponentModifier_IsTrail()
                    }
                })
                // (".shader", SoftMaskableShaderModifier.Create)
            })
        {
        }

        [MenuItem("UIParticleModifierRunner/Run")]
        private static void Run()
        {
            new UIParticleModifierRunner().RunIfUserWantsTo();
        }
    }
}
