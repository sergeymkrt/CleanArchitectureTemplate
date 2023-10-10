namespace CleanArchitectureTemplate.Application.DTOs;

public class MultiKeyValuesDto<TKey1, TKey2, TValue> : MultiKeyValuesDto<TKey1, TKey2, TKey2, TValue>
    where TKey1 : struct, IConvertible
    where TKey2 : struct, IConvertible
{
    #region Methods
    public override bool Equals(object obj)
    {
        return obj.GetType() == GetType() && GetHashCode() == (obj as MultiKeyValuesDto<TKey1, TKey2, TValue>).GetHashCode();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Key1, Key2);
    }
    #endregion Methods
}

public class MultiKeyValuesDto<TKey1, TKey2, TKey3, TValue>
    where TKey1 : struct, IConvertible
    where TKey2 : struct, IConvertible
    where TKey3 : struct, IConvertible
{
    #region Constructors
    public MultiKeyValuesDto()
    {
        Values = new List<TValue>();
    }

    public MultiKeyValuesDto(TKey1 key1, TKey2? key2, TKey3? key3, IEnumerable<TValue> values)
    {
        Key1 = key1;
        Key2 = key2;
        Key3 = key3;
        Values = values;
    }
    #endregion Constructors

    #region Properties
    public TKey1 Key1 { get; set; }
    public TKey2? Key2 { get; set; }
    public TKey3? Key3 { get; set; }
    public IEnumerable<TValue> Values { get; set; }
    #endregion Properties

    #region Methods
    public override bool Equals(object obj)
    {
        return obj.GetType() == GetType() && GetHashCode() == (obj as MultiKeyValuesDto<TKey1, TKey2, TKey3, TValue>).GetHashCode();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Key1, Key2, Key3);
    }
    #endregion Methods
}
