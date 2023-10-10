using CleanArchitectureTemplate.Domain.SeedWork;

namespace CleanArchitectureTemplate.Application.Models;

/// <summary>
/// Multi-Key Dictionary Class
/// </summary>
/// <typeparam name="TKey1">Key1Type</typeparam>
/// <typeparam name="TKey2">Key2 Type</typeparam>
/// <typeparam name="TValue">Value Type</typeparam>
public class MultiKeyDictionary<TKey1, TKey2, TValue> : MultiKeyDictionary<TKey1, TKey2, TKey2, TValue>
    where TKey1 : struct, IConvertible
    where TKey2 : struct, IConvertible
{
}

/// <summary>
/// Multi-Key Dictionary Class
/// </summary>
/// <typeparam name="TKey1">Key1Type</typeparam>
/// <typeparam name="TKey2">Key2 Type</typeparam>
/// <typeparam name="TKey3">Key2 Type</typeparam>
/// <typeparam name="TValue">Value Type</typeparam>
public class MultiKeyDictionary<TKey1, TKey2, TKey3, TValue>
    where TKey1 : struct, IConvertible
    where TKey2 : struct, IConvertible
    where TKey3 : struct, IConvertible
{
    #region Private Fields

    private readonly Dictionary<KeyModel<TKey1, TKey2, TKey3>, TValue> _multiKeyDictionary = new Dictionary<KeyModel<TKey1, TKey2, TKey3>, TValue>();
    private readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

    #endregion

    #region Properties / Indexers

    public TValue this[TKey1 key1]
    {
        get
        {
            return TryGetValue(key1, default, default, out var item) ? item : default;
        }
    }

    public TValue this[TKey1 key1, TKey2 key2]
    {
        get
        {
            return TryGetValue(key1, key2, default, out var item) ? item : default;
        }
    }

    public TValue this[TKey1 key1, TKey2 key2, TKey3 key3]
    {
        get
        {
            return TryGetValue(key1, key2, key3, out var item) ? item : default;
        }
    }

    public List<TValue> Values
    {
        get
        {
            _readerWriterLock.EnterReadLock();

            try
            {
                return _multiKeyDictionary.Values.ToList();
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }
    }

    public int Count
    {
        get
        {
            _readerWriterLock.EnterReadLock();

            try
            {
                return _multiKeyDictionary.Count;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }
    }

    #endregion

    #region Methods

    private bool TryGetValue(TKey1 key1, TKey2 key2, TKey3 key3, out TValue value)
    {
        _readerWriterLock.EnterReadLock();

        try
        {
            return _multiKeyDictionary.TryGetValue(new KeyModel<TKey1, TKey2, TKey3>(key1, key2, key3), out value);
        }
        finally
        {
            _readerWriterLock.ExitReadLock();
        }
    }

    #region Contains Key
    public bool ContainsKey(TKey1 key1)
    {
        return TryGetValue(key1, default, default, out _);
    }

    public bool ContainsKey(TKey1 key1, TKey2 key2)
    {
        return TryGetValue(key1, key2, default, out _);
    }

    public bool ContainsKey(TKey1 key1, TKey2 key2, TKey3 key3)
    {
        return TryGetValue(key1, key2, key3, out _);
    }
    #endregion

    #region Add
    public void Add(TKey1 key1, TValue value)
    {
        Add(key1, default, default, value);
    }

    public void Add(TKey1 key1, TKey2 key2, TValue value)
    {
        Add(key1, key2, default, value);
    }

    public void Add(TKey1 key1, TKey2 key2, TKey3 key3, TValue value)
    {
        _readerWriterLock.EnterWriteLock();

        try
        {
            var key = new KeyModel<TKey1, TKey2, TKey3>(key1, key2, key3);
            _multiKeyDictionary.Add(key, value);
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }
    }
    #endregion

    #region Remove
    public void Remove(TKey1 key1)
    {
        Remove(key1, default, default);
    }

    public void Remove(TKey1 key1, TKey2 key2)
    {
        Remove(key1, key2, default);
    }

    public void Remove(TKey1 key1, TKey2 key2, TKey3 key3)
    {
        _readerWriterLock.EnterWriteLock();

        try
        {
            var key = new KeyModel<TKey1, TKey2, TKey3>(key1, key2, key3);
            if (_multiKeyDictionary.ContainsKey(key))
            {
                _multiKeyDictionary.Remove(key);
            }
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }
    }
    #endregion

    public TValue[] CloneValues()
    {
        _readerWriterLock.EnterReadLock();

        try
        {
            var values = new TValue[_multiKeyDictionary.Values.Count];

            _multiKeyDictionary.Values.CopyTo(values, 0);

            return values;
        }
        finally
        {
            _readerWriterLock.ExitReadLock();
        }
    }

    public KeyModel<TKey1, TKey2, TKey3>[] CloneKeys()
    {
        _readerWriterLock.EnterReadLock();

        try
        {
            var values = new KeyModel<TKey1, TKey2, TKey3>[_multiKeyDictionary.Keys.Count];
            _multiKeyDictionary.Keys.CopyTo(values, 0);
            return values;
        }
        finally
        {
            _readerWriterLock.ExitReadLock();
        }
    }

    public void Clear()
    {
        _readerWriterLock.EnterWriteLock();

        try
        {
            _multiKeyDictionary.Clear();
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }
    }

    public IEnumerator<KeyValuePair<KeyModel<TKey1, TKey2, TKey3>, TValue>> GetEnumerator()
    {
        _readerWriterLock.EnterReadLock();

        try
        {
            return _multiKeyDictionary.GetEnumerator();
        }
        finally
        {
            _readerWriterLock.ExitReadLock();
        }
    }

    #endregion
}

/// <summary>
/// Model with 3 keys
/// </summary>
/// <typeparam name="TKey1"></typeparam>
/// <typeparam name="TKey2"></typeparam>
/// <typeparam name="TKey3"></typeparam>
public class KeyModel<TKey1, TKey2, TKey3> : ValueObject
    where TKey1 : struct, IConvertible
    where TKey2 : struct, IConvertible
    where TKey3 : struct, IConvertible
{
    public TKey1 Key1 { get; set; }
    public TKey2 Key2 { get; set; }
    public TKey3 Key3 { get; set; }

    public KeyModel(TKey1 key1, TKey2 key2, TKey3 key3)
    {
        Key1 = key1;
        Key2 = key2;
        Key3 = key3;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key1;
        yield return Key2;
        yield return Key3;
    }
}
