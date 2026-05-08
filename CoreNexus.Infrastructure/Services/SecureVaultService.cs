using CoreNexus.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace CoreNexus.Infrastructure.Services;

public class SecureVaultService : ISecureVaultService
{
    private readonly byte[] _key1;
    private readonly byte[] _iv1;
    private readonly byte[] _aesKey;
    private readonly byte[] _aesIv;

    public SecureVaultService(IConfiguration config)
    {
        // Recuperamos las llaves desde la configuración
        _key1 = StringToByteArray(config["SecureVault:Key1"]);
        _iv1 = StringToByteArray(config["SecureVault:IV1"]);
        _aesKey = Convert.FromBase64String(config["SecureVault:AesKey"]);
        _aesIv = Convert.FromBase64String(config["SecureVault:AesIV"]);
    }

    public string Protect(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;

        // Capa 1: Encriptación Primaria
        string firstLayer = EncryptFirstLayer(plainText);

        // Capa 2: Doble Blindaje con AES
        return EncryptSecondLayer(firstLayer);
    }

    public string Unprotect(string protectedText)
    {
        // Desencriptamos en orden inverso
        string firstLayerDecrypted = DecryptSecondLayer(protectedText);
        return DecryptFirstLayer(firstLayerDecrypted);
    }

    // --- MÉTODOS PRIVADOS ---

    private string EncryptFirstLayer(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key1;
        aes.IV = _iv1;

        using var encryptor = aes.CreateEncryptor();
        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = transform(inputBytes, encryptor);

        return Convert.ToBase64String(encryptedBytes);
    }

    private string EncryptSecondLayer(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = _aesIv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = transform(inputBytes, encryptor);

        return Convert.ToBase64String(encryptedBytes);
    }

    // Métodos de apoyo para evitar repetición de código (DRY)
    private byte[] transform(byte[] input, ICryptoTransform cryptoTransform)
    {
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);
        cs.Write(input, 0, input.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    private string DecryptFirstLayer(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = _key1;
        aes.IV = _iv1;
        using var decryptor = aes.CreateDecryptor();
        byte[] input = Convert.FromBase64String(cipherText);
        byte[] output = transform(input, decryptor);
        return Encoding.UTF8.GetString(output);
    }

    private string DecryptSecondLayer(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = _aesIv;
        using var decryptor = aes.CreateDecryptor();
        byte[] input = Convert.FromBase64String(cipherText);
        byte[] output = transform(input, decryptor);
        return Encoding.UTF8.GetString(output);
    }

    private byte[] StringToByteArray(string hex) =>
        Enumerable.Range(0, hex.Length / 2).Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)).ToArray();
}
