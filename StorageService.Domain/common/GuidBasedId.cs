namespace StorageService.Domain.common;

public abstract class GuidBasedId : ValueObject, IComparable
{
    public Guid Value { get; }
    protected GuidBasedId(Guid value) => Value = value;
    public override string ToString() => Value.ToString();

    public override IEnumerable<object> GetEqualityComponents() {
        yield return Value;
    }

    public int CompareTo(object? obj) => obj switch {
        GuidBasedId ed => Value.CompareTo(ed.Value),
        Guid guid => guid.CompareTo(Value),
        _ => -1
    };
}