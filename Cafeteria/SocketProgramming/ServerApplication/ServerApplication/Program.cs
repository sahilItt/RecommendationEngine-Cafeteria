using ServerApplication.Models;
using ServerApplication.Services;

namespace ServerApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            string? connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ModuleDB", EnvironmentVariableTarget.User);
            DbHandler dbHandler = new DbHandler(connectionString);
            Authentication authentication = new Authentication(dbHandler); 

            Server server = new Server("127.0.0.1", 12345, authentication, dbHandler);
            server.Start();
        }
    }
}
