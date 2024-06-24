using System.Text.Json;
using CafeteriaApplication.Models;

namespace CafeteriaApplication.Utils
{
    public class ChefMenu
    {
        private StreamWriter writer;
        private StreamReader reader;

        public ChefMenu(StreamWriter writer, StreamReader reader)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public void ShowChefMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Chef Menu:");
                Console.WriteLine("1. View Menu");
                Console.WriteLine("2. View Recommendation");
                Console.WriteLine("3. Send Menu for Next Day");
                Console.WriteLine("4. Logout");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ViewFullMenu();
                        break;
                    case "2":
                        ViewRecommendationItems();
                        break;
                    case "3":
                        SendNextDayMenu();
                        break;
                    case "4":
                        Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
            }
        }

        private void ViewFullMenu()
        {
            ChefRequest request = new ChefRequest { Action = "read" };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<ChefResponse>(responseJson);

            if (response.Success)
            {
                Console.WriteLine("{0, -5} | {1, -30} | {2, -10} | {3, -20} | {4, -20}", "ID", "Name", "Price (INR)", "Category", "Date Created");
                Console.WriteLine(new string('-', 90));
                foreach (var item in response.FullMenuItems)
                {
                    Console.WriteLine("{0, -5} | {1, -30} | {2, -10} | {3, -20} | {4, -20}",
                        item.ItemId,
                        item.Name,
                        item.Price,
                        item.Category,
                        item.DateCreated.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        private void ViewRecommendationItems()
        {
            ChefRequest request = new ChefRequest { Action = "readRecommendationMenu" };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<ChefResponse>(responseJson);
            if (response.Success)
            {
                var sortedItems = response.RecommendedMenuItems
                    .OrderByDescending(item => item.SentimentScore)
                    .ToList();

                Console.WriteLine("{0, -30} | {1, -10}", "Name", "Sentiment Score");
                Console.WriteLine(new string('-', 50));
                foreach (var item in sortedItems)
                {
                    Console.WriteLine("{0, -30} | {1, -10}", item.MenuItem, item.SentimentScore);
                }
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        private void SendNextDayMenu()
        {
            Console.WriteLine("Enter item IDs (comma-separated, e.g., 1,2,3):");
            string input = Console.ReadLine();
            string[] idStrings = input.Split(',');

            List<int> itemIds = new List<int>();
            foreach (string idString in idStrings)
            {
                if (int.TryParse(idString.Trim(), out int id))
                {
                    itemIds.Add(id);
                }
                else
                {
                    Console.WriteLine($"Invalid item ID: {idString.Trim()}. Skipping...");
                }
            }

            ChefRequest request = new ChefRequest { Action = "create", ItemIds = itemIds };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<ChefResponse>(responseJson);
            Console.WriteLine(response.Message);
        }


        private void Logout()
        {
            ChefRequest request = new ChefRequest { Action = "logout" };
            writer.WriteLine(JsonSerializer.Serialize(request));
            string response = reader.ReadLine();

            if (response.ToLower() == "logged out")
            {
                Console.WriteLine("Logged out successfully.");
            }
        }
    }
}
