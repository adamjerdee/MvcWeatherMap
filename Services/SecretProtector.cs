using Microsoft.AspNetCore.DataProtection;

public interface ISecretProtector
{
    byte[] Protect(string plaintext);
    string Unprotect(byte[] cipher);
}

public sealed class SecretProtector : ISecretProtector
{
    private readonly IDataProtector _p;
    public SecretProtector(IDataProtectionProvider provider) =>
        _p = provider.CreateProtector("AppSecrets.v1");

    public byte[] Protect(string plaintext) =>
        _p.Protect(System.Text.Encoding.UTF8.GetBytes(plaintext));

    public string Unprotect(byte[] cipher) =>
        System.Text.Encoding.UTF8.GetString(_p.Unprotect(cipher));
}
