using System.Net.Sockets;
using System.Text.Json;
using ServerApplication.Services;

namespace ServerApplication.Models
{
    public class ClientHandler
    {
        private TcpClient client;
        private Authentication authentication;
        private DbHandler dbHandler;

        public ClientHandler(TcpClient client, Authentication authentication, DbHandler dbHandler)
        {
            this.client = client;
            this.authentication = authentication;
            this.dbHandler = dbHandler;
        }

        public void Process()
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
            AdminService adminService = new(dbHandler);
            ChefService chefService = new(dbHandler);

            try
            {
                string jsonRequest = reader.ReadLine();
                var request = JsonSerializer.Deserialize<LoginRequest>(jsonRequest);

                User user = authentication.Login(request.Email, request.Password);

                if (user != null)
                {
                    writer.WriteLine(JsonSerializer.Serialize(new LoginResponse
                    {
                        IsAuthenticated = true,
                        Role = user.Role,
                        Message = "Login successful"
                    }));

                    if (user.Role == "admin")
                    {
                        HandleAdminRequests(reader, writer, adminService);
                    }
                    else if (user.Role == "chef")
                    {
                        HandleChefRequests(reader, writer, chefService, adminService);
                    }    
                    else
                    {
                        HandleClientRequests(user, reader, writer);
                    }
                }
                else
                {
                    writer.WriteLine(JsonSerializer.Serialize(new LoginResponse
                    {
                        IsAuthenticated = false,
                        Role = null,
                        Message = "Invalid email or password"
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing client request: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void HandleChefRequests(StreamReader reader, StreamWriter writer, ChefService chefService, AdminService adminService)
        {
            string clientRequest;
            while ((clientRequest = reader.ReadLine()) != null)
            {
                var request = JsonSerializer.Deserialize<ChefRequest>(clientRequest);
                ChefResponse response = new ChefResponse();
                AdminResponse adminResponse = new AdminResponse();

                switch (request.Action)
                {
                    case "create":
                        response.Success = chefService.SaveNotification(request.NotificationMessage);
                        response.Message = response.Success ? "Menu item added successfully." : "Failed to add menu item.";
                        break;
                    case "read":
                        adminResponse.MenuItems = adminService.GetMenuItems();
                        adminResponse.Success = adminResponse.MenuItems != null;
                        adminResponse.Message = adminResponse.Success ? "Menu items retrieved successfully." : "Failed to retrieve menu items.";
                        break;
                    case "readRecommendationMenu":
                        response.RecommendedMenuItems = chefService.GetRecommendedMenuItems();
                        response.Success = response.RecommendedMenuItems != null;
                        response.Message = response.Success ? "Recommended menu items retrieved successfully." : "Failed to retrieve menu items.";
                        break;
                    case "logout":
                        writer.WriteLine("Logged out");
                        return;
                }

                writer.WriteLine(JsonSerializer.Serialize(response));
            }
        }

        private void HandleAdminRequests(StreamReader reader, StreamWriter writer, AdminService adminService)
        {
            string clientRequest;
            while ((clientRequest = reader.ReadLine()) != null)
            {
                var request = JsonSerializer.Deserialize<AdminRequest>(clientRequest);
                AdminResponse response = new AdminResponse();

                switch (request.Action)
                {
                    case "create":
                        response.Success = adminService.AddMenuItem(request.Name, request.Price, request.Category);
                        response.Message = response.Success ? "Menu item added successfully." : "Failed to add menu item.";
                        break;
                    case "read":
                        response.MenuItems = adminService.GetMenuItems();
                        response.Success = response.MenuItems != null;
                        response.Message = response.Success ? "Menu items retrieved successfully." : "Failed to retrieve menu items.";
                        break;
                    case "update":
                        response.Success = adminService.UpdateMenuItem(request.ItemId, request.Name, request.Price, request.Category);
                        response.Message = response.Success ? "Menu item updated successfully." : "Failed to update menu item.";
                        break;
                    case "delete":
                        response.Success = adminService.DeleteMenuItem(request.ItemId);
                        response.Message = response.Success ? "Menu item deleted successfully." : "Failed to delete menu item.";
                        break;
                    case "logout":
                        authentication.Logout(request.Email);
                        writer.WriteLine("Logged out");
                        return;
                }

                writer.WriteLine(JsonSerializer.Serialize(response));
            }
        }

        private void HandleClientRequests(User user, StreamReader reader, StreamWriter writer)
        {
            string clientRequest;
            while ((clientRequest = reader.ReadLine()) != null)
            {
                if (clientRequest.ToLower() == "logout")
                {
                    authentication.Logout(user.Email);
                    writer.WriteLine("Logged out");
                    return;
                }
            }
        }
    }
}
