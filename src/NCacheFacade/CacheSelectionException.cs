namespace NCacheFacade
{
    using System;
    using System.Collections.Generic;

    public class CacheSelectionException : Exception
    {
        private readonly Dictionary<string, ICacheImplementation> _cacheImplementations;

        public CacheSelectionException(Dictionary<string, ICacheImplementation> cacheImplementations)
        {
            _cacheImplementations = cacheImplementations;
        }

        public override string Message
        {
            get
            {
                var message =
                    "Unable to find a CacheImplementation that meets the requirements of the caller. Are the relevant CacheImplementations enabled? The supplied CacheImplementations were: ";
                var currentPosition = 0;

                foreach (var i in _cacheImplementations)
                {
                    message += i.Key;
                    if (currentPosition != _cacheImplementations.Count - 1)
                        message += ", ";
                    else
                        message += ".";

                    currentPosition++;
                }

                return message;
            }
        }
    }
}