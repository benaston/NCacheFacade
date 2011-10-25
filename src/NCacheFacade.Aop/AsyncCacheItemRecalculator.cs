namespace NCacheFacade.Aop
{
    using System;
    using System.Threading;
    using NBasicExtensionMethod;
    using NSure;
    using PostSharp.Aspects;

    //   0            0      //not in cache, no mutex; start thread
    //   1            0      //in cache, no mutex; assume thread already started? (could be in another process)
    //   0            1      //not in cache, mutex found; thread started, do not start thread - log error
    //   1            1      //in cache, mutex found; thread started, do not start thread
    /// <summary>
    ///   Responsible ensuring recalculation strategies 
    ///   are started asynchronously.
    /// </summary>
    /// <remarks>
    ///   NOTE: the simplistic check to see if the cache item 
    ///   exists is designed to enable the use of out of process
    ///   caching subsystems. This mechanism limits us to
    ///   use interprocess mutex to limit calculation of *shared* 
    ///   cache item. ASP.NET data cache must be calculated process 
    ///   locally. Possible need a boolean check to determine if 
    ///   the cache in use is inter-process, or process-local
    ///   See: http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c
    ///   in-cache | mutex-found
    /// </remarks>
    [Serializable]
    public static class AsyncCacheItemRecalculator
    {
        // unique id for global mutex - Global prefix means it is global to the machine
        private const string MutexPrefix = "Global\\";

        public static void EnsureIsStarted(Key key,
                                           ICacheItemRecalculationStrategy recalculationStrategy,
                                           MethodInterceptionArgs args,
                                           ICache cache)
        {
            var keyString = key.ToString();
            Ensure.That<NHelpfulException.FrameworkExceptions.ArgumentException>(keyString.Length.IsLessThanOrEqualTo(260),
                                                              "key must be less than 260 characters long.");

            try
            {
                bool createdNew;
                var mutex = new Mutex(false, MutexPrefix + keyString, out createdNew);
                if (createdNew)
                {
                    if (cache[keyString].IsNotNull())
                        return;
                    //item already in cache, assume thread already started TODO: possibly log this as an error

                    ThreadPool.QueueUserWorkItem(
                        o => recalculationStrategy.RunRoutine(key, args, cache, mutex, args.Proceed));

                    //NOTE: mutex.ReleaseMutex(); is not called because the mutex is only expected to be released upon closure of the application                                                  
                }
            }
            catch (Exception)
            {
                //log exception
                throw;
            }
        }
    }
}