using System.Net.Sockets;
using System.Text.Json;
using CafeteriaApplication.Controller;
using CafeteriaApplication.Models;

namespace CafeteriaApplication.Services
{
    public class Client
    {
        private const string ServerAddress = "127.0.0.1";
        private const int ServerPort = 12345;

        private User currentUser;
        private AdminController adminMenu;
        private ChefController chefMenu;
        private EmployeeController employeeMenu;

        public void Run()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Enter your email:");
                    string? email = Console.ReadLine();

                    Console.WriteLine("Enter your password:");
                    string? password = Console.ReadLine();

                    LoginRequest request = new LoginRequest { Email = email, Password = password };
                    string jsonRequest = JsonSerializer.Serialize(request);

                    using (TcpClient client = new TcpClient(ServerAddress, ServerPort))
                    using (NetworkStream stream = client.GetStream())
                    using (StreamReader reader = new StreamReader(stream))
                    using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                    {
                        writer.WriteLine(jsonRequest);

                        string? jsonResponse = reader.ReadLine();
                        var response = JsonSerializer.Deserialize<LoginResponse>(jsonResponse);

                        if (response.IsAuthenticated)
                        {
                            Console.WriteLine(response.Message);
                            currentUser = new User { Email = email, Role = response.Role };
                            ShowMenuForRole(response.Role, writer, reader, currentUser);
                        }
                        else
                        {
                            Console.WriteLine(response.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ShowMenuForRole(string role, StreamWriter writer, StreamReader reader, User currentUser)
        {
            adminMenu = new(writer, reader);
            chefMenu = new(writer, reader);
            employeeMenu = new(writer, reader);

            Console.WriteLine();
            if (role != null)
            {
                Console.WriteLine($"Logged in as {role}");

                if (role == "admin")
                {
                    adminMenu.ShowAdminMenu();
                }
                else if (role == "chef")
                {
                    chefMenu.ShowChefMenu();
                }
                else if (role == "employee")
                {
                    employeeMenu.ShowEmployeeMenu();
                }
            }
        }
    }
}
