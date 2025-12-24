using System.Collections;
using Coffee.UIParticleInternal;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coffee.UIParticle.Editor.Tests
{
    public class NewTestScript
    {
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(2048)]
        [TestCase(3000)]
        public void GetParticleArray(int requiredSize)
        {
            var array = ParticleSystemExtensions.GetParticleArray(requiredSize);
            Debug.Log($"requiredSize: {requiredSize}, array.Length: {array.Length}");
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
