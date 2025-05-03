using StorageService.Domain.errs;

namespace StorageService.Domain.rules;

public static class StorageRules
{
    private const int
        MinNameLength = 8,
        MaxNameLength = 60;

    public static ErrOrNothing CheckStorageNameForErrs(string? name) {
        int nameLength = name?.Length ?? 0;
        if (nameLength < MinNameLength || nameLength > MaxNameLength) {
            return ErrFactory.InvalidData(
                $"Incorrect storage name length. Storage name length must be between {MinNameLength} and {MaxNameLength} characters",
                $"Current name length: {nameLength}"
            );
        }

        return ErrOrNothing.Nothing;
    }
}