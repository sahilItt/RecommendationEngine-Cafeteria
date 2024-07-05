using System.Text.Json;
using CafeteriaApplication.Models;
using static CafeteriaApplication.Utils.MenuHelper;

namespace CafeteriaApplication.Controller
{
    public class ChefController
    {
        private StreamWriter writer;
        private StreamReader reader;

        public ChefController(StreamWriter writer, StreamReader reader)
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
                Console.WriteLine("4. View Menu Votes");
                Console.WriteLine("5. View Discard Menu Item List");
                Console.WriteLine("6. Logout");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ChefRequest request = new ChefRequest { Action = "read" };
                        ViewMenuItems(writer, reader, request);
                        break;
                    case "2":
                        ViewRecommendationItems();
                        break;
                    case "3":
                        SendNextDayMenu();
                        break;
                    case "4":
                        ViewMenuVotes();
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

                Console.WriteLine("{0, -30} | {1, -10} | {2, -10}", "Name", "Sentiment Score", "Sentiments");
                Console.WriteLine(new string('-', 90));
                foreach (var item in sortedItems)
                {
                    Console.WriteLine("{0, -30} | {1, -10} | {2, -10}", item.MenuItem, item.SentimentScore, item.Sentiments);
                }
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        private void ViewMenuVotes()
        {
            ChefRequest request = new ChefRequest { Action = "viewMenuVotes" };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<ChefResponse>(responseJson);

            if (response.Success)
            {
                using (JsonDocument document = JsonDocument.Parse(response.MenuVotes))
                {
                    var votesArray = document.RootElement;

                    if (votesArray.ValueKind == JsonValueKind.Array && votesArray.GetArrayLength() > 0)
                    {
                        var votes = votesArray[0];
                        int voteYes = votes.GetProperty("VoteYes").GetInt32();
                        int voteNo = votes.GetProperty("VoteNo").GetInt32();

                        Console.WriteLine("Total Votes for menu: ");
                        Console.WriteLine("{0, -10} | {1, -10}", "VoteYes", "VoteNo");
                        Console.WriteLine("{0, -10} | {1, -10}", voteYes, voteNo);
                    }
                    else
                    {
                        Console.WriteLine("No vote data available.");
                    }
                }
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        private void ViewDiscardMenuItems()
        {
            ChefRequest request = new ChefRequest { Action = "readDiscardMenu" };
            DiscardMenuItems(writer, reader, request);
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
