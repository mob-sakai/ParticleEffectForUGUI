using System.Text;
using System.Text.RegularExpressions;

namespace Coffee.UIParticleInternal.AssetModification
{
    internal class TextReplaceModifier : ITextModifier
    {
        private readonly Regex _pattern;
        private readonly string _replace;

        public TextReplaceModifier(string pattern, string replace)
        {
            _pattern = new Regex(pattern);
            _replace = replace;
        }

        public bool ModifyText(StringBuilder sb, string text)
        {
            if (!_pattern.IsMatch(text)) return false;

            sb.AppendLine(_pattern.Replace(text, _replace));
            return true;
        }
    }
}
