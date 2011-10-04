namespace NCacheFacade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NBasicExtensionMethod;
    using NSure;

    public delegate ICacheImplementation
        CacheImplementationSelectorDelegate(TimeSpan durationToCacheItemFor,
                                            StorageStyle cacheStorageStyle,
                                            ExpirationType cacheExpirationType,
                                            Dictionary<string, ICacheImplementation> implementations);

    /// <summary>
    ///   Maintains a list of CacheImplementations and the logic
    ///   to choose between them (maintained within a delegate 
    ///   instance variable).
    ///   Todo: possibly add to the information passed to the delegate including 
    ///   information on whether a callback is required or cache depedencies,
    ///   thereby exposing the capability of the ASP.NET cache
    ///   and hiding the limitations of Memcached.
    ///   Alternatively, for specific applications, users could bypass this 
    ///   cache subsystem and go direct to asp.net.
    ///   Todo: supply with any scheduled IIS reset times so cache items are not 
    ///   lost in the reset (i.e. persist in any out of proc storage)
    /// </summary>
    public class CacheImplementationSelector : ICacheImplementationSelector
    {
        private const int MaxImplementations = 10; //arbitrary number

        private readonly Dictionary<string, ICacheImplementation> _implementations =
            new Dictionary<string, ICacheImplementation>();

        private readonly CacheImplementationSelectorDelegate _selectorDelegate;

        /// <summary>
        ///   Implementations placed in a dictionary indexed by fullname for convenience of use
        ///   elsewhere in the system.
        /// </summary>
        public CacheImplementationSelector(ICollection<ICacheImplementation> implementations,
                                           CacheImplementationSelectorDelegate selectorDelegate)
        {
            Ensure.That(implementations.IsNotNull(), "key not supplied")
                .And(selectorDelegate.IsNotNull(), "selectorDelegate not supplied")
                .And(implementations.Count.IsBetweenInclusive(1, MaxImplementations), "invalid number of cacheImplementations");

            _selectorDelegate = selectorDelegate;
            foreach (var i in implementations.Where(i => i != null))
            {
                _implementations.Add(i.GetType().FullName, i);
            }

            if (!_implementations.Where(i => i.Value.IsEnabled).Any())
                throw new ArgumentException("One or more implementations supplied must be enabled.", "implementations");
        }

        public ICacheImplementation Select(TimeSpan durationToCacheItemFor,
                                           StorageStyle cacheStorageStyle,
                                           ExpirationType expirationType)
        {
            Ensure.That(durationToCacheItemFor.IsGreaterThanZero(), "durationToCacheItemFor");

            return _selectorDelegate(durationToCacheItemFor,
                                     cacheStorageStyle,
                                     expirationType,
                                     _implementations);
        }

        public Dictionary<string, ICacheImplementation> CacheImplementations
        {
            get { return _implementations; }
        }
    }
}