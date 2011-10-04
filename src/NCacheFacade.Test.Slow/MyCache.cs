namespace NCacheFacade.Test.Slow
{
    using System;

    [Serializable]
    public class MyCache : ICache
    {
        public bool Add(Key key, object value)
        {
            return true;
        }

        public int CountAll
        {
            get { throw new NotImplementedException(); }
        }

        public T Get<T>(Key key) where T : class
        {
            throw new NotImplementedException();
        }

        public object this[string key]
        {
            get { return 2; }
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

    [Serializable]
    public class TestCacheMissCache : ICache
    {
        public bool Add(Key key, object value)
        {
            return true;
        }

        public int CountAll
        {
            get { throw new NotImplementedException(); }
        }

        public T Get<T>(Key key) where T : class
        {
            throw new NotImplementedException();
        }

        public object this[string key]
        {
            get { return null; }
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