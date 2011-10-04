namespace NCacheFacade
{
    using NBasicExtensionMethod;
    using NHelpfulException.FrameworkExceptions;
    using NSure;

    /// <summary>
    ///   Maintains the state of the strategy and coordinates 
    ///   the encoding / decoding of cache items.
    /// </summary>
    /// <remarks>
    ///   Possible improvements include support for callbacks.
    /// </remarks>
    public class Cache : ICache
    {
        private readonly ICacheItemEncoder _encoder;
        private readonly ICachingStrategy _strategy;

        public Cache(ICachingStrategy strategy, ICacheItemEncoder encoder)
        {
            Ensure.That(strategy.IsNotNull(), "strategy not supplied")
                .And(encoder.IsNotNull(), "encoder not supplied");

            _strategy = strategy;
            _encoder = encoder;
        }

        bool ICache.Add(Key key, object value)
        {
            Ensure.That<ArgumentNullException>(key.IsNotNull(), "key")
                .And<ArgumentNullException>(value.IsNotNull(), "value");

            return _strategy.Add(key, _encoder.Encode(value, key.StorageStyle));
        }

        int ICache.CountAll
        {
            get { return _strategy.CountAll; }
        }

        T ICache.Get<T>(Key key)
        {
            Ensure.That(key.IsNotNull(), "key not supplied");

            var objectToReturn = _strategy.Get(key);

            if (objectToReturn != null)
                objectToReturn = _encoder.Decode<object>(objectToReturn, key.StorageStyle);

            return objectToReturn as T;
        }

        object ICache.this[string key]
        {
            get
            {
                Ensure.That(key.IsNotNullOrWhiteSpace(), "key not supplied or empty")
                    .And(key.Length.IsLessThan(Key.MaxKeyLength), "key too long");

                var cacheKey = new Key(key);
                var objectToReturn = _strategy.Get(cacheKey);

                if (objectToReturn != null)
                    objectToReturn = _encoder.Decode<object>(objectToReturn, cacheKey.StorageStyle);

                return objectToReturn;
            }
        }

        void ICache.Remove(Key key)
        {
            Ensure.That(key.IsNotNull(), "key not supplied");

            _strategy.Remove(key);
        }

        void ICache.RemoveAll()
        {
            _strategy.RemoveAll();
        }
    }
}