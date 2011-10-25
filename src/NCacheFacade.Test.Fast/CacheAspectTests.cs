// ReSharper disable InconsistentNaming
namespace NCacheFacade.Test.Fast
{
    using System;
    using System.Threading;

    namespace CacheAspectTests
    {
        using NUnit.Framework;


        public class Given_a_method_decorated_with_a_cache_aspect
        {
            [TestFixture, Category("Fast")]
            public class When_the_method_is_invoked
            {
                [Test]
                public void Then_the_return_value_is_from_the_cache_when_the_key_is_present()
                {
                    //arrange / act
                    var result1 = new TestType().MyMethod1();

                    //assert
                    Assert.That(result1 == 2);

                    //arrange / act
                    var result2 = new TestType().MyMethod2(3, "test string");

                    //assert
                    Assert.That(result2 == 2);
                }
            }

            [TestFixture, Category("Fast")]
            public class When_a_cache_item_recalculation_strategy_is_supplied
            {
                [Test]
                public void Then_the_cache_item_is_automatically_recalculated_according_to_the_strategy()
                {
                    //arrange / act
                    var instance = new TestType();
                    var result = instance.MyMethod3();

                    //assert
                    Assert.That(result == 1);

                    Thread.Sleep(TimeSpan.FromSeconds(5)); //wait for auto recalculation to be performed                                                        

                    //assert
                    Assert.That(TestType.Counter > 1); //the method has been invoked automatically behind the scenes a number of times by a worker thread
                }                
            }

            [TestFixture, Category("Fast")]
            public class When_the_recalculation_takes_longer_than_the_time_an_item_is_cached
            {
                [Test]
                public void Then_the_result_is_recalculated_as_if_no_caching_was_in_place() //TODO: BA; error should also be logged
                {
                    //arrange / act
                    var instance = new TestType();
                    var result = instance.MyMethod4();

                    //assert
                    Assert.That(result == 1);

                    //arrange / act
                    instance.MyMethod4();

                    //assert
                    Assert.That(TestType.Counter2 == 3); //the counter has been manually incremented once for each of the direct invocations, with one automatic calculation ocurring in the meantime
                }
            }
        }
    }
}
// ReSharper restore InconsistentNaming