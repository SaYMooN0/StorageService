using Microsoft.EntityFrameworkCore;
using StorageService.Domain.entities;
using StorageService.Domain.entities.storage;
using StorageService.Domain.entities.transport_company;

namespace StorageService.Infrastructure.persistence;

public static class DbSetsExtensions
{
    public static IQueryable<CompanyAdmin> WithTransportCompanies(this IQueryable<CompanyAdmin> query) =>
        query.Include(p => EF.Property<List<TransportCompany>>(p, "_transportCompanies"));
    public static IQueryable<CompanyAdmin> WithStorages(this IQueryable<CompanyAdmin> query) =>
        query.Include(p => EF.Property<List<Storage>>(p, "_storages"));

    public static IQueryable<Storage> WithProductRecords(this IQueryable<Storage> query) =>
        query.Include(s => EF.Property<List<ProductRecord>>(s, "_products"));

    public static IQueryable<Storage> WithProductCountHistory(this IQueryable<Storage> query) =>
        query.Include(s => EF.Property<List<ProductCountChangedRecord>>(s, "_productCountChangedHistory"));

    public static IQueryable<TransportCompany> WithTransportations(this IQueryable<TransportCompany> query) =>
        query.Include(c => EF.Property<List<TransportationRecord>>(c, "_transportations"));
}