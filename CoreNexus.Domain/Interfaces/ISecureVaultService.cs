namespace CoreNexus.Domain.Interfaces;

/// <summary>
/// Define el contrato para el blindaje criptográfico del sistema.
/// Implementa una arquitectura de seguridad Zero-Trust mediante cifrado 
/// de extremo a extremo (E2E) con vectores de inicialización dinámicos.
/// </summary>
public interface ISecureVaultService
{
    /// <summary>
    /// Aplica múltiples capas de cifrado AES-256 a un texto plano, 
    /// generando un payload blindado único por cada ejecución.
    /// </summary>
    /// <param name="plainText">Contenido sensible original.</param>
    /// <returns>Cadena cifrada en formato Base64 con metadatos de descifrado incluidos.</returns>
    string Protect(string plainText);

    /// <summary>
    /// Revierte el proceso de blindaje utilizando las llaves maestras 
    /// y los metadatos extraídos del payload protegido.
    /// </summary>
    /// <param name="protectedText">Cadena cifrada en Base64.</param>
    /// <returns>Texto original en plano o string.Empty si la desencriptación falla.</returns>
    string Unprotect(string protectedText);
}