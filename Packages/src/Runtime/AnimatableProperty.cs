using System;
using UnityEngine;

namespace Coffee.UIExtensions
{
    [Serializable]
    public class AnimatableProperty : ISerializationCallbackReceiver
    {
        public enum ShaderPropertyType
        {
            None = -1,
            Color,
            Vector,
            Float,
            Range,
            Texture,
            Int,

            Matrix = 100,
            MatrixArray = 101,
            FloatArray = 102,
            VectorArray = 103,
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
#if UNITY_2021_1_OR_NEWER
            if (!mpb.HasProperty(id)) return;
#else
            if (!material.HasProperty(id)) return;
#endif

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
                case ShaderPropertyType.Int:
                    material.SetInt(id, mpb.GetInt(id));
                    break;
                case ShaderPropertyType.Matrix:
                    material.SetMatrix(id, mpb.GetMatrix(id));
                    break;
                case ShaderPropertyType.MatrixArray:
                    material.SetMatrixArray(id, mpb.GetMatrixArray(id));
                    break;
                case ShaderPropertyType.FloatArray:
                    material.SetFloatArray(id, mpb.GetFloatArray(id));
                    break;
                case ShaderPropertyType.VectorArray:
                    material.SetVectorArray(id, mpb.GetVectorArray(id));
                    break;
            }
        }
    }
}
