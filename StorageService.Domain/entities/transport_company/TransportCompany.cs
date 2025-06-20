﻿using StorageService.Domain.common;
using StorageService.Domain.errs;
using StorageService.Domain.rules;

namespace StorageService.Domain.entities.transport_company;

public class TransportCompany : Entity<TransportCompanyId>
{
    private TransportCompany() { }
    public CompanyAdminId AdminId { get; }
    public string Name { get;private set; }
    public DateTime CreatedAt { get; }
    private readonly List<TransportationRecord> _transportations ;

    private TransportCompany(TransportCompanyId id, CompanyAdminId adminId, string name, DateTime createdAt) {
        Id = id;
        Name = name;
        AdminId = adminId;
        CreatedAt = createdAt;
        _transportations = [];
    }
    public static ErrOr<TransportCompany> CreateNew(string name, CompanyAdminId adminId) {
        if (TransportCompanyRules.CheckCompanyNameForErrs(name).IsErr(out var err)) {
            return err;
        }

        return new TransportCompany(TransportCompanyId.CreateNew(), adminId, name, DateTime.UtcNow);
    }

    public ErrOrNothing Rename(string newName) {
        if (TransportCompanyRules.CheckCompanyNameForErrs(newName).IsErr(out var err)) {
            return err;
        }
        Name = newName;
        return ErrOrNothing.Nothing;
    }
}