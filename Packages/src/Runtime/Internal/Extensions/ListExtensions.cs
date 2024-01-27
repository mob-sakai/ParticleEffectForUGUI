using System.Collections.Generic;

namespace Coffee.UIParticleInternal
{
    /// <summary>
    /// Extension methods for Component class.
    /// </summary>
    internal static class ListExtensions
    {
        public static void RemoveAtFast<T>(this List<T> self, int index)
        {
            if (self == null) return;

            var lastIndex = self.Count - 1;
            self[index] = self[lastIndex];
            self.RemoveAt(lastIndex);
        }
    }
}
