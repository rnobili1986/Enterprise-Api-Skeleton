using CoreNexus.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace CoreNexus.Infrastructure.Services;

public class SecureVaultService : ISecureVaultService
{
    private readonly byte[] _key1;
    private readonly byte[] _aesKey;

    public SecureVaultService(IConfiguration config)
    {
        // Solo recuperamos las llaves. Los IVs ahora se generan dinámicamente por cada mensaje.
        _key1 = StringToByteArray(config["SecureVault:Key1"] ?? throw new InvalidOperationException("Key1 is missing in configuration."));
        _aesKey = Convert.FromBase64String(config["SecureVault:AesKey"] ?? throw new InvalidOperationException("AesKey is missing in configuration."));
    }

    public string Protect(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;

        byte[] data = Encoding.UTF8.GetBytes(plainText);

        // Capa 1: Encriptación Primaria con IV dinámico
        byte[] firstLayer = EncryptLayer(data, _key1);

        // Capa 2: Doble Blindaje con AES e IV dinámico independiente
        byte[] secondLayer = EncryptLayer(firstLayer, _aesKey);

        return Convert.ToBase64String(secondLayer);
    }

    public string Unprotect(string protectedText)
    {
        if (string.IsNullOrEmpty(protectedText)) return string.Empty;

        try
        {
            byte[] data = Convert.FromBase64String(protectedText);

            // Desencriptamos en orden inverso: primero la capa exterior (segunda llave)
            byte[] secondLayerDecrypted = DecryptLayer(data, _aesKey);

            // Luego la capa interior (primera llave)
            byte[] originalData = DecryptLayer(secondLayerDecrypted, _key1);

            return Encoding.UTF8.GetString(originalData);
        }
        catch (CryptographicException)
        {
            // En un sistema real, aquí loguearías un intento de manipulación o error de llave
            return string.Empty;
        }
    }

    // --- MÉTODOS PRIVADOS DE PROCESAMIENTO ---

    private byte[] EncryptLayer(byte[] input, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        // aes.GenerateIV() se llama automáticamente al crear el encryptor si no se asigna uno.

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();

        // Guardamos el IV al principio del stream para que el receptor pueda usarlo
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(input, 0, input.Length);
            cs.FlushFinalBlock();
        }

        return ms.ToArray();
    }

    private byte[] DecryptLayer(byte[] input, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        int ivSize = aes.BlockSize / 8; // 16 bytes para AES

        if (input.Length < ivSize) throw new CryptographicException("Invalid cipher text.");

        // Extraemos el IV de los primeros 16 bytes
        byte[] iv = new byte[ivSize];
        Array.Copy(input, 0, iv, 0, ivSize);
        aes.IV = iv;

        // Extraemos el contenido cifrado (el resto del array)
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

    private byte[] StringToByteArray(string hex)
    {
        if (hex.Length % 2 != 0) throw new ArgumentException("Invalid Hex Key");
        return Enumerable.Range(0, hex.Length / 2)
                         .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                         .ToArray();
    }
}