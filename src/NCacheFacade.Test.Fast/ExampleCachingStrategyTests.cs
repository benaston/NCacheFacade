namespace NCacheFacade.Test.Fast
{
    using System;
    using System.Collections.Generic;

    namespace ExampleCachingStrategyTests
    {
        using Extensions;
        using NUnit.Framework;

        public class Given_the_type
        {
            [TestFixture, Category("Fast")]
            public class When_SelectCacheImplementation_is_invoked_with_a_duration_to_cache_of_zero
            {
                [Test]
                public void Then_an_exception_is_thrown()
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => ExampleCachingStrategy.SelectCacheImplementation(TimeSpan.Zero, StorageStyle.Unmodified, ExpirationType.Absolute, null));
                }
            }

            [TestFixture, Category("Fast")]
            public class When_SelectCacheImplementation_is_invoked_with_a_collection_of_caches_that_does_not_include_the_asp_dot_net_data_cache_and_memcached
            {
                [Test]
                public void Then_an_exception_is_thrown()
                {
                    Assert.Throws<MissingCacheImplementationArgumentException>(() => ExampleCachingStrategy.SelectCacheImplementation(10.Minutes(), StorageStyle.Unmodified, ExpirationType.Absolute, new Dictionary<string, ICacheImplementation> { { "key1", new FakeCacheImplementation() }, { "key2", new FakeCacheImplementation() } }));
                }
            }
        }

        /// <summary>
        /// Responsible for defining a fake caching strategy for use with unit tests.
        /// </summary>
        public class FakeCacheImplementation : ICacheImplementation
        {
            public bool IsEnabled
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public bool Add(Key key, object value)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public object Get(Key key)
            {
                throw new NotImplementedException();
            }

            public bool Supports(ExpirationType expirationType)
            {
                throw new NotImplementedException();
            }

            public void Remove(Key key)
            {
                throw new NotImplementedException();
            }

            public void RemoveAll()
            {
                throw new NotImplementedException();
            }
        }
    }
}