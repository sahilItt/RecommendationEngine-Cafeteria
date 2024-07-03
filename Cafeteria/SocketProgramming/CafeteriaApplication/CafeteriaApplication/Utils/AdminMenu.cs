using System.Data;
using System.Text.Json;
using CafeteriaApplication.Models;
using static CafeteriaApplication.Utils.MenuHelper;

namespace CafeteriaApplication.Utils
{
    public class AdminMenu
    {
        private StreamWriter writer;
        private StreamReader reader;

        public AdminMenu(StreamWriter writer, StreamReader reader)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public void ShowAdminMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Add Menu Item");
                Console.WriteLine("2. View Menu Items");
                Console.WriteLine("3. Update Menu Item");
                Console.WriteLine("4. Delete Menu Item");
                Console.WriteLine("5. View Discard Menu Item List");
                Console.WriteLine("6. Logout");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        AddMenuItem();
                        break;
                    case "2":
                        ViewMenuItems();
                        break;
                    case "3":
                        UpdateMenuItem();
                        break;
                    case "4":
                        DeleteMenuItem();
                        break;
                    case "5":
                        ViewDiscardMenuItems();
                        break;
                    case "6":
                        Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
            }
        }

        private void AddMenuItem()
        {
            Console.WriteLine("Enter name:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter price:");
            float price = float.Parse(Console.ReadLine());
            Console.WriteLine("Enter category:");
            string category = Console.ReadLine();

            AdminRequest request = new AdminRequest { Action = "create", Name = name, Price = price, Category = category };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<AdminResponse>(responseJson);
            Console.WriteLine(response.Message);
        }

        public void ViewMenuItems()
        {
            AdminRequest request = new AdminRequest { Action = "read" };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<AdminResponse>(responseJson);

            if (response.Success)
            {
                Console.WriteLine(new string('-', 50));
                Console.WriteLine("| {0, -5} | {1, -20} | {2, -10} | {3, -15} | {4, -20} |", "ID", "Name", "Price (INR)", "Category", "Date Created");
                Console.WriteLine(new string('-', 50));
                foreach (var item in response.MenuItems)
                {
                    Console.WriteLine("| {0, -5} | {1, -20} | {2, -10} | {3, -15} | {4, -20} |",
                        item.ItemId,
                        item.Name,
                        item.Price,
                        item.Category,
                        item.DateCreated);
                }
                Console.WriteLine(new string('-', 50));
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        private void ViewDiscardMenuItems()
        {
            AdminRequest request = new AdminRequest { Action = "readDiscardMenu" };
            DiscardMenuItems(writer, reader, request);
        }

        private void UpdateMenuItem()
        {
            Console.WriteLine("Enter item ID to update:");
            int itemId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter new name:");
            string? name = Console.ReadLine();
            Console.WriteLine("Enter new price:");
            float price = float.Parse(Console.ReadLine());
            Console.WriteLine("Enter new category:");
            string? category = Console.ReadLine();

            AdminRequest request = new AdminRequest { Action = "update", ItemId = itemId, Name = name, Price = price, Category = category };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<AdminResponse>(responseJson);
            Console.WriteLine(response.Message);
        }

        private void DeleteMenuItem()
        {
            Console.WriteLine("Enter item ID to delete:");
            int itemId = int.Parse(Console.ReadLine());

            AdminRequest request = new AdminRequest { Action = "delete", ItemId = itemId };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<AdminResponse>(responseJson);
            Console.WriteLine(response.Message);
        }

        private void Logout()
        {
            AdminRequest request = new AdminRequest { Action = "logout" };
            writer.WriteLine(JsonSerializer.Serialize(request));
            string response = reader.ReadLine();

            if (response.ToLower() == "logged out")
            {
                Console.WriteLine("Logged out successfully.");
            }
        }
    }
}
