using StorageService.Domain.common;

namespace StorageService.Domain.entities;

public class TransportCompany : Entity<TransportCompanyId>
{
    private TransportCompany() { }
    private string Name { get; }

    private TransportCompany(TransportCompanyId id, string name) {
        Id = id;
        Name = name;
    }
}