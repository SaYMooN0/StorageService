using System.Text;
using System.Text.Json.Serialization;

namespace StorageService.Domain.errs;


public class Err
{
    public string Message { get; init; }
    public ushort Code { get; init; }
    public string? Details { get; init; }

    public Err(
        string message,
        ushort code = ErrCodes.Unspecified,
        string? details = null
    ) {
        Message = message;
        Code = code;
        Details = details;
    }

    public override string ToString() {
        var sb = new StringBuilder();
        if (Code != ErrCodes.Unspecified) {
            sb.AppendLine($"Code: {Code}");
        }

        sb.AppendLine($"Message: {Message}");

        if (!string.IsNullOrEmpty(Details)) {
            sb.AppendLine($"Details: {Details}");
        }

        return sb.ToString();
    }
    public static class ErrCodes
    {
        public const ushort Unspecified = 0;

        // Validation
        public const ushort InvalidData = 1101;
        public const ushort NoValue = 1102;
        public const ushort IncorrectFormat = 1103;
        public const ushort ValueOutOfRange = 1104;

        // Business logic
        public const ushort Conflict = 1201;
        public const ushort NotFound = 1204;

        // Auth
        public const ushort AuthRequired = 1301;
        public const ushort NoAccess = 1302;
        public const ushort Forbidden = 1303;
        
        
    }
}