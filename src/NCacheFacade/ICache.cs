namespace NCacheFacade
{
    using System;

    /// <summary>
    /// Defines interface for our implementation of a cache facade which will
    /// transparently use the most appropriate caching implementation depending 
    /// on configuration settings and duration of caching.
    /// Could be extended to explicitly support per-user caching, with the supply of a user ID.
    /// </summary>
    /// <example>
     //var cache = ResolveType.Of<ICache>(); //suggest making this an instance variable
     //var key = new Key(TimeSpan.FromMinutes(5), ExpirationType.Whatever, "unique-name-for-the-cache-item");
     ////-- or -- 
     ////var key = new Key(TimeSpan.FromMinutes(5), StorageStyle.Whatever, ExpirationType.Whatever, "unique-name-for-the-cache-item");
     //T cachedObject;
    
     //if (cache[key.ToString()] == null)
     //{
     //    cachedObject = new T();
     //    bool success = cache.Add(key, cachedObject);
     //}
     //else
     //{
     //    cachedObject = cache.Get<T>(key);
     //}
    /// </example>
    public interface ICache
    {
        bool Add(Key key, object value); //, CacheItemRemovedCallback onRemoveCallback);

        //bool Insert(CacheKey key, object value);
        
        /// <summary>
        /// Returns the number of objects in the Cache, aggregating all objects in all caches.
        /// </summary>        
        int CountAll { get; }
        
        T Get<T>(Key key) where T : class;
        
        Object this[string key] { get; } //generic indexers not possible in C# 4.0 (dynamics might help)
        
        void Remove(Key key);
        
        void RemoveAll();
    }
}