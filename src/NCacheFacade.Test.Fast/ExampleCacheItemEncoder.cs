namespace NCacheFacade.Test.Fast
{
    /// <summary>
    ///   Responsible for defining an example mechanism for the encoding 
    ///   and decoding of cache items based on the StorageStyle supplied.
    /// </summary>
    public class ExampleCacheItemEncoder : ICacheItemEncoder
    {
        private readonly IStringCompressor _compressor;
        private readonly IStringEncryptor _encryptor;

        public ExampleCacheItemEncoder(IStringCompressor compressor,
                                       IStringEncryptor encryptor)
        {
            Ensure.That<ArgumentNullException>(compressor.IsNotNull(), "compressor")
                .And<ArgumentNullException>(encryptor.IsNotNull(), "encryptor");

            _compressor = compressor;
            _encryptor = encryptor;
        }

        public object Encode<T>(T value, StorageStyle cacheStorageStyle) where T : class
        {
            Ensure.That(value.IsNotNull(), "value not supplied");

            object objectToReturn = value;
            if (cacheStorageStyle != StorageStyle.Unmodified)
            {
                objectToReturn = ObjectSerializer.SerializeObjectToXmlString(value);

                if (cacheStorageStyle == StorageStyle.Compressed ||
                    cacheStorageStyle == StorageStyle.CompressedAndEncrypted)
                    objectToReturn = _compressor.Compress((string) objectToReturn);

                if (cacheStorageStyle == StorageStyle.Encrypted ||
                    cacheStorageStyle == StorageStyle.CompressedAndEncrypted)
                    objectToReturn = _encryptor.Encrypt((string) objectToReturn);
            }

            return objectToReturn;
        }

        public T Decode<T>(object value, StorageStyle cacheStorageStyle) where T : class
        {
            Ensure.That(value.IsNotNull(), "value not supplied");

            T valueToReturn;
            var isObjectSerialized = false;

            if (cacheStorageStyle == StorageStyle.Encrypted ||
                cacheStorageStyle == StorageStyle.CompressedAndEncrypted)
            {
                value = _encryptor.Decrypt(value as string);
                isObjectSerialized = true;
            }

            if (cacheStorageStyle == StorageStyle.Compressed ||
                cacheStorageStyle == StorageStyle.CompressedAndEncrypted)
            {
                value = _compressor.Decompress(value as string);
                isObjectSerialized = true;
            }

            if (isObjectSerialized)
                valueToReturn =
                    ObjectSerializer.DeSerializeObjectFromXmlString<T>(value as string);
            else
                valueToReturn = value as T;

            return valueToReturn;
        }
    }
}