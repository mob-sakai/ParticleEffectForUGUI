using System.Collections;
using ParticleUI = Coffee.UIExtensions.UIParticle;
using UnityEngine;

namespace Coffee.UIParticle.Issue333.Scene
{
    public class MeshSharingScaleDemo : MonoBehaviour
    {
        [SerializeField] private ParticleUI primary;
        [SerializeField] private ParticleUI reference;

        private IEnumerator Start()
        {
            yield return null;
            primary.Emit(1);
            reference.Emit(1);
        }
    }
}
