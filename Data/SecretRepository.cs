using Dapper;
using System.Data;

public interface ISecretRepository
{
    Task SetAsync(string name, byte[] encrypted);
    Task<byte[]?> GetAsync(string name);
}

public sealed class SecretRepository(ISqlConnectionFactory factory) : ISecretRepository
{
    public async Task SetAsync(string name, byte[] encrypted)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync();
        await conn.ExecuteProcAsync("dbo.AppSecrets_Set", new { Name = name, EncryptedValue = encrypted });
    }

    public async Task<byte[]?> GetAsync(string name)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync();

        // Dapper will map a single VARBINARY(MAX) column directly to byte[]
        var result = await conn.QuerySingleOrDefaultAsync<byte[]>(
            "dbo.AppSecrets_Get",
            new { Name = name },
            commandType: CommandType.StoredProcedure);

        return result;
    }
}