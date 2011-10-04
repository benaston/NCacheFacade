namespace NCacheFacade
{
    using System;
    using NHelpfulException;

    public class MissingCacheImplementationArgumentException : HelpfulException
    {
        public MissingCacheImplementationArgumentException(string message) : base(message) {}

        public MissingCacheImplementationArgumentException(string message, Exception innerException)
            : base(message, innerException: innerException) {}
    }
}