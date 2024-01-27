using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Coffee.UIParticleInternal
{
    internal class ObjectRepository<T> where T : Object
    {
        private readonly List<Entry> _cache = new List<Entry>();
        private readonly string _name;
        private readonly Action<T> _onRelease;
        private readonly Stack<Entry> _pool = new Stack<Entry>();

        public ObjectRepository(Action<T> onRelease = null)
        {
            _name = $"{typeof(T).Name}Repository";
            if (onRelease == null)
            {
                _onRelease = x =>
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        Object.DestroyImmediate(x, false);
                    }
                    else
#endif
                    {
                        Object.Destroy(x);
                    }
                };
            }
            else
            {
                _onRelease = onRelease;
            }
        }

        public int count => _cache.Count;

        public void Clear()
        {
            for (var i = 0; i < _cache.Count; i++)
            {
                var entry = _cache[i];
                if (entry == null) continue;

                entry.Release(_onRelease);
            }

            _cache.Clear();
        }

        public bool Valid(Hash128 hash, T obj)
        {
            // Find existing entry.
            Profiler.BeginSample("(COF)[ObjectRepository] Valid > Find existing entry");
            for (var i = 0; i < _cache.Count; ++i)
            {
                var entry = _cache[i];
                if (entry.hash != hash) continue;
                Profiler.EndSample();

                // Existing entry found.
                return entry.storedObject == obj;
            }

            Profiler.EndSample();

            return false;
        }

        /// <summary>
        /// Adds or retrieves a cached object based on the hash.
        /// </summary>
        public void Get(Hash128 hash, ref T obj, Func<T> onCreate)
        {
            // Find existing entry.
            Profiler.BeginSample("(COF)[ObjectRepository] Get > Find existing entry");
            for (var i = 0; i < _cache.Count; ++i)
            {
                var entry = _cache[i];
                if (entry.hash != hash) continue;

                // Existing entry found.
                if (entry.storedObject != obj)
                {
                    // if the object is different, release the old one.
                    Release(ref obj);
                    ++entry.reference;
                    obj = entry.storedObject;
                    Logging.Log(_name, $"Get(#{count}): {entry}");
                }

                Profiler.EndSample();
                return;
            }

            Profiler.EndSample();

            // Create new entry.
            Profiler.BeginSample("(COF)[ObjectRepository] Get > Create new entry");
            var newEntry = 0 < _pool.Count ? _pool.Pop() : new Entry();
            newEntry.storedObject = onCreate();
            newEntry.hash = hash;
            newEntry.reference = 1;
            _cache.Add(newEntry);
            Logging.Log(_name, $"Get(#{count}): {newEntry}");

            Release(ref obj);
            obj = newEntry.storedObject;
            Profiler.EndSample();
        }

        /// <summary>
        /// Adds or retrieves a cached object based on the hash.
        /// </summary>
        public void Get<TS>(Hash128 hash, ref T obj, Func<TS, T> onCreate, TS source)
        {
            // Find existing entry.
            Profiler.BeginSample("(COF)[ObjectRepository] Get > Find existing entry");
            for (var i = 0; i < _cache.Count; ++i)
            {
                var entry = _cache[i];
                if (entry.hash != hash) continue;

                // Existing entry found.
                if (entry.storedObject != obj)
                {
                    // if the object is different, release the old one.
                    Release(ref obj);
                    ++entry.reference;
                    obj = entry.storedObject;
                    Logging.Log(_name, $"Get(#{count}): {entry}");
                }

                Profiler.EndSample();
                return;
            }

            Profiler.EndSample();

            // Create new entry.
            Profiler.BeginSample("(COF)[ObjectRepository] Get > Create new entry");
            var newEntry = 0 < _pool.Count ? _pool.Pop() : new Entry();
            newEntry.storedObject = onCreate(source);
            newEntry.hash = hash;
            newEntry.reference = 1;
            _cache.Add(newEntry);
            Logging.Log(_name, $"Get(#{count}): {newEntry}");

            Release(ref obj);
            obj = newEntry.storedObject;
            Profiler.EndSample();
        }

        /// <summary>
        /// Release a object.
        /// </summary>
        public void Release(ref T obj)
        {
            if (ReferenceEquals(obj, null)) return;

            Profiler.BeginSample("(COF)[ObjectRepository] Release");
            for (var i = 0; i < _cache.Count; i++)
            {
                var entry = _cache[i];

                if (entry.storedObject != obj)
                {
                    continue;
                }

                if (--entry.reference <= 0)
                {
                    Profiler.BeginSample("(COF)[ObjectRepository] Release > RemoveAt");
                    _cache.RemoveAtFast(i);
                    Logging.Log(_name, $"Release(#{_cache.Count}): {entry}");
                    entry.Release(_onRelease);
                    _pool.Push(entry);
                    Profiler.EndSample();
                    break;
                }

                Logging.Log(_name, $"Release(#{count}): {entry}");
                break;
            }

            obj = null;
            Profiler.EndSample();
        }

        private class Entry
        {
            public Hash128 hash;
            public int reference;
            public T storedObject;

            public void Release(Action<T> onRelease)
            {
                reference = 0;
                if (storedObject)
                {
                    onRelease?.Invoke(storedObject);
                }

                storedObject = null;
            }

            public override string ToString()
            {
                return $"h{(uint)hash.GetHashCode()} (#{reference}), {storedObject}";
            }
        }
    }
}
