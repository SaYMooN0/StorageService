using StorageService.Domain.entities.transport_company;

namespace StorageService.Api.contracts.transport_companies;

public record TransportationRecordDataResponse(
    string Id,
    string SourceStorageId,
    string DestinationStorageId,
    Dictionary<string, int> Products,
    DateTime TransportedAt
)
{
    public static TransportationRecordDataResponse FromEntity(TransportationRecord record) => new(
        record.Id.ToString(),
        record.SourceStorageId.ToString(),
        record.DestinationStorageId.ToString(),
        record.ProductsWithCount.ToDictionary(
            p => p.Key.Value.ToString(),
            p => (int)p.Value
        ),
        record.TransportedAt
    );
}