namespace StorageService.Domain.common;

public class CompanyAdminId(Guid value) : GuidBasedId(value)
{
    public static CompanyAdminId CreateNew() => new(Guid.CreateVersion7());
}

public class StorageId(Guid value) : GuidBasedId(value)
{
    public static StorageId CreateNew() => new(Guid.CreateVersion7());
}

public class TransportCompanyId(Guid value) : GuidBasedId(value)
{
    public static TransportCompanyId CreateNew() => new(Guid.CreateVersion7());
}
public class ProductId(Guid value) : GuidBasedId(value)
{
    public static ProductId CreateNew() => new(Guid.CreateVersion7());
}
public class ProductCountChangedRecordId(Guid value) : GuidBasedId(value)
{
    public static ProductCountChangedRecordId CreateNew() => new(Guid.CreateVersion7());
}public class ProductRecordId(Guid value) : GuidBasedId(value)
{
    public static ProductRecordId CreateNew() => new(Guid.CreateVersion7());
}