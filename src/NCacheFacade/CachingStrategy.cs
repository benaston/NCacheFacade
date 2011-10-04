namespace NCacheFacade
{
    using System.Linq;
    using NBasicExtensionMethod;
    using NSure;

    /// <summary>
    ///   A CachingStrategy mediates access to one or more CacheImplementations
    ///   associated with it at construction-time by passing in the names of
    ///   the CacheImplementation types to use.
    ///   For example, you may choose to use a particular CacheImplementation
    ///   if the duration the item is to be stored for is more than 5
    ///   minutes.
    ///   CacheImplementations may be marked as "disabled". The constructor
    ///   for the CachingStrategy checks that at least one of the
    ///   CacheImplementations is enabled (there would be no point in
    ///   continuing without a cache to use!).
    /// </summary>
    public abstract class CachingStrategy : ICachingStrategy
    {
        private readonly ICacheImplementationSelector _selector;

        protected CachingStrategy(ICacheImplementationSelector selector)
        {
            Ensure.That(selector.IsNotNull(), "selector not supplied");

            _selector = selector;
        }

        bool ICachingStrategy.Add(Key key, object value)
        {
            return _selector.Select(key.DurationToStore, key.StorageStyle, key.CacheExpirationType)
                .Add(key, value);
        }

        /// <summary>
        ///   Aggregates counts for all CacheImplementations in use.
        ///   To find the count for a given cache you need to instantiate 
        ///   the CacheImplementation you wish to query and invoke the 
        ///   method directly.
        /// </summary>
        int ICachingStrategy.CountAll
        {
            get { return _selector.CacheImplementations.Sum(cacheImplementation => cacheImplementation.Value.Count); }
        }

        /// <summary>
        ///   Does this have any implications for overriding?
        /// </summary>
        object ICachingStrategy.Get(Key key)
        {
            return _selector.Select(key.DurationToStore, key.StorageStyle, key.CacheExpirationType).Get(key);
        }

        /// <summary>
        ///   Removes the item with the specified key from the cache.
        /// </summary>
        public virtual void Remove(Key key)
        {
            _selector.Select(key.DurationToStore, key.StorageStyle, key.CacheExpirationType).Remove(key);
        }

        /// <summary>
        ///   Clears all CacheImplementations. 
        ///   To find clear a given cache you need to instantiate 
        ///   the CacheImplementation you wish to clear and invoke the 
        ///   method directly.
        /// </summary>
        public void RemoveAll()
        {
            foreach (var cacheImplementation in _selector.CacheImplementations) cacheImplementation.Value.RemoveAll();
        }
    }
}