using Microsoft.Data.SqlClient;

namespace apiModeloExamen.Helpers
{
    public interface IDatabaseExecutor
    {
        Task<T> ExecuteCommand<T>(Func<SqlConnection, Task<T>> operation);
    }
}
