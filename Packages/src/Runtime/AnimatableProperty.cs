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

        public ShaderPropertyType type => m_Type;

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
                    material.SetColor(id, mpb.GetColor(id));
                    break;
                case ShaderPropertyType.Vector:
                    material.SetVector(id, mpb.GetVector(id));
                    break;
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    material.SetFloat(id, mpb.GetFloat(id));
                    break;
                case ShaderPropertyType.Texture:
                    material.SetTexture(id, mpb.GetTexture(id));
                    break;
            }
        }
    }
}
