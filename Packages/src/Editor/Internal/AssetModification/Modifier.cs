using System;

namespace Coffee.UIParticleInternal.AssetModification
{
    internal abstract class Modifier
    {
        private string _error;
        public string path;

        protected abstract string id { get; }
        protected bool hasUpgraded { private set; get; }

        public void Modify(bool dryRun)
        {
            try
            {
                hasUpgraded = RunModify(dryRun);
            }
            catch (Exception e)
            {
                _error = e.Message;
            }
        }

        public string GetModificationReport()
        {
            return !string.IsNullOrEmpty(_error)
                ? $"<b><color=red>[{id} (Error)]</color> {path}</b> {_error}\n"
                : hasUpgraded
                    ? $"<b><color=green>[{id}]</color> {path}</b> {ModificationReport()}\n"
                    : string.Empty;
        }

        protected abstract bool RunModify(bool dryRun);

        protected virtual string ModificationReport()
        {
            return string.Empty;
        }
    }
}
