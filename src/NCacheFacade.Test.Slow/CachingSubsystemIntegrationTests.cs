namespace NCacheFacade.Test.Slow
{
    using System;
    using System.Web;
    using Compression;
    using Encryption;
    using Extensions;
    using GG.Infrastructure.Extensions;
    using MemcachedProviders.Cache;
    using NUnit.Framework;
    using ServiceLocation;

    /// <summary>
    ///   Memcached must be running for these tests to work.
    ///   See the app.config and the cofiguration information in the 
    ///   Memcached CacheImplementation.
    ///   Uncomment the NUnit attributes to run the tests.
    /// </summary>
    /// <remarks>
    ///   For debugging with verbose console output use:
    ///   [path to memcached]\memcached.exe -p 11211 -m 128 -vv
    /// </remarks>
    [TestFixture, Category("Integration"), Ignore("Memcached must be running for this test fixture to work.")]
    public class CachingSubsystemIntegrationTests
    {
        #region Setup/Teardown

        [SetUp]
        [IocConfiguration(new[]
                                       {
                                           typeof (Cache),
                                           typeof (ExampleCachingStrategy),
                                           typeof (ExampleCacheItemEncoder),
                                           typeof (SharpZipStringCompressor),
                                           typeof (SymmetricRijndaelStringEncryptor),
                                           typeof (Key)
                                       })]
        public void Setup()
        {
            _longTermBoundary = ExampleCachingStrategy.LongTermBoundary;
            _shortTimeSpan = 5.Seconds().ShorterThan(_longTermBoundary);
        }

        [TearDown]
        public void TearDown()
        {
            //asp.net cache does not have a clearall method, so use the one on
            //the asp.net cacheimplementation, not nice, but works.
            ICacheImplementation a = new AspDotNetDataCacheImplementation(Key.KeyCreator);
            a.RemoveAll();

            //empty memcached
            DistCache.RemoveAll();
        }

        #endregion

        private TimeSpan _longTermBoundary;
        private TimeSpan _shortTimeSpan;

        [TestFixtureSetUp]
        public void FixtureSetUp() {}

        [Test]
        public void Add_Long_Term_Absolute()
        {
            //arrange
            var cache = ResolveType.Of<ICache>();
            var key = Key.KeyCreator.Create(_longTermBoundary, StorageStyle.Unmodified, ExpirationType.Absolute,
                                            "friendly-name");
            var testType = new TestType(("test-string"));

            //act
            cache.Add(key, testType);

            //assert
            Assert.IsNotNull(key);
            Assert.AreEqual((DistCache.Get(key.ToString()) as TestType).TestString, "test-string");
        }

        [Test]
        public void Add_Overwrites_Existing_Cache_Items()
        {
            //arrange
            var cache = ResolveType.Of<ICache>();
            var key1 = Key.KeyCreator.Create(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Absolute,
                                             "friendly-name1");

            var testType1 = new TestType(("test-string-1"));
            var testType2 = new TestType(("test-string-2"));

            //act
            cache.Add(key1, testType1);
            cache.Add(key1, testType2);

            //assert
            Assert.AreEqual((HttpRuntime.Cache[key1.ToString()] as TestType).TestString, "test-string-2");
        }

        /// <summary>
        ///   End-to-end test for the ASP.NET data cache.
        /// </summary>
        [Test]
        public void Add_Short_Term_Absolute()
        {
            //arrange
            var cache = ResolveType.Of<ICache>();
            var key = Key.KeyCreator.Create(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Absolute,
                                            "friendly-name");
            var testType = new TestType(("test-string"));

            //act
            cache.Add(key, testType);

            //assert
            Assert.IsNotNull(key);
            Assert.AreEqual((HttpRuntime.Cache[key.ToString()] as TestType).TestString, "test-string");
        }

        /// <summary>
        ///   Doesn't actually test the sliding aspect directly.
        /// </summary>
        [Test]
        public void Add_Short_Term_Sliding()
        {
            //arrange
            var cache = ResolveType.Of<ICache>();
            var key = Key.KeyCreator.Create(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Sliding,
                                            "friendly-name");
            var testType = new TestType(("test-string"));

            //act
            cache.Add(key, testType);

            //assert
            Assert.IsNotNull(key);
            Assert.AreEqual((HttpRuntime.Cache[key.ToString()] as TestType).TestString, "test-string");
        }

        /// <summary>
        ///   Note memcached currently lies and *always* returns a count of zero
        ///   as there is no count method on the 3rd party interface.
        /// 
        ///   Change this to raising an exception?
        /// </summary>
        [Test]
        public void Count_All_Returns_The_Count()
        {
            //arrange
            var cache = ResolveType.Of<ICache>();
            var key1 = Key.KeyCreator.Create(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Absolute,
                                             "friendly-name1");
            var key2 = Key.KeyCreator.Create(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Absolute,
                                             "friendly-name2");
            var key3 = Key.KeyCreator.Create(_longTermBoundary, StorageStyle.Unmodified, ExpirationType.Absolute,
                                             "friendly-name3");

            var testType = new TestType(("test-string"));

            //act
            cache.Add(key1, testType);
            cache.Add(key2, testType);
            cache.Add(key3, testType);

            //assert
            Assert.AreEqual(HttpRuntime.Cache.Count, 2); //long term not counted
            Assert.AreEqual(cache.CountAll, 2);

            //act
            cache.RemoveAll();

            //assert
            Assert.AreEqual(cache.CountAll, 0);
        }

        [Test]
        public void Get_And_Indexer_Retrieves_An_Object_From_The_Appropriate_Cache_Implementation()
        {
            //arrange
            var cache = ResolveType.Of<ICache>();
            var key1 = Key.KeyCreator.Create(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Absolute,
                                             "friendly-name1");
            var key2 = Key.KeyCreator.Create(_longTermBoundary, StorageStyle.Unmodified, ExpirationType.Absolute,
                                             "friendly-name2");

            var testType1 = new TestType(("test-string-1"));
            var testType2 = new TestType(("test-string-2"));

            //act
            cache.Add(key1, testType1);
            cache.Add(key2, testType2);

            var shortObject = cache.Get<TestType>(key1);
            var longObject = cache.Get<TestType>(key2);
            var shortObjectFromIndexer = cache[key1.ToString()] as TestType;
            var longObjectFromIndexer = cache[key2.ToString()] as TestType;

            //assert
            Assert.AreEqual(shortObject.TestString, "test-string-1");
            Assert.AreEqual(longObject.TestString, "test-string-2");
            Assert.AreEqual(shortObjectFromIndexer.TestString, "test-string-1");
            Assert.AreEqual(longObjectFromIndexer.TestString, "test-string-2");
        }

        [Test]
        public void ICache_Can_Be_Resolved_In_DI_Container()
        {
            //act
            var cache = ResolveType.Of<ICache>();

            //assert
            Assert.IsNotNull(cache);
        }

        /// <summary>
        ///   Simulates real world use.
        /// </summary>
        [Test]
        public void Items_Can_Be_Added_And_Retrieved_From_The_Cache_Based_Upon_A_Key_Calculated_At_Runtime_Long_Term()
        {
            var cache = ResolveType.Of<ICache>();
            TestType topFeeds = null;
            var success = false;
            var key = new Key(_longTermBoundary, ExpirationType.Absolute, "top-ten-rss-feeds");

            if (cache[key.ToString()] == null)
            {
                topFeeds = new TestType(("top-ten-feeds"));
                success = cache.Add(key, topFeeds);
            }
            else
            {
                topFeeds = cache.Get<TestType>(key);
            }

            Assert.IsTrue(success);
            Assert.AreEqual(topFeeds.TestString, "top-ten-feeds");
            Assert.IsNotNull(cache.Get<TestType>(key));
            Assert.IsNotNull(DistCache.Get(key.ToString()));
            Assert.AreEqual((DistCache.Get(key.ToString()) as TestType).TestString, "top-ten-feeds");
        }

        /// <summary>
        ///   Simulates real world use.
        /// </summary>
        [Test]
        public void Items_Can_Be_Added_And_Retrieved_From_The_Cache_Based_Upon_A_Key_Calculated_At_Runtime_Short_Term()
        {
            var cache = ResolveType.Of<ICache>();
            TestType topFeeds = null;
            var success = false;
            var key = new Key(_shortTimeSpan, ExpirationType.Absolute, "top-ten-rss-feeds");

            if (cache[key.ToString()] == null)
            {
                topFeeds = new TestType(("top-ten-feeds"));
                success = cache.Add(key, topFeeds);
            }
            else
            {
                topFeeds = cache.Get<TestType>(key);
            }

            Assert.IsTrue(success);
            Assert.AreEqual(topFeeds.TestString, "top-ten-feeds");
            Assert.IsNotNull(cache.Get<TestType>(key));
            Assert.IsNotNull(HttpRuntime.Cache[key.ToString()]);
            Assert.AreEqual((HttpRuntime.Cache[key.ToString()] as TestType).TestString, "top-ten-feeds");
        }

        [Test]
        public void Remove_Removes_The_Specified_Item_From_The_Relevant_Cache()
        {
            //arrange
            var cache = ResolveType.Of<ICache>();
            var key1 = Key.KeyCreator.Create(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Absolute,
                                             "friendly-name1");
            var key2 = Key.KeyCreator.Create(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Absolute,
                                             "friendly-name2");
            var key3 = Key.KeyCreator.Create(_longTermBoundary, StorageStyle.Unmodified, ExpirationType.Absolute,
                                             "friendly-name3");

            var testType = new TestType(("test-string"));

            //act
            cache.Add(key1, testType);
            cache.Add(key2, testType);
            cache.Add(key3, testType);

            //act
            cache.Remove(key2);

            //assert
            Assert.IsNull(cache[key2.ToString()]);
            Assert.AreEqual(HttpRuntime.Cache.Count, 1);
            Assert.IsNotNull(cache[key1.ToString()]);
            Assert.IsNotNull(cache[key3.ToString()]);

            //act
            cache.Remove(key3);

            //assert
            Assert.AreEqual(HttpRuntime.Cache.Count, 1);
            Assert.IsNotNull(cache[key1.ToString()]);
            Assert.IsNull(cache[key2.ToString()]);
            Assert.IsNull(cache[key3.ToString()]);
        }
    }
}