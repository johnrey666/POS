namespace POSSystem.Domain.Models;

public class AuthResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }
    public AuthenticatedUser? User { get; }

    private AuthResult(bool success, string? errorMessage, AuthenticatedUser? user)
    {
        Success = success;
        ErrorMessage = errorMessage;
        User = user;
    }

    public static AuthResult Succeeded(AuthenticatedUser user) 
        => new(true, null, user);

    public static AuthResult Failed(string errorMessage) 
        => new(false, errorMessage, null);
}