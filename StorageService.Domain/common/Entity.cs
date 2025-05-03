namespace StorageService.Domain.common;

public abstract class Entity<IdType> where IdType : GuidBasedId
{
    public IdType Id { get; protected init; }
    protected Entity() { }

    public override bool Equals(object? other) {
        if (other is null || other.GetType() != GetType()) {
            return false;
        }

        Entity<IdType> otherEntity = (Entity<IdType>)other;
        return Id.Equals(otherEntity.Id);
    }

    public override int GetHashCode() =>
        Id.GetHashCode();
}