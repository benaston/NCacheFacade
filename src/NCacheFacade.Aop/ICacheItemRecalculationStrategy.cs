namespace NCacheFacade.Aop
{
    using System;
    using System.Threading;
    using PostSharp.Aspects;

    /// <summary>
    /// Responsible for defining the interface for types 
    /// providing the ability to recalculate cache items.
    /// </summary>
    public interface ICacheItemRecalculationStrategy
    {
        /// <summary>
        /// Responsible for the execution of the supplied routine.
        /// </summary>
        /// <param name="key">The key of the item to recalculate.</param>
        /// <param name="args">Any additional parameters required for the successful running of the cache item recalculation routine.</param>
        /// <param name="cache">The cache to store the item in once calculated.</param>
        /// <param name="mutex">An *inter-process* mutex for the synchronization of the invocation of the rountine.</param>
        /// <param name="routine">A function continaing logic for the re-calculation of the cache item and the insertion into the cache.</param>
        void RunRoutine(Key key, MethodInterceptionArgs args, ICache cache, Mutex mutex, Action routine);
        /// <summary>
        /// The amount of time before scheduled cache item expiration
        /// for the recalculation routine to be re-instantiated.
        /// </summary>
        TimeSpan TimeBeforeExpiryToRunRoutine { get; set; }
    }
}