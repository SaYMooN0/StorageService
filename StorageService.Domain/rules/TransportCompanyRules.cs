using StorageService.Domain.errs;

namespace StorageService.Domain.rules;

public class TransportCompanyRules
{
    private const int
        MinNameLength = 8,
        MaxNameLength = 60;

    public static ErrOrNothing CheckCompanyNameForErrs(string? name) {
        int nameLength = string.IsNullOrWhiteSpace(name) ? 0 : name.Length;

        if (nameLength < MinNameLength || nameLength > MaxNameLength) {
            return ErrFactory.InvalidData(
                $"Incorrect transport company name length. Company name length must be between {MinNameLength} and {MaxNameLength} characters",
                $"Current name length: {nameLength}"
            );
        }

        string nameToLower = name.ToLower();
        if (!nameToLower.Contains("trans") && !nameToLower.Contains("транс")) {
            return ErrFactory.IncorrectFormat(
                "Transport companymust have either 'trans' or 'транс' in its name"
            );
        }

        return ErrOrNothing.Nothing;
    }
}