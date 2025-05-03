namespace StorageService.Domain.common;

public class StorageAdminId(Guid value) : GuidBasedId(value)
{
    public static StorageAdminId CreateNew() => new(Guid.CreateVersion7());
}

public class StorageId(Guid value) : GuidBasedId(value)
{
    public static StorageId CreateNew() => new(Guid.CreateVersion7());
}

public class TransportCompanyId(Guid value) : GuidBasedId(value)
{
    public static TransportCompanyId CreateNew() => new(Guid.CreateVersion7());
}
public class TransportRouteId(Guid value) : GuidBasedId(value)
{
    public static TransportRouteId CreateNew() => new(Guid.CreateVersion7());
}
public class ProductId(Guid value) : GuidBasedId(value)
{
    public static TransportRouteId CreateNew() => new(Guid.CreateVersion7());
}