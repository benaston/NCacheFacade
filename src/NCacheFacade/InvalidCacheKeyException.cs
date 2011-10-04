namespace NCacheFacade
{
    using System;

    public class InvalidCacheKeyException : Exception
    {
        private readonly string _key;

        public InvalidCacheKeyException(string key)
        {
            if (key == null) key = String.Empty;
            _key = key;
        }

        public override string Message
        {
            get
            {
                return
                    "Unable to parse the supplied cache key string. Are you using hand-crafted keys? The supplied key string was: " +
                    _key;
            }
        }
    }
}