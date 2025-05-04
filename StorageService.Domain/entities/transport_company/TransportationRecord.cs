using System.Collections.Immutable;
using StorageService.Domain.common;

namespace StorageService.Domain.entities.transport_company;

public class TransportationRecord : Entity<TransportationRecordId>
{
    private TransportationRecord() { }

    public TransportCompanyId TransportCompanyId { get; }
    public StorageId SourceStorageId { get; }
    public StorageId DestinationStorageId { get; }
    public DateTime TransportedAt { get; }

    public ImmutableDictionary<ProductId, uint> ProductsWithCount;

    private TransportationRecord(
        TransportationRecordId id,
        TransportCompanyId companyId,
        StorageId source,
        StorageId destination,
        ImmutableDictionary<ProductId, uint> products,
        DateTime transportedAt
    ) {
        Id = id;
        TransportCompanyId = companyId;
        SourceStorageId = source;
        DestinationStorageId = destination;
        ProductsWithCount = products;
        TransportedAt = transportedAt;
    }

    public static TransportationRecord CreateNew(
        TransportCompanyId companyId,
        StorageId source,
        StorageId destination,
        IDictionary<ProductId, uint> products
    ) => new(
        TransportationRecordId.CreateNew(), companyId,
        source, destination,
        products.ToImmutableDictionary(), DateTime.UtcNow
    );
}