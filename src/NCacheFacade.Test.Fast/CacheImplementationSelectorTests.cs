namespace NCacheFacade.Test.Fast
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class CacheImplementationSelectorTests
    {
        private CacheImplementationSelectorDelegate _stubCacheImplementationSelectorDelegate;
        private ICacheImplementation _stubICacheImplementation;        
         
        [TestFixtureSetUp]
        public void FixtureSetUp() {}

        [SetUp]
        public void Setup()
        {            
            _stubCacheImplementationSelectorDelegate = MockRepository.GenerateStub<CacheImplementationSelectorDelegate>();
            _stubICacheImplementation = MockRepository.GenerateStub<ICacheImplementation>();            
        }

        //constructor
        [Test]
        public void Constructor_Accepts_A_Collection_Of_ICacheImplementations_And_A_CacheImplementationSelectorDelegate()
        {
            //arrange
            _stubICacheImplementation.IsEnabled = true;
            var l = new List<ICacheImplementation> { _stubICacheImplementation };

            //act
            var s = new CacheImplementationSelector(l, _stubCacheImplementationSelectorDelegate);

            //assert
            Assert.IsNotNull(s);
        }
        
        [Test]
        public void Constructor_Throws_An_Exception_When_ImplementationCollection_Is_Null()
        {
            //assert
            Assert.Throws<Exception>(() => new CacheImplementationSelector(null, _stubCacheImplementationSelectorDelegate));
        }

        [Test]
        public void Constructor_Throws_An_Exception_When_Selector_Delegate_Is_Null()
        {
            //arrange            
            var l = new List<ICacheImplementation>() { _stubICacheImplementation };

            //assert
            Assert.Throws<Exception>(() => new CacheImplementationSelector(l, null));
        }

        [Test]
        public void ConstructorThrowsAnExceptionWhenNoneOfTheSuppliedImplementationsAreEnabled()
        {
            //arrange            
             _stubICacheImplementation.IsEnabled = false;
            var l = new List<ICacheImplementation>() { _stubICacheImplementation };

            //assert
            Assert.Throws<ArgumentException>(() => new CacheImplementationSelector(l, _stubCacheImplementationSelectorDelegate));
        }

        [Test]
        public void ConstructorThrowsExceptionWhenNeitherCacheIsEnabled()
        {
            ICacheImplementation a = new AspDotNetDataCacheImplementation(Key.KeyCreator);
            a.IsEnabled = false;
            ICacheImplementation b = new MemcachedCacheImplementation(Key.KeyCreator);
            b.IsEnabled = false;
            var l = new List<ICacheImplementation>() { a, b };

            //assert            
            Assert.Throws(typeof(ArgumentException),
                          () => new CacheImplementationSelector(l, (ExampleCachingStrategy.SelectCacheImplementation)));
        }
    }
}
