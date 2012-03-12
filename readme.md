NCacheFacade
=====

A simple, flexible cache facade for .NET.

Usage:
--------

```C#

	
	var cache = ResolveType.Of<ICache>(); 	
	//optional storage style indicates compression and/or encryption
	var key = new Key(5.Minutes(), StorageStyle.Whatever, ExpirationType.Whatever, "unique-name");
	MyType cachedObject;
	
	if (cache[key.ToString()] == null)
	{
		cachedObject = new MyObject();
		bool success = cache.Add(key, cachedObject);
	}
	else
	{
		cachedObject = cache.Get<MyObject>(key);
	}
	
	//...make use of the object
	
```

License & Copyright
=====

This software is released under the GNU Lesser GPL. It is Copyright 2012, Ben Aston. I may be contacted at ben@bj.ma.