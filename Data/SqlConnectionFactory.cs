using Microsoft.Data.SqlClient;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}

public sealed class SqlConnectionFactory(IConfiguration cfg) : ISqlConnectionFactory
{
    private readonly string _cs = cfg.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");
    public SqlConnection Create() => new SqlConnection(_cs);
}
