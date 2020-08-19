using System;
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

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            id = Shader.PropertyToID(m_Name);
        }
    }
}
