namespace NCacheFacade.Test.Fast
{
    using System;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// AAA: http://ayende.com/Wiki/(S(lxlrwqf20t0e1x4500w3pp45))/Rhino%2BMocks%2B3.5.ashx#Arrange,Act,Assert
    /// </summary>
    /// <remarks>
    /// Insertion unit tests incomplete as they will add 
    /// limited value given the available time.
    /// </remarks>
    [TestFixture]
    public class CacheTests
    {
        private ICache _c;        
        private ICachingStrategy _stubStrategy;
        private ICacheItemEncoder _stubEncoder;
        private TimeSpan _oneHour;

        [TestFixtureSetUp]
        public void FixtureSetUp() {}

        [SetUp]
        public void Setup()
        {
            _stubStrategy = MockRepository.GenerateStub<ICachingStrategy>();
            _stubEncoder = MockRepository.GenerateStub<ICacheItemEncoder>();            
            _c = new Cache(_stubStrategy, _stubEncoder);            
            _oneHour = new TimeSpan(1,0,0);
        }
                
        [Test]
        public void Constructor_Accepts_A_Strategy_And_An_Encoder()
        {            
            //assert
            Assert.IsNotNull(_c);                  
        }               

        [Test]
        public void Add_Throws_An_Exception_If_Supplied_With_A_Null_CacheItem()
        {
            //arrange   
            var key = Key.KeyCreator.Create(_oneHour, StorageStyle.Unmodified, ExpirationType.Absolute, "key-not-in-cache");

            //assert
            Assert.Throws<Bjma.Utility.FriendlyExceptions.ArgumentNullException>(() => _c.Add(key, null));
        }   

        [Test]
        public void RemoveAll_Invokes_The_RemoveAll_Method_On_The_Strategy_Object()
        {
            //act            
            _c.RemoveAll();

            //assert
            _stubStrategy.AssertWasCalled(x => x.RemoveAll());
        }              
    }           
}
