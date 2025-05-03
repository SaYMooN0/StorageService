using StorageService.Domain.errs;

namespace StorageService.Domain.rules;

public class CompanyAdminRules
{
    private const int
        MinNameLength = 10,
        MaxNameLength = 120;

    private const int
        MinPasswordLength = 6,
        MaxPasswordLength = 50;

    public static ErrOrNothing CheckAdminNameForErr(string? name) {
        int nameLength = name?.Length ?? 0;
        if (nameLength < MinNameLength || nameLength > MaxNameLength) {
            return ErrFactory.InvalidData(
                $"Incorrect name length. Name length must be between {MinNameLength} and {MaxNameLength} characters",
                $"Current name length: {nameLength}"
            );
        }

        return ErrOrNothing.Nothing;
    }
    public static ErrOrNothing CheckPasswordForErr(string password) {
        int passwordLength = string.IsNullOrEmpty(password) ? 0 : password.Length;
        if (passwordLength < MinPasswordLength || passwordLength > MaxPasswordLength)
            return ErrFactory.IncorrectFormat( "Password must be between 8 and 20 characters");

        if (!password.Any(char.IsLetter)) {
            return ErrFactory.IncorrectFormat("Password must contain at least one letter");
        }

        if (!password.Any(char.IsDigit)) {
            return ErrFactory.IncorrectFormat("Password must contain at least one number");
        }

        return ErrOrNothing.Nothing;
    }
}