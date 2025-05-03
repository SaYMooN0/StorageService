namespace StorageService.Api.services;

public class PasswordHasher
{
    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyPassword(string hashedPassword, string passwordToCheck) =>
        BCrypt.Net.BCrypt.Verify(passwordToCheck, hashedPassword);
}