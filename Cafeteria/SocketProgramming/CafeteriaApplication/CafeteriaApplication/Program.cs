using CafeteriaApplication.Services;

namespace CafeteriaApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client application started...");

            Client client = new Client();
            client.Run();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}