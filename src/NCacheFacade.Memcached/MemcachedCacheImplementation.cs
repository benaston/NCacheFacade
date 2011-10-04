namespace NCacheFacade
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using FluentAssertions;
    using Logging;
    using MemcachedProviders.Cache;
    using NBasicExtensionMethod;
    using NSure;

    /// <summary>
    ///   Defines facade for the Memcached cache.
    ///   http://sourceforge.net/projects/memcacheddotnet/
    ///   http://www.codeplex.com/memcachedproviders
    ///   http://allegiance.chi-town.com/MemCacheDManager.aspx
    /// </summary>
    /// <remarks>
    ///   To install memcached as a service used the daemon (-d)
    ///   option: sc create  "Memcached11211" binPath= "[path to memcached]\memcached.exe -d runservice -p 11211 -m 128"  DisplayName= "Memcached11211" start= auto
    ///   For debugging with verbose console output use:
    ///   [path to memcached]\memcached.exe -p 11211 -m 128 -vv
    /// </remarks>
    public class MemcachedCacheImplementation : ICacheImplementation
    {
        private readonly ICacheKeyCreator _keyCreator;

        private readonly List<ExpirationType> _supportedCacheExpirationTypes = new List<ExpirationType>
                                                                                   {ExpirationType.Absolute};

        public MemcachedCacheImplementation(ICacheKeyCreator keyCreator)
        {
            Ensure.That(keyCreator.IsNotNull(), "keyCreator not supplied");

            _keyCreator = keyCreator;
            (this as ICacheImplementation).IsEnabled = true;
            //hard-coded for time being, could be read in from config file            
        }

        #region ICacheImplementation Members

        bool ICacheImplementation.IsEnabled { get; set; }

        /// <summary>
        ///   Confused re exception thrown here that is not propagated.            
        ///   TODO: BA; exception logging.
        /// </summary>
        bool ICacheImplementation.Add(Key key, object value)
        {
            Ensure.That(key.IsNotNull(), "key not supplied").And(value.IsNotNull(), "value not supplied");

            var success = false;

            try
            {
                if (key.CacheExpirationType == ExpirationType.Absolute)
                {
                    success = DistCache.Add(key.ToString(), value, key.DurationToStore);
                }
                else
                    throw new NotSupportedException("Sliding expirations are not supported by Memcached.");
            }
            catch (Exception e)
            {
                e.Log();
            }

            return success;
        }

        //bool ICacheImplementation.Insert(CacheKey key, object value)
        //{
        //    Check.Argument.IsNotNull(key, "key");
        //    Check.Argument.IsNotNull(value, "value");            

        //    var success = false;

        //    if (key.CacheExpirationType == CacheExpirationType.AbsoluteExpiration)
        //    {
        //        success = DistCache.Add(key.ToString(), value, key.DurationToStore);
        //    }
        //    else
        //    {
        //        throw new NotSupportedException("Sliding expirations are not supported by Memcached.");
        //    }

        //    return success ? key : String.Empty;
        //}   

        /// <summary>
        ///   Not implemented on memcached provider.
        /// </summary>
        int ICacheImplementation.Count
        {
            get { return 0; }
        }


        object ICacheImplementation.Get(Key key)
        {
            Ensure.That(key.IsNotNull(), "key not supplied");

            return DistCache.Get(key.ToString());
        }

        void ICacheImplementation.Remove(Key key)
        {
            Ensure.That(key.IsNotNull(), "key not supplied");

            DistCache.Remove(key.ToString());
        }

        void ICacheImplementation.RemoveAll()
        {
            DistCache.RemoveAll();
        }

        bool ICacheImplementation.Supports(ExpirationType expirationType)
        {
            return _supportedCacheExpirationTypes.Contains(expirationType);
        }

        #endregion
    }
}