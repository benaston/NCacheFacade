namespace NCacheFacade.AspNetDataCache
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Defines facade for the ASP.NET data cache.
    /// </summary>
    public class AspDotNetDataCacheImplementation : ICacheImplementation
    {
        private readonly ICacheKeyCreator _keyCreator;

        private readonly List<ExpirationType> _supportedCacheExpirationTypes = new List<ExpirationType>
                                                                                   {
                                                                                       ExpirationType.Absolute,
                                                                                       ExpirationType.Sliding
                                                                                   };

        public AspDotNetDataCacheImplementation(ICacheKeyCreator keyCreator)
        {
            Ensure.That(keyCreator.IsNotNull(), "keyCreator not supplied");

            _keyCreator = keyCreator;
            (this as ICacheImplementation).IsEnabled = true;
                //hard-coded for time being, could be read in from config file
        }

        #region ICacheImplementation Members

        bool ICacheImplementation.IsEnabled { get; set; }

        /// <summary>
        ///   StorageStyle needed for the key creation.
        ///   Use of Insert method enables overwriting.
        /// </summary>
        bool ICacheImplementation.Add(Key key, object value)
        {
            Ensure.That(key.IsNotNull(), "key not supplied.").And(value.IsNotNull(), "value not supplied.");

            var valueToReturn = false;

            try
            {
                if (key.CacheExpirationType == ExpirationType.Absolute)
                {
                    var absoluteExpiration = DateTime.Now.Add(key.DurationToStore);
                    HttpRuntime.Cache.Insert(key.ToString(), value, null, absoluteExpiration,
                                             System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default,
                                             null);
                }
                else if (key.CacheExpirationType == ExpirationType.Sliding)
                {
                    HttpRuntime.Cache.Insert(key.ToString(), value, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                                             key.DurationToStore, CacheItemPriority.Default, null);
                }
                else
                    throw new Exception("Unrecognised CacheExpirationType.");


                valueToReturn = true;
            }
            catch (Exception e)
            {
                e.Log();
            }

            return valueToReturn;
        }

        int ICacheImplementation.Count
        {
            get { return HttpRuntime.Cache.Count; }
        }

        /// <summary>
        ///   Weakly typed as may be compressed and or encrypted.
        /// </summary>
        object ICacheImplementation.Get(Key key)
        {
            Ensure.That(key.IsNotNull(), "key not supplied");

            return HttpRuntime.Cache[key.ToString()];
        }

        void ICacheImplementation.Remove(Key key)
        {
            Ensure.That(key.IsNotNull(), "key not supplied");

            HttpRuntime.Cache.Remove(key.ToString());
        }

        /// <summary>
        ///   Copy keys into separate list as enumerator only remains 
        ///   valid as long as collection it is working on remains 
        ///   unchanged.
        /// </summary>
        void ICacheImplementation.RemoveAll()
        {
            var keys = new List<string>();
            var enumerator = HttpRuntime.Cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }

            foreach (var key in keys)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }

        bool ICacheImplementation.Supports(ExpirationType expirationType)
        {
            return _supportedCacheExpirationTypes.Contains(expirationType);
        }

        #endregion
    }
}