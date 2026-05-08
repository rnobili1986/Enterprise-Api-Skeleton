namespace CoreNexus.Domain.Interfaces;

/// <summary>
/// Define las capacidades criptográficas del núcleo del sistema.
/// Permite el blindaje de payloads sensibles para comunicación E2E.
/// </summary>
public interface ISecureVaultService
{
    string Protect(string plainText);
    string Unprotect(string protectedText);
}
