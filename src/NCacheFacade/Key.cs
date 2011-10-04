namespace NCacheFacade
{
    using System;
    using NBasicExtensionMethod;
    using NSure;

    /// <summary>
    ///   Used to conveniently create, store and "decode" cache keys.
    ///   The static GetKeyCreator method returns an uninitialised 
    ///   instance that is only good for key creation.
    ///   Should prob refactor the encode / decode functionality out,
    ///   as this type has two big responsibilities - DTO and 
    ///   key creator making a "weird" implementation.
    /// </summary>
    public class Key : ICacheKeyCreator
    {
        public const int MaxKeyLength = 5000; //arbitrary 
        public const int MaxFriendlyNameLength = 255;
        public const int MaxFriendlyNameLengthEncoded = 1500;

        /// <summary>
        ///   Note: do not use fullstop (used in durationToStore) 
        ///   or any of the base64 encoding characters 
        ///   (http://en.wikipedia.org/wiki/Base64) as a delimter.
        /// </summary>
        private const char KeyPartDelimiter = ':';

        /// <summary>
        ///   Used for an instance for simply generating keys.
        ///   Hackish, I know.
        /// </summary>
        private Key() {}

        /// <summary>
        ///   Responsible for enabling the construction of the object 
        ///   from a string representation.
        /// </summary>
        /// <param name = "key">The string representation of a Key instance.</param>
        public Key(string key)
        {
            Ensure.That(key.IsNotNullOrWhiteSpace(), "key is empty")
                .And(key.Length.IsLessThan(MaxKeyLength), "invalid key length");

            var keyParts = key.Split(KeyPartDelimiter);

            try
            {
                DurationToStore = TimeSpan.FromSeconds(double.Parse(keyParts[0]));
                StorageStyle = (StorageStyle) int.Parse(keyParts[1]);
                CacheExpirationType = (ExpirationType) int.Parse(keyParts[2]);
                FriendlyName = keyParts[3] == String.Empty
                                   ? String.Empty
                                   : new Base64Serializer<string>().DeSerialize(keyParts[3]);
            }
            catch (Exception)
            {
                throw new InvalidCacheKeyException(key);
            }
        }

        /// <summary>
        ///   Convenience consructor uses the default storage style.
        ///   Todo extract default storage style out into a member of the enum.
        /// </summary>
        public Key(TimeSpan durationToStore, ExpirationType cacheExpirationType, string friendlyName)
            : this(durationToStore,
                   StorageStyle.Unmodified,
                   cacheExpirationType,
                   friendlyName) {}

        /// <summary>
        ///   Made public to make API more intuitive, even though it opens up
        ///   two routes to do the same thing (this plus the "ICacheKeyCreator.Create"
        ///   method.
        /// </summary>
        public Key(TimeSpan durationToStore, StorageStyle cacheStorageStyle, ExpirationType cacheExpirationType,
                   string friendlyName)
        {
            Ensure.That(durationToStore.IsGreaterThan(TimeSpan.Zero), "durationToStore must be greater than zero")
                .And(friendlyName.IsNotNullOrWhiteSpace(), "friendlyName not supplied")
                .And(friendlyName.Length.IsLessThan(MaxFriendlyNameLength), "friendlyName too long");

            DurationToStore = durationToStore;
            StorageStyle = cacheStorageStyle;
            CacheExpirationType = cacheExpirationType;
            FriendlyName = friendlyName;
        }

        /// <summary>
        ///   News-up an instance for purpose of exposing the key 
        ///   generation functionality. This functionality should 
        ///   probably be moved onto a different type.
        /// </summary>
        public static Key KeyCreator
        {
            get { return new Key(); }
        }

        public TimeSpan DurationToStore { get; private set; }
        public StorageStyle StorageStyle { get; private set; }
        public ExpirationType CacheExpirationType { get; private set; }

        public bool IsItemCompressed
        {
            get { return (StorageStyle == StorageStyle.CompressedAndEncrypted || StorageStyle == StorageStyle.Compressed); }
        }

        public bool IsItemEncrypted
        {
            get { return (StorageStyle == StorageStyle.CompressedAndEncrypted || StorageStyle == StorageStyle.Encrypted); }
        }

        public string FriendlyName { get; private set; }

        /// <summary>
        ///   Enables the ICacheKeyCreator to be injected into 
        ///   types (constructors cannot be constrained by interfaces).
        /// </summary>
        public Key Create(TimeSpan durationToStore, StorageStyle cacheStorageStyle, ExpirationType cacheExpirationType,
                          string friendlyName)
        {
            return new Key(durationToStore, cacheStorageStyle, cacheExpirationType, friendlyName);
        }

        public override string ToString()
        {
            var durationToStoreInSeconds = DurationToStore.TotalSeconds.ToString();
            var storageStyle = ((int) StorageStyle).ToString();
            var cacheExpirationType = ((int) CacheExpirationType).ToString();
            var encodedFriendlyName = FriendlyName == null
                                          ? String.Empty
                                          : new Base64Serializer<string>().Serialize(FriendlyName);

            const string format = "{1}{0}{2}{0}{3}{0}{4}";
            return String.Format(format, new[]
                                             {
                                                 KeyPartDelimiter.ToString(),
                                                 durationToStoreInSeconds,
                                                 storageStyle,
                                                 cacheExpirationType,
                                                 encodedFriendlyName
                                             });
        }
    }
}