namespace NCacheFacade.Test.Fast
{
    using System;
    using Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class CacheKeyTests
    {
        [TestFixtureSetUp]
        public void FixtureSetUp() {}

        [SetUp]
        public void Setup() {}

        [Test]
        public void Constructor_Throws_ArgumentException_With_Null_Key()
        {
            //assert            
            Assert.Throws<Exception>(() => new Key(null));
        }

        [Test]
        public void Constructor_Throws_ArgumentException_With_Too_Long_Key()
        {
            //assert            
            Assert.Throws<Exception>(() => new Key("a".Repeat(Key.MaxKeyLength + 1)));                    
        }

        [Test]
        public void Can_Be_Instantiated_And_Subsequently_Decoded_With_A_Key_String()
        {
            //arrange              
            var key = new Key("5000:0:0:AAEAAAD/////AQAAAAAAAAAGAQAAAA1mcmllbmRseS1uYW1lCw==");
            
            //assert
            Assert.IsNotNull(key);
            Assert.AreEqual(key.DurationToStore, TimeSpan.FromSeconds(5000));
            Assert.AreEqual(key.StorageStyle, StorageStyle.Unmodified);
            Assert.AreEqual(key.CacheExpirationType, ExpirationType.Absolute);
            Assert.AreEqual(key.IsItemCompressed, false);
            Assert.AreEqual(key.IsItemEncrypted, false);
            Assert.AreEqual(key.FriendlyName, "friendly-name");
        }

        [Test]
        public void Can_Be_Instantiated_And_Subsequently_Decoded_With_A_Key_String_Marking_The_Item_As_Compressed_And_Encrypted()
        {
            //arrange              
            var key = new Key("6000:3:1:AAEAAAD/////AQAAAAAAAAAGAQAAAA9mcmllbmRseS1uYW1lLTIL");

            //assert
            Assert.IsNotNull(key);
            Assert.AreEqual(key.DurationToStore, TimeSpan.FromSeconds(6000));
            Assert.AreEqual(key.StorageStyle, StorageStyle.CompressedAndEncrypted);
            Assert.AreEqual(key.CacheExpirationType, ExpirationType.Sliding);
            Assert.AreEqual(key.IsItemCompressed, true);
            Assert.AreEqual(key.IsItemEncrypted, true);
            Assert.AreEqual(key.FriendlyName, "friendly-name-2");
        }

        [Test]
        public void Create_Instantiates_A_New_Key()
        {
            //arrange              
            var key = Key.KeyCreator.Create(TimeSpan.FromSeconds(60), StorageStyle.Compressed, ExpirationType.Absolute, "friendly-name");

            //assert
            Assert.IsNotNull(key);
            Assert.AreEqual(key.DurationToStore, TimeSpan.FromSeconds(60));
            Assert.AreEqual(key.StorageStyle, StorageStyle.Compressed);
            Assert.AreEqual(key.CacheExpirationType, ExpirationType.Absolute);
            Assert.AreEqual(key.IsItemCompressed, true);
            Assert.AreEqual(key.IsItemEncrypted, false);
            Assert.AreEqual(key.FriendlyName, "friendly-name");
        }

        [Test]
        public void ToString_Is_Overloaded_To_Enable_Round_Trip_Creation()
        {
            //arrange             
            var key = Key.KeyCreator.Create(TimeSpan.FromSeconds(60), 
                                            StorageStyle.Compressed, 
                                            ExpirationType.Absolute, 
                                            "friendly-name");

            //assert
            Assert.IsNotNull(key);
            Assert.AreEqual(key.DurationToStore, TimeSpan.FromSeconds(60));
            Assert.AreEqual(key.StorageStyle, StorageStyle.Compressed);
            Assert.AreEqual(key.CacheExpirationType, ExpirationType.Absolute);
            Assert.AreEqual(key.IsItemCompressed, true);
            Assert.AreEqual(key.IsItemEncrypted, false);
            Assert.AreEqual(key.FriendlyName, "friendly-name");

            string k = key.ToString();

            Key newKey = new Key(k);

            Assert.IsNotNull(newKey);
            Assert.AreEqual(newKey.DurationToStore, TimeSpan.FromSeconds(60));
            Assert.AreEqual(newKey.StorageStyle, StorageStyle.Compressed);
            Assert.AreEqual(newKey.CacheExpirationType, ExpirationType.Absolute);
            Assert.AreEqual(newKey.IsItemCompressed, true);
            Assert.AreEqual(newKey.IsItemEncrypted, false);
            Assert.AreEqual(newKey.FriendlyName, "friendly-name");
        }
    }
}
