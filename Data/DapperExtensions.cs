using Dapper;
using System.Data;

public static class DapperExtensions
{
    public static Task<int> ExecuteProcAsync(this IDbConnection conn, string proc, object? args = null, IDbTransaction? tx = null, int timeout = 30) =>
        conn.ExecuteAsync(proc, args, tx, timeout, CommandType.StoredProcedure);

    public static Task<IEnumerable<T>> QueryProcAsync<T>(this IDbConnection conn, string proc, object? args = null, IDbTransaction? tx = null, int timeout = 30) =>
        conn.QueryAsync<T>(proc, args, tx, timeout, CommandType.StoredProcedure);
}
