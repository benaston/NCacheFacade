namespace NCacheFacade.Test.Fast
{
    using System;
    using System.Threading;
    using PostSharp.Aspects;

    internal class TestRecalculationStrategy : ICacheItemRecalculationStrategy
    {
        public void RunRoutine(Key key, MethodInterceptionArgs args, ICache cache, Mutex mutex, Action routine)
        {
            throw new NotImplementedException();
        }

        public TimeSpan TimeBeforeExpiryToRunRoutine
        {
            get { return TimeSpan.FromSeconds(10); }
            set { /*do nothing*/ }
        }        
    }
}