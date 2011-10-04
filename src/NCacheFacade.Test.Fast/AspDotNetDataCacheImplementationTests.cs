namespace NCacheFacade.Test.Fast
{
    using System;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class AspDotNetDataCacheImplementationTests
    {
        private ICacheKeyCreator _stubKeyCreator;
        private ICacheImplementation _c;
        private TimeSpan _oneHour;   

        [TestFixtureSetUp]
        public void FixtureSetUp() {}

        [SetUp]
        public void Setup()
        {
            ICacheImplementation a = new AspDotNetDataCacheImplementation(Key.KeyCreator);
            a.RemoveAll();

            _stubKeyCreator = MockRepository.GenerateStub<ICacheKeyCreator>();
            _c = new AspDotNetDataCacheImplementation(_stubKeyCreator);            
            _oneHour = new TimeSpan(1,0,0);            
        }

        [TearDown]
        public void TearDown() {}

        //constructor
        [Test]
        public void Can_Be_Instantiated_With_A_Key_Creator()
        {
            //assert
            Assert.IsNotNull(_c);
        }

        [Test]
        public void Constructor_Throws_An_Exception_If_Supplied_With_A_Null_Key_Creator()
        {
            //assert
            Assert.Throws<Exception>(() => new AspDotNetDataCacheImplementation(null));
        }

        //IsEnabled
        [Test]
        public void IsEnabled_Defaults_To_True()
        {            
            //assert
            Assert.IsTrue(_c.IsEnabled);
        }

        [Test]
        public void IsEnabled_Permits_Getting_And_Setting_Of_The_Enabled_State()
        {
            //arrange
            Assert.IsTrue(_c.IsEnabled);
            _c.IsEnabled = false;
            
            //assert
            Assert.IsFalse(_c.IsEnabled);
        }
        
        [Test]
        public void Add_Throws_An_Exception_If_Supplied_With_A_Null_CacheKey()
        {
            //assert
            Assert.Throws<Exception>(() => _c.Add(null, new object()));
        }

        [Test]
        public void Add_Throws_AnvException_If_Supplied_With_A_Null_CacheItem()
        {
            //arrange
            Key key = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "key-not-in-cache");

            //assert
            Assert.Throws<Exception>(() => _c.Add(key, null));
        }                                
               
        //get
        [Test]
        public void Get_Throws_An_Exception_If_Supplied_With_A_Null_CacheKey()
        {
            //assert
            Assert.Throws<Exception>(() => _c.Get(null));
        }        

        [Test]
        public void Get_Returns_Null_If_Supplied_With_A_Key_Not_In_The_Cache()
        {
            //arrange            
            Key key = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "key-not-in-cache");

            //act
            object o = _c.Get(key);            

            //assert
            Assert.IsNull(o);            
        }

        [Test]
        public void Remove_Does_Not_Throw_An_Exception_When_Invoed_On_A_Key_Not_In_The_Cache()
        {
            //arrange            
            Key key = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "key-not-in-cache");

            //act
            Assert.DoesNotThrow(() => _c.Remove(key));            
        }                           
    }    
}
