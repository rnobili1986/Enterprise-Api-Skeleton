namespace CoreNexus.Application.DTOs;
public record AuthResponse(
    string Token,
    string EncryptedWelcomeMessage,
    DateTime Expiration
);