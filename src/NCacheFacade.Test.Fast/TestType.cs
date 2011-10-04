namespace NCacheFacade.Test.Fast
{
    using System;
    using System.Threading;

    /// <summary>
    /// Responsible fro providing functionality for the 
    /// testing of caching aspects.
    /// </summary>
    [Serializable]
    public class TestType
    {
        public TestType() {} //for serialization 

        public TestType(string testString)
        {
            TestString = testString;
        }        

        public string TestString { get; set; }

        private static object _counterLock = new object();
        private static int _counter;
        private static object _counterLock2 = new object();
        private static int _counter2;

        public static int Counter { get { return _counter; } }
        public static int Counter2 { get { return _counter2; } }

        [CacheAspect(typeof(MyCache), 
         typeof(ArgBasedFriendlyNameCreationStrategy))]
        public int MyMethod1()
        {
            return 1;
        }

        [CacheAspect(typeof(MyCache), 
         typeof(ArgBasedFriendlyNameCreationStrategy))]
        public int MyMethod2(int index, string value)
        {
            return 1;
        }

        [CacheAspect(typeof(TestCacheMissCache),
                     typeof(ArgBasedFriendlyNameCreationStrategy),
                     2,
                     StorageStyle.Unmodified,
                     ExpirationType.Sliding,
                     typeof(DefaultCacheItemRecalculationStrategy),
                     1)]
        public int MyMethod3()
        {
            lock (_counterLock)
            {
                _counter++;
                return _counter;
            }
        }

        [CacheAspect(typeof(TestCacheMissCache),
                     typeof(ArgBasedFriendlyNameCreationStrategy),
                     2,
                     StorageStyle.Unmodified,
                     ExpirationType.Sliding,
                     typeof(DefaultCacheItemRecalculationStrategy),
                     1)]
        public int MyMethod4()
        {
            Thread.Sleep(TimeSpan.FromSeconds(5)); //longer than the cache expiration period

            lock (_counterLock2)
            {
                _counter2++;
                return _counter2;
            }
        }
    }
}
