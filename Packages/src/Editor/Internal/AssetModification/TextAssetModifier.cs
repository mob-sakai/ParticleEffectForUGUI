using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coffee.UIParticleInternal.AssetModification
{
    internal class TextAssetModifier : Modifier
    {
        public ITextModifier[] textModifiers;
        protected override string id => "Text";
        protected virtual string savePath => path;

        protected override bool RunModify(bool dryRun)
        {
            var changed = false;
            using (var scope = new EditScope(path, savePath))
            {
                foreach (var line in scope.lines)
                {
                    if (ModifyLine(scope.sb, line))
                    {
                        changed = true;
                    }
                    else
                    {
                        scope.sb.AppendLine(line);
                    }
                }

                if (!dryRun && changed)
                {
                    scope.Save();
                }

                return changed;
            }
        }

        private bool ModifyLine(StringBuilder sb, string line)
        {
            foreach (var modifier in textModifiers)
            {
                if (modifier.ModifyText(sb, line))
                {
                    return true;
                }
            }

            return false;
        }

        private readonly struct EditScope : IDisposable
        {
            private static readonly StringBuilder s_File = new StringBuilder();
            private readonly string _path;
            private readonly string _savePath;
            public IEnumerable<string> lines => File.ReadLines(_path);
            public StringBuilder sb => s_File;

            public EditScope(string path, string savePath)
            {
                s_File.Length = 0;
                _path = path;
                _savePath = savePath;
            }

            public void Dispose()
            {
            }

            public void Save()
            {
                File.WriteAllText(_savePath, sb.ToString());
            }
        }
    }
}
