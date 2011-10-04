namespace NCacheFacade.Test.Slow
{
    using System;
    using MemcachedProviders.Cache;
    using NUnit.Framework;

    [TestFixture]
    public class CacheImplementationSelectorIntegrationTests
    {
        private CacheImplementationSelector _c;
        private TimeSpan _longTermBoundary;
        private TimeSpan _shortTimeSpan;

        [TestFixtureSetUp]
        public void FixtureSetUp() {}

        [SetUp]
        public void Setup()
        {
            (new AspDotNetDataCacheImplementation(Key.KeyCreator) as ICacheImplementation).RemoveAll();
            DistCache.RemoveAll();

            _c = new CacheImplementationSelector(new System.Collections.Generic.List<ICacheImplementation>
                                                     {
                                                         new AspDotNetDataCacheImplementation(Key.KeyCreator),
                                                         new MemcachedCacheImplementation(Key.KeyCreator)
                                                     },
                                                     (ExampleCachingStrategy.SelectCacheImplementation));
                                    
            _longTermBoundary = ExampleCachingStrategy.LongTermBoundary;
            _shortTimeSpan = _longTermBoundary.Subtract(TimeSpan.FromSeconds(5)); //arbitrary choice of timespan
        }

        [TearDown]
        public void TearDown()
        {
            (new AspDotNetDataCacheImplementation(Key.KeyCreator) as ICacheImplementation).RemoveAll();            
            DistCache.RemoveAll();
        }

        [Test]
        public void Select_Chooses_AspDotNet_For_Normal_Short_Term_Storage()
        {
            //act
            ICacheImplementation i = _c.Select(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Absolute);

            //assert
            Assert.IsNotNull(i);
            Assert.AreEqual(i.GetType().FullName, typeof(AspDotNetDataCacheImplementation).FullName);
        }

        [Test]
        public void Select_Chooses_Memcached_For_Normal_Long_Term_Storage()
        {
            //act
            ICacheImplementation i = _c.Select(_longTermBoundary, StorageStyle.Unmodified, ExpirationType.Absolute);

            //assert
            Assert.IsNotNull(i);
            Assert.AreEqual(i.GetType().FullName, typeof(MemcachedCacheImplementation).FullName);
        }

        [Test]
        public void Select_Falls_Back_To_Memcached_For_Short_Term_Storage_When_AspDotNet_Is_Disabled()
        {
            //arrange
            _c.CacheImplementations[typeof(AspDotNetDataCacheImplementation).FullName].IsEnabled = false;

            //act
            ICacheImplementation i = _c.Select(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Absolute);

            //assert
            Assert.IsNotNull(i);
            Assert.AreEqual(i.GetType().FullName, typeof(MemcachedCacheImplementation).FullName);
        }

        [Test]
        public void Select_Throws_CacheSelectionException_When_AspDotNet_Is_Disabled_And_Desired_Caching_Style_Is_Unsupported_By_Memcached()
        {
            //arrange
            _c.CacheImplementations[typeof(AspDotNetDataCacheImplementation).FullName].IsEnabled = false;

            //assert            
            Assert.Throws(typeof(CacheSelectionException),
                          () => _c.Select(_shortTimeSpan, StorageStyle.Unmodified, ExpirationType.Sliding));
        }

        [Test]
        public void Select_Throws_Cache_Selection_Exception_When_Both_Caches_Are_Disabled()
        {
            //arrange
            _c.CacheImplementations[typeof(AspDotNetDataCacheImplementation).FullName].IsEnabled = false;
            _c.CacheImplementations[typeof(MemcachedCacheImplementation).FullName].IsEnabled = false;

            //assert            
            Assert.Throws(typeof(CacheSelectionException),
                          () => _c.Select(_shortTimeSpan, StorageStyle.Unmodified,
                                          ExpirationType.Sliding));
        }

        [Test]
        public void Select_Falls_Back_To_AspDotNet_For_Long_Term_Storage_When_Memcached_Is_Disabled()
        {
            //arrange
            _c.CacheImplementations[typeof(MemcachedCacheImplementation).FullName].IsEnabled = false;

            //act
            ICacheImplementation i = _c.Select(_longTermBoundary, StorageStyle.Unmodified,
                                               ExpirationType.Absolute);

            //assert
            Assert.IsNotNull(i);
            Assert.AreEqual(i.GetType().FullName, typeof(AspDotNetDataCacheImplementation).FullName);
        }                  
    }
}
