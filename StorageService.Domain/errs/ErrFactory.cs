namespace StorageService.Domain.errs;

public static class ErrFactory
    {
        public static Err Unspecified(string message = "Unspecified error", string? details = null) =>
            new(message, Err.ErrCodes.Unspecified, details);

        // Validation
        public static Err InvalidData(string message = "Invalid data", string? details = null) =>
            new(message, Err.ErrCodes.InvalidData, details);

        public static Err NoValue(string message = "Missing required value", string? details = null) =>
            new(message, Err.ErrCodes.NoValue, details);

        public static Err IncorrectFormat(string message = "Incorrect format", string? details = null) =>
            new(message, Err.ErrCodes.IncorrectFormat, details);

        public static Err ValueOutOfRange(string message = "Value is out of allowed range", string? details = null) =>
            new(message, Err.ErrCodes.ValueOutOfRange, details);

        // Business logic
        public static Err Conflict(string message = "Conflict occurred", string? details = null) =>
            new(message, Err.ErrCodes.Conflict, details);

        public static Err NotFound(string message = "Not found", string? details = null) =>
            new(message, Err.ErrCodes.NotFound, details);

        // Auth
        public static Err AuthRequired(string message = "Authentication required", string? details = null) =>
            new(message, Err.ErrCodes.AuthRequired, details);

        public static Err NoAccess(string message = "Access is denied", string? details = null) =>
            new(message, Err.ErrCodes.NoAccess, details);

        public static Err Forbidden(string message = "Forbidden", string? details = null) =>
            new(message, Err.ErrCodes.Forbidden, details);
    }