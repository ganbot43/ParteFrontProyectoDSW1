using Microsoft.Data.SqlClient;

namespace ParteFrontProyectoDSW1.Helpers
{
    public interface IDatabaseExecutor
    {
        Task<T> ExecuteCommand<T>(Func<SqlConnection, Task<T>> operation);
    }
}
