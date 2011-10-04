namespace NCacheFacade.Test.Slow
{
    using System;
    using MemcachedProviders.Cache;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// A local Memcached server must be running for these tests to pass.
    /// Uncomment the TestFixture and Test attributes to run the tests. 
    /// Uncommenting: ctrl+k,u. Re-commenting: ctrl+k,c.
    /// How to setup memcached: http://www.rdlt.com/multiple-memcache-instancesservers-on-windows.html.
    /// </summary>
    /// <remarks>
    /// For debugging with verbose console output use:
    /// [path to memcached]\memcached.exe -p 11211 -m 128 -vv
    /// </remarks>
    [TestFixture, Category("Integration"), Ignore("Memcached must be running for this test fixture to work.")]
    public class MemcachedCacheImplementationIntegrationTests
    {
        private ICacheKeyCreator _stubKeyCreator;
        private ICacheImplementation _c;
        private TimeSpan _oneHour;
        private string _testString;  

        [TestFixtureSetUp]
        public void FixtureSetUp() { }

        [SetUp]
        public void Setup()
        {
            ICacheImplementation a = new AspDotNetDataCacheImplementation(Key.KeyCreator);
            a.RemoveAll();

            _stubKeyCreator = MockRepository.GenerateStub<ICacheKeyCreator>();
            _c = new MemcachedCacheImplementation(_stubKeyCreator);            
            _oneHour = new TimeSpan(1, 0, 0);
            _testString = "test";            
        }

        [TearDown]
        public void TearDown()
        {
            DistCache.RemoveAll();
        }

        [Test]
        public void Add_Absolute_Adds_An_Item_To_Memcached()
        {
            //arrange            
            _stubKeyCreator.Stub(x => x.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, _testString)).Return(Key.KeyCreator);            

            //act            
            Key key = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, _testString);
            bool success = _c.Add(key, new TestType("test-1"));


            //assert                        
            Assert.IsNotNull(DistCache.Get(key.ToString()));
            Assert.AreEqual((DistCache.Get(key.ToString()) as TestType).TestString, "test-1");
        }

        //[Test]
        //public void Add_Sliding_Adds_An_Item_To_Memcached()
        //{
        //    //arrange            
        //    _stubKeyCreator.Stub(x => x.Create(_c, StorageStyle.Unmodified, _testString)).Return("test:key");
        //    Assert.AreEqual(System.Web.HttpRuntime.Cache.Count, 0);

        //    //act            
        //    string key = _c.Add(new TestType("test-1"), _oneHour, StorageStyle.Unmodified, _testString);


        //    //assert                        
        //    Assert.AreEqual(_c.Count, 1);
        //    Assert.AreEqual((System.Web.HttpRuntime.Cache[key] as TestType).TestString, "test-1");
        //}

        //[Test]
        //public void Count_Returns_The_Number_Of_Items_In_The_Cache()
        //{
        //    //arrange
        //    Assert.AreEqual(System.Web.HttpRuntime.Cache.Count, 0);
        //    System.Web.HttpRuntime.Cache.Add("test1", new object(), null, _tomorrow, System.Web.Caching.Cache.NoSlidingExpiration,
        //                                     CacheItemPriority.Default, null);

        //    //assert            
        //    Assert.AreEqual(_c.Count, 1);

        //    System.Web.HttpRuntime.Cache.Add("test2", new object(), null, _tomorrow, System.Web.Caching.Cache.NoSlidingExpiration,
        //                                     CacheItemPriority.Default, null);

        //    //assert            
        //    Assert.AreEqual(_c.Count, 2);
        //}

        [Test]
        public void Get_Retrieves_An_Item_By_Key()
        {
            //arrange
            Assert.AreEqual(System.Web.HttpRuntime.Cache.Count, 0);
            Key key1 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "1");
            _c.Add(key1, new TestType("test-1"));
            Key key2 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "2");
            _c.Add(key2, new TestType("test-2"));
            Key key3 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "3");
            _c.Add(key3, new TestType("test-3"));

            //act                        
            var o = _c.Get(key2) as TestType;

            //assert            
            Assert.IsNotNull(o);
            Assert.AreEqual(o.TestString, "test-2");
        }

        [Test]
        public void Remove_Removes_An_Item_From_Memcached()
        {
            //arrange                        
            Assert.AreEqual(System.Web.HttpRuntime.Cache.Count, 0);
            Key key1 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "1");
            _c.Add(key1, new TestType("test-1"));
            Key key2 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "2");
            _c.Add(key2, new TestType("test-2"));
            Key key3 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "3");
            _c.Add(key3, new TestType("test-3"));            

            //act            
            _c.Remove(key2);

            //assert                                    
            Assert.IsNull(DistCache.Get(key2.ToString()));
        }

        [Test]
        public void RemoveAll_Empties_The_Cache()
        {
            //arrange            
            DistCache.Add("test1", new object(), _oneHour);
            DistCache.Add("test2", new object(), _oneHour);
            DistCache.Add("test3", new object(), _oneHour);
            var key1 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "1");
            _c.Add(key1, new TestType("test-1"));
            var key2 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "2");
            _c.Add(key2, new TestType("test-2"));
            var key3 = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "3");
            _c.Add(key3, new TestType("test-3"));
            Assert.IsNotNull(DistCache.Get("test1"));
            Assert.IsNotNull(DistCache.Get("test2"));
            Assert.IsNotNull(DistCache.Get(key3.ToString()));

            //act            
            _c.RemoveAll();

            //assert            
            Assert.IsNull(DistCache.Get("test1"));
            Assert.IsNull(DistCache.Get("test2"));
            Assert.IsNull(DistCache.Get(key3.ToString()));
        }              
    }
}
