using System;
using UnityEngine;

namespace Coffee.UIExtensions
{
    [Serializable]
    public class AnimatableProperty : ISerializationCallbackReceiver
    {
        public enum ShaderPropertyType
        {
            Color,
            Vector,
            Float,
            Range,
            Texture
        }

        [SerializeField] private string m_Name = "";
        [SerializeField] private ShaderPropertyType m_Type = ShaderPropertyType.Vector;
        public int id { get; private set; }

        public ShaderPropertyType type
        {
            get { return m_Type; }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            id = Shader.PropertyToID(m_Name);
        }

        public void UpdateMaterialProperties(Material material, MaterialPropertyBlock mpb)
        {
            if (!material.HasProperty(id)) return;

            switch (type)
            {
                case ShaderPropertyType.Color:
                    var color = mpb.GetColor(id);
                    if (color != default)
                    {
                        material.SetColor(id, color);
                    }

                    break;
                case ShaderPropertyType.Vector:
                    var vector = mpb.GetVector(id);
                    if (vector != default)
                    {
                        material.SetVector(id, vector);
                    }

                    break;
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    var value = mpb.GetFloat(id);
                    if (!Mathf.Approximately(value, 0))
                    {
                        material.SetFloat(id, value);
                    }

                    break;
                case ShaderPropertyType.Texture:
                    var tex = mpb.GetTexture(id);
                    if (tex != default(Texture))
                    {
                        material.SetTexture(id, tex);
                    }

                    break;
            }
        }
    }
}
