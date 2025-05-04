using System.Collections.Immutable;
using StorageService.Domain.common;
using StorageService.Domain.errs;
using StorageService.Domain.rules;

namespace StorageService.Domain.entities;

public class Product : Entity<ProductId>
{
    private Product() { }
    public string Name { get; }
    public string? Description { get; }

    public ImmutableDictionary<string, string> Props { get; }

    //Dictionary потому что характеристика товара различается у всех
    //у ноутбука есть продолжительность работы от батареи
    //а у проводной мышки количество нажатий которое можно сдеалть перед тем как она сойдет с ума
    private Product(ProductId id, string name, string? description, ImmutableDictionary<string, string> props) {
        Id = id;
        Name = name;
        Description = description;
        Props = props;
    }

    public static ErrOr<Product> CreateNew(
        string name,
        string? description,
        ImmutableDictionary<string, string> props
    ) {     
        if (ProductRules.CheckNameForErrs(name).IsErr(out var err)) {
            return err;
        }

        if (ProductRules.CheckDescriptionForErrs(description).IsErr(out err)) {
            return err;
        }

        if (ProductRules.CheckPropsForErrs(props).IsErr(out err)) {
            return err;
        }

        return new Product(ProductId.CreateNew(), name, description, props);
    }
}