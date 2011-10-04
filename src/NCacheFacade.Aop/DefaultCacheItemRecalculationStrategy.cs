namespace NCacheFacade
{
    using System;
    using System.Threading;
    using PostSharp.Aspects;

    /// <summary>
    /// Responsible for providing a type to expose cache item 
    /// recalculation functionality.
    /// </summary>
    [Serializable]
    public class DefaultCacheItemRecalculationStrategy : ICacheItemRecalculationStrategy
    {
        public DefaultCacheItemRecalculationStrategy(TimeSpan timeGivenToPerformCalculation)
        {
            TimeBeforeExpiryToRunRoutine = timeGivenToPerformCalculation;
        }

        public void RunRoutine(Key key, MethodInterceptionArgs args, ICache cache, Mutex mutex, Action routine)
        {
            using (mutex) //avoid mutex being lost through garbage collection (unsure if required) see: odetocode.com/blogs/scott/archive/2004/08/20/the-misunderstood-mutex.aspx
            {
                for (;;)
                {
                    routine();
                    cache.Add(key, args.ReturnValue);
                    Thread.Sleep(key.DurationToStore - TimeBeforeExpiryToRunRoutine);
                }
            }
        }

        public TimeSpan TimeBeforeExpiryToRunRoutine { get; set; }
    }
}