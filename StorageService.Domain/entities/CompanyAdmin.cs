using StorageService.Domain.common;
using StorageService.Domain.entities.storage;
using StorageService.Domain.errs;
using StorageService.Domain.rules;

namespace StorageService.Domain.entities;

public class CompanyAdmin : Entity<CompanyAdminId>
{
    private CompanyAdmin() { }
    public string Name { get; }
    public string PasswordHash { get; }
    public DateTime RegistrationDate { get; }
    private readonly List<Storage> _storages;
    private readonly List<TransportCompany> _transportCompanies;

    private CompanyAdmin(CompanyAdminId id, string name, string passwordHash, DateTime registrationDate) {
        Id = id;
        Name = name;
        PasswordHash = passwordHash;
        _storages = [];
        _transportCompanies = [];
        RegistrationDate = registrationDate;
    }

    public static ErrOr<CompanyAdmin> CreateNew(string name, string passwordHash) {
        if (CompanyAdminRules.CheckAdminNameForErr(name).IsErr(out var err)) {
            return err;
        }

        return new CompanyAdmin(CompanyAdminId.CreateNew(), name, passwordHash, DateTime.UtcNow);
    }

    //to create new copy so the _storages is immutable outside of CompanyAdmin
    public IReadOnlyList<Storage> Storages => _storages.ToList();
    public IReadOnlyList<TransportCompany> TransportCompanies => _transportCompanies.ToList();    //same

    public ErrOr<Storage> CreateStorage(string storageName) {
        var res = Storage.CreateNew(storageName, this.Id);
        if (res.IsErr(out var err)) {
            return err;
        }

        _storages.Add(res.AsSuccess());
        return res.AsSuccess();
    }
}