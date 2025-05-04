using StorageService.Domain.common;
using StorageService.Domain.entities.storage;
using StorageService.Domain.entities.transport_company;
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
    public IReadOnlyList<TransportCompany> TransportCompanies => _transportCompanies.ToList(); //same

    public ErrOr<Storage> CreateStorage(string storageName) {
        var res = Storage.CreateNew(storageName, this.Id);
        if (res.IsErr(out var err)) {
            return err;
        }

        if (_storages.Any(s => s.Name == storageName)) {
            return ErrFactory.Conflict("You cannot have more than one storage with the same name");
        }

        _storages.Add(res.AsSuccess());
        return res.AsSuccess();
    }

    public ErrOr<TransportCompany> CreateTransportCompany(string companyName) {
        var res = TransportCompany.CreateNew(companyName, this.Id);
        if (res.IsErr(out var err)) {
            return err;
        }

        if (_transportCompanies.Any(s => s.Name == companyName)) {
            return ErrFactory.Conflict("You cannot have more than one company with the same name");
        }

        _transportCompanies.Add(res.AsSuccess());
        return res.AsSuccess();
    }

    public ErrOr<string> RenameTransportCompany(TransportCompanyId transportCompanyId, string newName) {
        var companyToRename = _transportCompanies.FirstOrDefault(t => t.Name == newName);
        if (companyToRename is null) {
            return ErrFactory.NotFound("Company not fount");
        }

        if (TransportCompanyRules.CheckCompanyNameForErrs(newName).IsErr(out var err)) {
            return err;
        }

        if (companyToRename.Name == newName) {
            return ErrFactory.Unspecified("New name must differ from the old one");
        }

        if (_transportCompanies.Any(s => s.Name == newName && s.Id != transportCompanyId)) {
            return ErrFactory.Conflict("You cannot have more than one company with the same name");
        }

        var renameRes = companyToRename.Rename(newName);
        if (renameRes.IsErr(out err)) {
            return err;
        }

        return companyToRename.Name;
    }
}