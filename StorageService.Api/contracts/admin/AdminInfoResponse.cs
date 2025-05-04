using StorageService.Domain.entities;
using StorageService.Domain.entities.storage;
using StorageService.Domain.entities.transport_company;

namespace StorageService.Api.contracts.admin;

internal record class AdminInfoResponse(
    string Id,
    string Name,
    DateTime RegistrationDate,
    AdminInfoResponseStorageData[] Storages,
    AdminInfoResponseTransportData[] TransportCompanies
)
{
    public static AdminInfoResponse FromAdmin(CompanyAdmin admin) => new(
        admin.Id.ToString(),
        admin.Name,
        admin.RegistrationDate,
        admin.Storages.Select(AdminInfoResponseStorageData.FromStorage).ToArray(),
        admin.TransportCompanies.Select(AdminInfoResponseTransportData.FromTransportCompany).ToArray()
    );
}

internal record AdminInfoResponseStorageData(string Id, string Name, DateTime CreatedAt)
{
    public static AdminInfoResponseStorageData FromStorage(Storage storage) => new(
        storage.Id.ToString(), storage.Name, storage.CreatedAt
    );
}

internal record AdminInfoResponseTransportData(string Id, string Name, DateTime CreatedAt)
{
    public static AdminInfoResponseTransportData FromTransportCompany(TransportCompany company) => new(
        company.Id.ToString(), company.Name, company.CreatedAt
    );
}
