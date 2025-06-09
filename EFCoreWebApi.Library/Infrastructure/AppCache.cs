namespace EFCoreWebApi.Library
{

    /// <summary>
    /// An <see cref="IMemoryCache"/> wrapper 
    /// </summary>
    public class AppCache  
    {
        /* private */
        IMemoryCache Cache;

        /* construction */
        /// <summary>
        /// Constructor
        /// </summary>
        public AppCache(IMemoryCache Cache)
        {
            this.Cache = Cache;
        }

        /* public */
        /// <summary>
        /// Returns true if the key exists.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public bool ContainsKey(string Key)
        {
            return Cache.TryGetValue(Key, out var Item);
        }
        /// <summary>
        /// Removes an entry by a specified key, if is in the cache.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public void Remove(string Key)
        {
            if (ContainsKey(Key))
                Cache.Remove(Key);
        }

        /// <summary>
        /// Sets an entry under a specified key. Creates the entry if not already exists.
        /// <para>If is a new entry it will be removed from the cache after <see cref="DefaultEvictionTimeoutMinutes"/> minutes. </para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public void Set<T>(string Key, T Value)
        {
            Set(Key, Value, DefaultEvictionTimeoutMinutes);
        }
        /// <summary>
        /// Sets an entry under a specified key. Creates the entry if not already exists.
        /// <para>If is a new entry it will be removed from the cache after the specified timeout minutes. </para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public void Set<T>(string Key, T Value, int TimeoutMinutes)
        {
             
            Remove(Key);

            if (TimeoutMinutes > 0)
            {
                var o = new MemoryCacheEntryOptions();
                o.AbsoluteExpiration = DateTime.Now.AddMinutes(TimeoutMinutes); // An absolute expiration means a cached item will be removed an an explicit date and time
                o.SlidingExpiration = TimeSpan.FromMinutes(TimeoutMinutes);     // Sliding expiration means a cached item will be removed it is remains idle (not accessed) for a certain amount of time.

                Cache.Set(Key, Value, o);
            }
            else
            {
                Cache.Set(Key, Value);
            }
        }

        /// <summary>
        /// Returns true if an entry exists under a specified key. Returns the value too as out parameter.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public bool TryGetValue(string Key, out object Value)
        {
            return Cache.TryGetValue(Key, out Value);
        }
        /// <summary>
        /// Returns true if an entry exists under a specified key. Returns the value too as out parameter.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public bool TryGetValue<T>(string Key, out T Value)
        {
            return Cache.TryGetValue(Key, out Value);
        }

        /// <summary>
        /// Returns a value found under a specified key, if any, else returns the default value of the specified type argument.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public T Get<T>(string Key)
        {
            return Cache.Get<T>(Key);
        }
        /// <summary>
        /// Returns a value found under a specified key, if any, else returns null.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public object Get(string Key)
        {
            return Cache.Get(Key);
        }

        /// <summary>
        /// Removes and returns a value found under a specified key, if any, else returns the default value of the specified type argument.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public T Pop<T>(string Key)
        {
            T Result = Get<T>(Key);
            Remove(Key);
            return Result;
        }
        /// <summary>
        /// Removes and returns a value found under a specified key, if any, else returns null.
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public object Pop(string Key)
        {
            object Result = Get(Key);
            Remove(Key);
            return Result;
        }

        /// <summary>
        /// Returns a value found under a specified key.
        /// <para>If the key does not exist, it calls the specified loader call-back function </para>
        /// <para>The loader function should be defined as <c>object LoaderFunc()</c></para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public object Get(string Key, Func<object> LoaderFunc)
        {
            object Value;
            if (Cache.TryGetValue(Key, out Value))
                return Value;

            Value = LoaderFunc();
            Set(Key, Value);
            return Value;
        }
        /// <summary>
        /// Returns a value found under a specified key.
        /// <para>If the key does not exist, it calls the specified loader call-back function </para>
        /// <para>The loader function should be defined as <c>(int, object) LoaderFunc().</c></para>
        /// <para>The loader function must return a tuple where the first value is the eviction timeout and the second is the result object.</para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public object Get(string Key, Func<(int, object)> LoaderFunc)
        {
            object Value;
            if (Cache.TryGetValue(Key, out Value))
                return Value;

            (int, object) Result = LoaderFunc();
            Set(Key, Result.Item2, Result.Item1);
            return Result.Item2;
        }
        /// <summary>
        /// Returns a value found under a specified key.
        /// <para>If the key does not exist, it calls the specified loader call-back function </para>
        /// <para>The loader function should be defined as <c>T LoaderFunc&lt;T&gt;()</c></para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public T Get<T>(string Key, Func<T> LoaderFunc)
        {
            T Value;
            if (Cache.TryGetValue(Key, out Value))
                return Value;

            Value = LoaderFunc();
            Set(Key, Value);
            return Value;
        }
        /// <summary>
        /// Returns a value found under a specified key.
        /// <para>If the key does not exist, it calls the specified loader call-back function </para>
        /// <para>The loader function should be defined as <c>(int, T) LoaderFunc&lt;T&gt;().</c></para>
        /// <para>The loader function must return a tuple where the first value is the eviction timeout and the second is the result object.</para>
        /// <para>NOTE: Key is case sensitive.</para>
        /// </summary>
        public T Get<T>(string Key, Func<(int, T)> LoaderFunc)
        {
            T Value;
            if (Cache.TryGetValue(Key, out Value))
                return Value;

            (int, T) Result = LoaderFunc();
            Set(Key, Result.Item2, Result.Item1);
            return Result.Item2;
        }

        /* properties */
        /// <summary>
        /// The default eviction timeout of an entry from the cache, in minutes. 
        /// <para>Defaults to 0 which means "use the timeouts of the internal implementation".</para>
        /// </summary>
        public int DefaultEvictionTimeoutMinutes { get; set; }  
    }
}
