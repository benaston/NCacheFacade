namespace NCacheFacade.Aop
{
    using System;
    using System.Reflection;
    using Bjma.Utility.Aop;
    using NBasicExtensionMethod;
    using NSure;
    using PostSharp.Aspects;
    using PostSharp.Extensibility;
    using ArgumentException = NHelpfulException.FrameworkExceptions.ArgumentException;
    using ArgumentNullException = NHelpfulException.FrameworkExceptions.ArgumentNullException;

    /// <summary>
    ///   Responsible for providing aspect-oriented 
    ///   caching functionality. Remember to guard 
    ///   against an explosion in cache items by 
    ///   limiting the number of possible keys for 
    ///   cache items.
    /// </summary>
    /// <remarks>
    ///   NOTE: configurable disabling might be added 
    ///   to this by extension - possibly by incorporation 
    ///   into a more abstract type such as "Service" or 
    ///   "AggregateRoot".
    /// </remarks>
    [Serializable]
    public class CacheAspect : AsyncMethodInterceptionAspect
    {
        private const int OneYearInSeconds = 31556926;
        private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(5);
        private static readonly ExpirationType DefaultCacheExpirationType = ExpirationType.Sliding;
        private readonly Type _cacheType;
        private readonly TimeSpan _durationToRecalculate;
        private readonly TimeSpan _durationToStore;
        private readonly ExpirationType _expirationType;
        private readonly Type _keyCreationStrategyType;
        private readonly Type _recalculationStrategyType;
        private readonly StorageStyle _storageStyle;
        private ICache _cache; //was static with lock
        private IFriendlyNameCreationStrategy _keyCreationStrategy; //was static with lock
        private ICacheItemRecalculationStrategy _recalculationStrategy;

        /// <remarks>
        ///   NOTE: PostSharp does not appear to support 
        ///   initialization of static variables from 
        ///   aspect constructors.
        /// </remarks>
        public CacheAspect(Type cacheType, Type keyCreationStrategyType)
        {
            _cacheType = cacheType;
            _keyCreationStrategyType = keyCreationStrategyType;
        }

        public CacheAspect(Type cacheType,
                           Type keyCreationStrategyType,
                           int durationToStoreInSeconds,
                           StorageStyle storageStyle,
                           ExpirationType expirationType)
        {
            Ensure.That<ArgumentNullException>(cacheType.IsNotNull(), "cacheType")
                .And<ArgumentNullException>(keyCreationStrategyType.IsNotNull(), "keyCreationStrategyType")
                .And<ArgumentException>(durationToStoreInSeconds.IsBetween(0, OneYearInSeconds),
                                        "durationToStoreInSeconds");

            _cacheType = cacheType;
            _keyCreationStrategyType = keyCreationStrategyType;
            _durationToStore = TimeSpan.FromSeconds(durationToStoreInSeconds);
            _storageStyle = storageStyle;
            _expirationType = expirationType;
        }

        public CacheAspect(Type cacheType,
                           Type keyCreationStrategyType,
                           int durationToStoreInSeconds,
                           StorageStyle storageStyle,
                           ExpirationType expirationType,
                           Type recalculationStrategyType,
                           int durationToRecalculateInSeconds)
        {
            Ensure.That<ArgumentNullException>(cacheType.IsNotNull(), "cacheType")
                .And<ArgumentNullException>(keyCreationStrategyType.IsNotNull(), "keyCreationStrategyType")
                .And<ArgumentException>(durationToStoreInSeconds.IsBetween(0, OneYearInSeconds),
                                        "durationToStoreInSeconds")
                .And<ArgumentNullException>(recalculationStrategyType.IsNotNull(), "recalculationStrategyType")
                .And<ArgumentException>(durationToRecalculateInSeconds.IsBetween(0, durationToStoreInSeconds),
                                        "durationToRecalculateInSeconds");

            _cacheType = cacheType;
            _keyCreationStrategyType = keyCreationStrategyType;
            _storageStyle = storageStyle;
            _expirationType = expirationType;
            _recalculationStrategyType = recalculationStrategyType;
            _durationToStore = TimeSpan.FromSeconds(durationToStoreInSeconds);
            _durationToRecalculate = TimeSpan.FromSeconds(durationToRecalculateInSeconds);
        }

        /// <remarks>
        ///   Consider alternatives to instantiating instances 
        ///   of the specified types using Activator.CreateInstance.
        /// </remarks>
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            Ensure.That<ArgumentNullException>(args.IsNotNull(), "args");

            _cache = (ICache) Activator.CreateInstance(_cacheType);
            _keyCreationStrategy = (IFriendlyNameCreationStrategy) Activator.CreateInstance(_keyCreationStrategyType);

            if (_recalculationStrategyType.IsNotNull())
            {
                _recalculationStrategy =
                    (ICacheItemRecalculationStrategy)
                    Activator.CreateInstance(_recalculationStrategyType, _durationToRecalculate);
            }

            var friendlyName = _keyCreationStrategy.GenerateFriendlyName(args);
            var key = _durationToStore == default(TimeSpan) //infer from the variables set, the key type to create?
                          ? new Key(DefaultCacheDuration, DefaultCacheExpirationType, friendlyName)
                          : new Key(_durationToStore, _storageStyle, _expirationType, friendlyName);
            object cacheItem;
            if ((cacheItem = _cache[key.ToString()]).IsNotNull())
            {
                args.ReturnValue = cacheItem;
            }
            else
            {
                args.Proceed();
                _cache.Add(key, args.ReturnValue);
                if (_recalculationStrategy.IsNotNull())
                    AsyncCacheItemRecalculator.EnsureIsStarted(key, _recalculationStrategy, args, _cache);
            }
        }

        public override bool CompileTimeValidate(MethodBase method)
        {
            if (method is ConstructorInfo) // Don't apply to constructors.
            {
                Message.Write(SeverityType.Error, "CX0001", "Cannot cache constructors.");

                return false;
            }

            var methodInfo = (MethodInfo) method;

            if (methodInfo.ReturnType.Name == "Void") // Don't apply to void methods.
            {
                Message.Write(SeverityType.Error, "CX0002", "Cannot cache void methods.");

                return false;
            }

            var parameters = method.GetParameters(); // Does not support out parameters.
            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].IsOut)
                {
                    Message.Write(SeverityType.Error, "CX0003", "Cannot cache methods with out parameters.");

                    return false;
                }
            }

            return true;
        }
    }
}