namespace NCacheFacade.Test.Slow
{
    using Castle.MicroKernel.Releasers;
    using Castle.Windsor;
    using Compression;
    using Encryption;
    using NUnit.Framework;
    using ServiceLocation;

    [TestFixture, Category("Integration")]
    public class EncodingIntegrationTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _container = new WindsorContainer();
            _container.Kernel.ReleasePolicy = new NoTrackingReleasePolicy();
            ResolveType.Initialise(_container);

            //register with DI container            
            ResolveType.RegisterComponent(typeof (ExampleCacheItemEncoder));
            ResolveType.RegisterComponent(typeof (SharpZipStringCompressor));
            ResolveType.RegisterComponent(typeof (SymmetricRijndaelStringEncryptor));
        }

        [TearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        #endregion

        private IWindsorContainer _container;

        [TestFixtureSetUp]
        public void FixtureSetUp() {}

        /// <summary>
        ///   End-to-end test cache item encoding.
        /// </summary>
        [Test]
        public void Encode_Will_Reversibly_Serialize_And_Compress_An_Object()
        {
            //arrange
            var testObject = new TestType("test-object-for-compression");
            var encoder = ResolveType.Of<ICacheItemEncoder>();

            //act
            var compressedObject = encoder.Encode(testObject, StorageStyle.Compressed);
            var decompressedObject = encoder.Decode<TestType>(compressedObject, StorageStyle.Compressed);

            Assert.IsNotNull(decompressedObject);
            Assert.AreEqual(decompressedObject.TestString, "test-object-for-compression");
        }

        /// <summary>
        ///   Note doesn't test the encryption directly.
        /// </summary>
        [Test]
        public void Encode_Will_Reversibly_Serialize_And_Compress_And_Ecrypt_An_Object()
        {
            //arrange
            var testObject = new TestType("test-object-for-compression-and-encryption");
            var encoder = ResolveType.Of<ICacheItemEncoder>();

            //act
            var compressedAndEncryptedObject = encoder.Encode(testObject, StorageStyle.CompressedAndEncrypted);
            var decryptedAndDecompressedObject = encoder.Decode<TestType>(compressedAndEncryptedObject,
                                                                          StorageStyle.CompressedAndEncrypted);

            Assert.IsNotNull(decryptedAndDecompressedObject);
            Assert.AreEqual(decryptedAndDecompressedObject.TestString, "test-object-for-compression-and-encryption");
        }

        [Test]
        public void Encode_Will_Reversibly_Serialize_And_Ecrypt_An_Object()
        {
            //arrange
            var testObject = new TestType("test-object-for-compression");
            var encoder = ResolveType.Of<ICacheItemEncoder>();

            //act
            var encryptedObject = encoder.Encode(testObject, StorageStyle.Encrypted);
            var decryptedObject = encoder.Decode<TestType>(encryptedObject, StorageStyle.Encrypted);

            Assert.IsNotNull(decryptedObject);
            Assert.AreEqual(decryptedObject.TestString, "test-object-for-compression");
        }

        [Test]
        public void ICacheItemEncoder_Can_Be_Resolved_In_DI_Container()
        {
            var encoder = ResolveType.Of<ICacheItemEncoder>();
            Assert.IsNotNull(encoder);
        }
    }
}