using CoreNexus.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace CoreNexus.Infrastructure.Services;

/// <summary>
/// Servicio encargado de proteger y recuperar información sensible
/// mediante un esquema de cifrado por capas utilizando AES.
/// </summary>
/// <remarks>
/// El servicio aplica doble cifrado utilizando claves independientes
/// y vectores de inicialización (IV) dinámicos para cada operación.
/// </remarks>
public class SecureVaultService : ISecureVaultService
{
    /// <summary>
    /// Clave utilizada para la primera capa de cifrado.
    /// </summary>
    private readonly byte[] _key1;

    /// <summary>
    /// Clave utilizada para la segunda capa de cifrado AES.
    /// </summary>
    private readonly byte[] _aesKey;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de cifrado.
    /// </summary>
    /// <param name="config">
    /// Configuración de la aplicación desde donde se obtienen las claves de seguridad.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando alguna clave requerida no existe en la configuración.
    /// </exception>
    public SecureVaultService(IConfiguration config)
    {
        // Las claves son obtenidas desde configuración segura.
        // Los IVs se generan dinámicamente para cada operación de cifrado.
        _key1 = StringToByteArray(
            config["SecureVault:Key1"]
            ?? throw new InvalidOperationException("Key1 is missing in configuration."));

        _aesKey = Convert.FromBase64String(
            config["SecureVault:AesKey"]
            ?? throw new InvalidOperationException("AesKey is missing in configuration."));
    }

    /// <summary>
    /// Protege un texto plano aplicando doble cifrado AES.
    /// </summary>
    /// <param name="plainText">Texto a proteger.</param>
    /// <returns>
    /// Cadena cifrada en formato Base64.
    /// </returns>
    public string Protect(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        byte[] data = Encoding.UTF8.GetBytes(plainText);

        // Primera capa de cifrado con IV dinámico.
        byte[] firstLayer = EncryptLayer(data, _key1);

        // Segunda capa de cifrado independiente.
        byte[] secondLayer = EncryptLayer(firstLayer, _aesKey);

        return Convert.ToBase64String(secondLayer);
    }

    /// <summary>
    /// Recupera y descifra un texto previamente protegido.
    /// </summary>
    /// <param name="protectedText">
    /// Contenido cifrado en formato Base64.
    /// </param>
    /// <returns>
    /// Texto original descifrado o una cadena vacía si el contenido
    /// es inválido o no puede ser descifrado.
    /// </returns>
    public string Unprotect(string protectedText)
    {
        if (string.IsNullOrEmpty(protectedText))
            return string.Empty;

        try
        {
            byte[] data = Convert.FromBase64String(protectedText);

            // Se descifra primero la capa exterior.
            byte[] secondLayerDecrypted = DecryptLayer(data, _aesKey);

            // Luego se descifra la capa interior.
            byte[] originalData = DecryptLayer(secondLayerDecrypted, _key1);

            return Encoding.UTF8.GetString(originalData);
        }
        catch (CryptographicException)
        {
            // En producción debería registrarse el intento
            // de manipulación o error criptográfico.
            return string.Empty;
        }
    }

    // --------------------------------------------------------------------
    // MÉTODOS PRIVADOS
    // --------------------------------------------------------------------

    /// <summary>
    /// Aplica una capa de cifrado AES utilizando un IV dinámico.
    /// </summary>
    /// <param name="input">Datos a cifrar.</param>
    /// <param name="key">Clave de cifrado.</param>
    /// <returns>
    /// Datos cifrados con el IV incluido al inicio del resultado.
    /// </returns>
    private byte[] EncryptLayer(byte[] input, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;

        // El IV se genera automáticamente al crear la instancia AES.
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream();

        // El IV se almacena al inicio del stream para poder
        // reconstruir el proceso de descifrado posteriormente.
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(input, 0, input.Length);
            cs.FlushFinalBlock();
        }

        return ms.ToArray();
    }

    /// <summary>
    /// Descifra una capa de datos cifrados con AES.
    /// </summary>
    /// <param name="input">
    /// Datos cifrados que contienen el IV al inicio.
    /// </param>
    /// <param name="key">Clave utilizada para descifrar.</param>
    /// <returns>
    /// Datos originales descifrados.
    /// </returns>
    /// <exception cref="CryptographicException">
    /// Se lanza cuando el contenido cifrado es inválido.
    /// </exception>
    private byte[] DecryptLayer(byte[] input, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;

        // AES utiliza bloques de 128 bits (16 bytes).
        int ivSize = aes.BlockSize / 8;

        if (input.Length < ivSize)
            throw new CryptographicException("Invalid cipher text.");

        // Se extrae el IV almacenado al inicio del mensaje.
        byte[] iv = new byte[ivSize];
        Array.Copy(input, 0, iv, 0, ivSize);

        aes.IV = iv;

        // Se obtiene el contenido cifrado restante.
        int cipherTextSize = input.Length - ivSize;

        byte[] cipherText = new byte[cipherTextSize];
        Array.Copy(input, ivSize, cipherText, 0, cipherTextSize);

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream();

        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
        {
            cs.Write(cipherText, 0, cipherText.Length);
            cs.FlushFinalBlock();
        }

        return ms.ToArray();
    }

    /// <summary>
    /// Convierte una cadena hexadecimal en un arreglo de bytes.
    /// </summary>
    /// <param name="hex">Cadena hexadecimal.</param>
    /// <returns>Arreglo de bytes equivalente.</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza cuando la cadena hexadecimal es inválida.
    /// </exception>
    private byte[] StringToByteArray(string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Invalid Hex Key");

        return Enumerable.Range(0, hex.Length / 2)
                         .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                         .ToArray();
    }
}