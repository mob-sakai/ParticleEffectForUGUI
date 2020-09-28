using UnityEngine;

namespace Coffee.UIExtensions
{
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
        }

        [SerializeField] string m_Name = "";
        [SerializeField] ShaderPropertyType m_Type = ShaderPropertyType.Vector;
        public int id { get; private set; }

        public ShaderPropertyType type
        {
            get { return m_Type; }
        }

        public void UpdateMaterialProperties(Material material, MaterialPropertyBlock mpb)
        {
            if (!material.HasProperty(id)) return;

            switch (type)
            {
                case ShaderPropertyType.Color:
                    var color = mpb.GetColor(id);
                    if (color != default(Color))
                        material.SetColor(id, color);
                    break;
                case ShaderPropertyType.Vector:
                    var vector = mpb.GetVector(id);
                    if (vector != default(Vector4))
                        material.SetVector(id, vector);
                    break;
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    var value = mpb.GetFloat(id);
                    if (value != default(float))
                        material.SetFloat(id, value);
                    break;
                case ShaderPropertyType.Texture:
                    var tex = mpb.GetTexture(id);
                    if (tex != default(Texture))
                        material.SetTexture(id, tex);
                    break;
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            id = Shader.PropertyToID(m_Name);
        }
    }
}
