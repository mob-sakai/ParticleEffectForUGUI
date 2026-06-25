using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ParticleSystemRenderer))]
public class ColorArrayInjection : MonoBehaviour
{
    [SerializeField] private string m_PropertyName = "_Color2";

    [SerializeField] private Color[] m_Colors = new Color[4]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    private void OnEnable()
    {
        if (TryGetComponent<ParticleSystemRenderer>(out var psr))
        {
            var mpb = new MaterialPropertyBlock();
            psr.GetPropertyBlock(mpb);
            mpb.SetVectorArray(m_PropertyName, m_Colors.Select(x => new Vector4(x.r, x.g, x.b, 1.0f)).ToArray());
            psr.SetPropertyBlock(mpb);
        }
    }

    private void OnValidate()
    {
        OnEnable();
    }
}
