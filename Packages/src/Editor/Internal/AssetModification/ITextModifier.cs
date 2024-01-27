using System.Text;

namespace Coffee.UIParticleInternal.AssetModification
{
    internal interface ITextModifier
    {
        bool ModifyText(StringBuilder sb, string text);
    }
}
