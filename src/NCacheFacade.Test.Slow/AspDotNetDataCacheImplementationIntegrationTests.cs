namespace NCacheFacade.Test.Slow
{
    using System;
    using System.Web;
    using System.Web.Caching;
    using Extensions;
    using GG.Infrastructure.Extensions;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Cache = Caching.Cache;

    /// <summary>
    ///   Uncomment the TestFixture and Test attributes to run the tests.
    /// </summary>
    [TestFixture]
    public class AspDotNetDataCacheImplementationIntegrationTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            ICacheImplementation a = new AspDotNetDataCacheImplementation(Key.KeyCreator);
            a.RemoveAll();

            _stubKeyCreator = MockRepository.GenerateStub<ICacheKeyCreator>();
            _c = new AspDotNetDataCacheImplementation(_stubKeyCreator);
            _tomorrow = 1.Day().Hence();
            _oneHour = 1.Hour();
            _testString = "test";
        }

        [TearDown]
        public void TearDown()
        {
            //asp.net cache does not have a clearall method, so use the one on
            //the asp.net cacheimplementation, not nice, but works.
            ICacheImplementation a = new AspDotNetDataCacheImplementation(Key.KeyCreator);
            a.RemoveAll();
        }

        #endregion

        private ICacheKeyCreator _stubKeyCreator;
        private ICacheImplementation _c;
        private DateTime _tomorrow;
        private TimeSpan _oneHour;
        private string _testString;

        [TestFixtureSetUp]
        public void FixtureSetUp() {}

        [Test]
        public void Add_Absolute_Adds_An_Item_To_The_HttpRuntimeCache()
        {
            //arrange            
            _stubKeyCreator.Stub(x => x.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, _testString))
                .Return(Key.KeyCreator);
            Assert.AreEqual(HttpRuntime.Cache.Count, 0);

            //act            
            var key = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, _testString);
            _c.Add(key, new TestType("test-1"));

            //assert                        
            Assert.AreEqual(_c.Count, 1);
            Assert.AreEqual((HttpRuntime.Cache[key.ToString()] as TestType).TestString, "test-1");
        }

        [Test]
        public void Count_Returns_The_Number_Of_Items_In_The_Cache()
        {
            //arrange
            Assert.AreEqual(HttpRuntime.Cache.Count, 0);
            HttpRuntime.Cache.Add("test1", new object(), null, _tomorrow, System.Web.Caching.Cache.NoSlidingExpiration,
                                  CacheItemPriority.Default, null);

            //assert            
            Assert.AreEqual(_c.Count, 1);

            HttpRuntime.Cache.Add("test2", new object(), null, _tomorrow, System.Web.Caching.Cache.NoSlidingExpiration,
                                  CacheItemPriority.Default, null);

            //assert            
            Assert.AreEqual(_c.Count, 2);
        }

        [Test]
        public void Get_Retrieves_An_Item_By_Key()
        {
            //arrange
            Assert.AreEqual(HttpRuntime.Cache.Count, 0);
            var key1 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "1");
            _c.Add(key1, new TestType("test-1"));
            var key2 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "2");
            _c.Add(key2, new TestType("test-2"));
            var key3 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "3");
            _c.Add(key3, new TestType("test-3"));

            //act            
            var key = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Sliding, _testString);
            var o = _c.Get(key2) as TestType;

            //assert            
            Assert.IsNotNull(o);
            Assert.AreEqual(o.TestString, "test-2");
        }

        [Test]
        public void RemoveAll_Empties_The_Cache()
        {
            //arrange
            Assert.AreEqual(HttpRuntime.Cache.Count, 0);
            HttpRuntime.Cache.Add("test1", new object(), null, _tomorrow, System.Web.Caching.Cache.NoSlidingExpiration,
                                  CacheItemPriority.Default, null);
            HttpRuntime.Cache.Add("test2", new object(), null, _tomorrow, System.Web.Caching.Cache.NoSlidingExpiration,
                                  CacheItemPriority.Default, null);
            HttpRuntime.Cache.Add("test3", new object(), null, _tomorrow, System.Web.Caching.Cache.NoSlidingExpiration,
                                  CacheItemPriority.Default, null);
            var key1 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "1");
            _c.Add(key1, new TestType("test-1"));
            var key2 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "2");
            _c.Add(key2, new TestType("test-2"));
            var key3 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "3");
            _c.Add(key3, new TestType("test-3"));
            Assert.AreEqual(HttpRuntime.Cache.Count, 6);

            //act            
            _c.RemoveAll();

            //assert            
            Assert.AreEqual(HttpRuntime.Cache.Count, 0);
        }

        [Test]
        public void Remove_Removes_An_Item_From_The_HttpRuntimeCache()
        {
            //arrange                        
            Assert.AreEqual(HttpRuntime.Cache.Count, 0);
            var key1 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "1");
            _c.Add(key1, new TestType("test-1"));
            var key2 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "2");
            _c.Add(key2, new TestType("test-2"));
            var key3 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "3");
            _c.Add(key3, new TestType("test-3"));
            Assert.AreEqual(HttpRuntime.Cache.Count, 3);

            //act            
            _c.Remove(key2);

            //assert                        
            Assert.AreEqual(HttpRuntime.Cache.Count, 2);
            Assert.IsNull(HttpRuntime.Cache[key2.ToString()]);
        }
    }
}