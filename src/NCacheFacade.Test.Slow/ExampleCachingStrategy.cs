namespace NCacheFacade.Test.Slow
{
    using System;

    /// <summary>
    ///   Responsible for defining a concrete instance of the abstract CachingStrategy type.
    ///   Specifies the CacheImplementations to use* and the logic to choose between them; 
    ///   i.e. configures the CachingStrategy object for use by  the application.
    ///   *the ASP.NET Data Cache and Memcached (via the .NET providers from
    ///   here: http://www.codeplex.com/memcachedproviders.
    ///   Memcached itself available from here: http://jehiah.cz/projects/memcached-win32/.
    ///   CacheImplementations are hard coded in the call to the base constructor.
    ///   Logic for choosing a CacheImplementation resides in the delegate in
    ///   the constructor.
    /// </summary>
    public class ExampleCachingStrategy : CachingStrategy
    {
        public static readonly TimeSpan LongTermBoundary = 10.Minutes(); //chosen arbitrarily

        private static readonly string AspDotNetImplementationTypeName =
            typeof (AspDotNetDataCacheImplementation).FullName;

        private static readonly string MemcachedImplementationTypeName = typeof (MemcachedCacheImplementation).FullName;

        public ExampleCachingStrategy()
            : base(new CacheImplementationSelector(new List<ICacheImplementation>
                                                       {
                                                           new AspDotNetDataCacheImplementation(Key.KeyCreator),
                                                           new MemcachedCacheImplementation(Key.KeyCreator)
                                                       }, SelectCacheImplementation)) {}

        /// <summary>
        ///   This is the method to be supplied to the constructor of 
        ///   the base type as the delegate to use for cache selection.
        ///   Selects a cache based on the heuristic of duration to 
        ///   store for, followed by the availability of the requested 
        ///   caching functionality. This logic serves as a workable 
        ///   example, rather than a recommended strategy. For example, 
        ///   the fallback case is handled without error or warning.        
        ///   Public rather than protected for testing purposes.
        /// </summary>
        /// <remarks>
        ///   NOTE: BA; should we throw an exception if the user requests 
        ///   a storage strategy that cannot be fulfilled due to 
        ///   limitations of the underlying cache?
        ///   NOTE: BA; Potentially have a flag to indicate strictness of
        ///   choice of cache - i.e. does the cache really need to 
        ///   support all the desired features, or can we substitute 
        ///   them?
        ///   "narrow down choice of caches to those that can be used, 
        ///   then choose the enabled one according to priority".
        /// </remarks>
        /// <param name = "implementations">Ordered by preference of use 
        ///   with highest pref first.</param>
        public static ICacheImplementation SelectCacheImplementation(TimeSpan durationToCacheItemFor,
                                                                     StorageStyle cacheStorageStyle,
                                                                     ExpirationType cacheExpirationType,
                                                                     Dictionary<string, ICacheImplementation>
                                                                         implementations)
        {
            Ensure.That<FriendlyExceptions.ArgumentOutOfRangeException>(durationToCacheItemFor.IsGreaterThanZero(),
                                                                        "durationToCacheFor")
                .And<FriendlyExceptions.ArgumentNullException>(implementations.IsNotNull(), "implementations")
                .And<FriendlyExceptions.ArgumentException>(implementations.Count.Is(2), "implementations")
                .And<MissingCacheImplementationArgumentException>(
                    implementations.ContainsKey(MemcachedImplementationTypeName), "memcached missing")
                .And<MissingCacheImplementationArgumentException>(
                    implementations.ContainsKey(AspDotNetImplementationTypeName), "asp.net data cache missing");

            ICacheImplementation cacheToUse = null;

            //simply tries to select memcached if an extended caching period is requested
            //otherwise attempts to fallback to ASP.NET data cache. This might occur because 
            //the caching expiration type is not supported by memcached.
            if (durationToCacheItemFor >= LongTermBoundary)
            {
                if (implementations[MemcachedImplementationTypeName].Supports((cacheExpirationType))
                    && implementations[MemcachedImplementationTypeName].IsEnabled)
                    cacheToUse = implementations[MemcachedImplementationTypeName];

                if (cacheToUse == null
                    && implementations[AspDotNetImplementationTypeName].Supports((cacheExpirationType))
                    && implementations[AspDotNetImplementationTypeName].IsEnabled)
                    cacheToUse = implementations[AspDotNetImplementationTypeName];

                Ensure.That(cacheToUse.IsNotNull(), () => { throw new CacheSelectionException(implementations); });
            }
            else
            {
                if (implementations[AspDotNetImplementationTypeName].Supports((cacheExpirationType))
                    && implementations[AspDotNetImplementationTypeName].IsEnabled)
                    cacheToUse = implementations[AspDotNetImplementationTypeName];

                if (cacheToUse == null
                    && implementations[MemcachedImplementationTypeName].Supports((cacheExpirationType))
                    && implementations[MemcachedImplementationTypeName].IsEnabled)
                    cacheToUse = implementations[MemcachedImplementationTypeName];

                Ensure.That(cacheToUse.IsNotNull(), () => { throw new CacheSelectionException(implementations); });
            }

            return cacheToUse;
        }
    }
}