using System.Collections.Immutable;
using StorageService.Domain.entities;
using StorageService.Domain.errs;

namespace StorageService.Domain.rules;

public static class ProductRules
{
    private const int
        MinNameLength = 3,
        MaxNameLength = 250,
        MaxDescriptionLength = 2000;

    private const int
        MinPropsKeyLength = 3,
        MaxPropsKeyLength = 120,
        MinPropsValueLength = 1,
        MaxPropsValueLength = 250;

    public static ErrOrNothing CheckNameForErrs(string? name) {
        int length = string.IsNullOrWhiteSpace(name) ? 0 : name.Length;
        if (length < MinNameLength || length > MaxNameLength) {
            return ErrFactory.InvalidData(
                $"Product name must be between {MinNameLength} and {MaxNameLength} characters",
                $"Current length: {length}"
            );
        }

        return ErrOrNothing.Nothing;
    }

    public static ErrOrNothing CheckDescriptionForErrs(string? description) {
        int length = string.IsNullOrEmpty(description) ? 0 : description.Length;
        if (length > MaxDescriptionLength) {
            return ErrFactory.InvalidData(
                $"Product description must not exceed {MaxDescriptionLength} characters",
                $"Current length: {length}"
            );
        }

        return ErrOrNothing.Nothing;
    }

    public static ErrOrNothing CheckPropsForErrs(IDictionary<string, string>? props) {
        if (props is null) {
            return ErrFactory.InvalidData("Product properties are not provided");
        }

        foreach (var (key, value) in props) {
            int length = string.IsNullOrWhiteSpace(key) ? 0 : key.Length;

            if (length < MinPropsKeyLength || length > MaxPropsKeyLength) {
                return ErrFactory.InvalidData(
                    $"Property key must be between {MinPropsKeyLength} and {MaxPropsKeyLength} characters",
                    $"Invalid key: '{key}' (length: {length})"
                );
            }

            length = string.IsNullOrWhiteSpace(value) ? 0 : value.Length;
            if (length < MinPropsValueLength || length > MaxPropsValueLength) {
                return ErrFactory.InvalidData(
                    $"Property value must be between {MinPropsValueLength} and {MaxPropsValueLength} characters",
                    $"Invalid value for key '{key}': (length: {length})"
                );
            }
        }

        return ErrOrNothing.Nothing;
    }
}