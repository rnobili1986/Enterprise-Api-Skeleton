namespace CoreNexus.Domain.Interfaces;

public interface ITokenService
{
    // Definimos qué necesitamos para el token. 
    // Podríamos pedir más datos, pero para este inicio con Username y Role basta.
    string GenerateToken(string username, string role);
}