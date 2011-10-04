NCacheFacade
=====

A simple, flexible cache façade for .NET.

Usage:
--------

```C#


	var cache = ResolveType.Of<ICache>(); //suggest making this an instance variable
    var key = new Key(5.Minutes(), ExpirationType.Whatever, "unique-name-for-the-cache-item");
    //-- or -- 
    //var key = new Key(5.Minutes(), StorageStyle.Whatever, ExpirationType.Whatever, "unique-name-for-the-cache-item");
    T cachedObject;
    
    if (cache[key.ToString()] == null)
    {
        cachedObject = new T();
        bool success = cache.Add(key, cachedObject);
    }
    else
    {
        cachedObject = cache.Get<T>(key);
    }    
	
```