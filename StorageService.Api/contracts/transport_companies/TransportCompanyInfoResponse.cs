using StorageService.Domain.entities.transport_company;

namespace StorageService.Api.contracts.transport_companies;

public record class TransportCompanyInfoResponse(
    string Id,
    string Name,
    DateTime CreatedAt
)
{
    public static TransportCompanyInfoResponse FromCompany(TransportCompany company) => new(
        company.Id.ToString(),
        company.Name,
        company.CreatedAt
    );
}