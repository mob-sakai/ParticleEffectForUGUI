#if !UNITY_2019_2_OR_NEWER
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Coffee.UIParticleInternal
{
    public static class TypeCache
    {
        private static readonly object s_Lock = new object();
        private static readonly Dictionary<Type, Type[]> s_DerivedTypesCache = new Dictionary<Type, Type[]>();
        private static readonly Dictionary<Type, Type[]> s_AttributeTypesCache = new Dictionary<Type, Type[]>();

        public static IEnumerable<Type> GetTypesDerivedFrom(Type baseType)
        {
            lock (s_Lock)
            {
                if (s_DerivedTypesCache.TryGetValue(baseType, out var cached))
                {
                    return cached;
                }

                var types = new List<Type>();
                foreach (var t in GetAllLoadableTypes())
                {
                    if (t != baseType && baseType.IsAssignableFrom(t))
                    {
                        types.Add(t);
                    }
                }

                return s_DerivedTypesCache[baseType] = types.ToArray();
            }
        }

        public static IEnumerable<Type> GetTypesWithAttribute(Type attr)
        {
            lock (s_Lock)
            {
                if (s_AttributeTypesCache.TryGetValue(attr, out var cached))
                {
                    return cached;
                }

                var types = new List<Type>();
                foreach (var t in GetAllLoadableTypes())
                {
                    if (t.GetCustomAttributes(attr, inherit: true).Length > 0)
                    {
                        types.Add(t);
                    }
                }

                return s_AttributeTypesCache[attr] = types.ToArray();
            }
        }

        private static IEnumerable<Type> GetAllLoadableTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                if (types == null)
                {
                    continue;
                }

                foreach (var t in types)
                {
                    if (t != null)
                    {
                        yield return t;
                    }
                }
            }
        }
    }
}
#endif
