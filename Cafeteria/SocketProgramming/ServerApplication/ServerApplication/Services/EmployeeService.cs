using System.Data;
using System.Text.Json;
using MySql.Data.MySqlClient;
using ServerApplication.Models;

namespace ServerApplication.Services
{
    public class EmployeeService
    {
        private DbHandler dbHandler;

        public EmployeeService(DbHandler dbHandler)
        {
            this.dbHandler = dbHandler;
        }

        public List<MenuNotification> GetMenuNotifications(string notificationType, string userEmail)
        {
            string query = @"select message, type, vote_yes, vote_no from notification 
                             where type = @NotificationType and notification_date between now() - interval 1 day and now()
                             order by notification_date desc 
                             limit 1;";
            MySqlParameter[] parameters = {
                new MySqlParameter("@NotificationType", notificationType)
            };
            List<MenuNotification> menuNotifications = new List<MenuNotification>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query, parameters))
            {
                while (reader.Read())
                {
                    string message = reader.GetString("message");
                    string type = reader.GetString("type");
                    int voteYes = reader.GetInt32("vote_yes");
                    int voteNo = reader.GetInt32("vote_no");

                    List<MenuNotificationItem> items = JsonSerializer.Deserialize<List<MenuNotificationItem>>(message);
                    string recommendationMessage = "";

                    if (notificationType == "chef")
                    {
                        (items, recommendationMessage) = GetUserPreferredMenu(message, userEmail);
                    }

                    menuNotifications.Add(new MenuNotification
                    {
                        VoteYes = voteYes,
                        VoteNo = voteNo,
                        Type = type,
                        Items = items,
                        RecommendationMessage = recommendationMessage
                    });
                }
            }

            return menuNotifications;
        }

        public bool UpdateEmployeeProfile(string userEmail, Profile userProfile)
        {
            string query = @"UPDATE profile 
                             SET vegetarian_preference = @VegetarianPreference, 
                             spice_level = @SpiceLevel, 
                             food_preference = @FoodPreference, 
                             sweet_tooth = @SweetTooth
                             WHERE email = @UserEmail";

            MySqlParameter[] parameters = {
                new MySqlParameter("@VegetarianPreference", userProfile.VegetarianPreference),
                new MySqlParameter("@SpiceLevel", userProfile.SpiceLevel),
                new MySqlParameter("@FoodPreference", userProfile.FoodPreference),
                new MySqlParameter("@SweetTooth", userProfile.SweetTooth),
                new MySqlParameter("@UserEmail", userEmail)
            };

            int rowsAffected = dbHandler.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool AddMenuVotes(bool hasLikedMenu)
        {
            string query;
            if(hasLikedMenu)
            {
                query = "UPDATE notification SET vote_yes = vote_yes + 1 ORDER BY notification_date DESC LIMIT 1;";
            }
            else
            {
                query = "UPDATE notification SET vote_no = vote_no + 1 ORDER BY notification_date DESC LIMIT 1;";
            }

            return dbHandler.ExecuteNonQuery(query) > 0;
        }

        public bool AddItemFeedback(int itemId, int foodRating, string comment, string userEmail)
        {
            string selectQuery = "SELECT name FROM menu_item WHERE itemid = @ItemId";
            MySqlParameter[] selectParameters = {
                new MySqlParameter("@ItemId", itemId)
            };

            DataTable result = dbHandler.ExecuteQuery(selectQuery, selectParameters);
            if (result.Rows.Count == 0)
            {
                return false;
            }

            string feedbackItemName = result.Rows[0]["name"].ToString();

            string insertQuery = "INSERT INTO feedback (feedback_date, comment, food_rating, feedback_person, feedback_item, ItemId) VALUES (NOW(), @Comment, @FoodRating, @UserEmail, @FeedbackItem, @ItemId)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Comment", comment),
                new MySqlParameter("@FoodRating", foodRating),
                new MySqlParameter("@UserEmail", userEmail),
                new MySqlParameter("@FeedbackItem", feedbackItemName),
                new MySqlParameter("@ItemId", itemId)
            };

            return dbHandler.ExecuteNonQuery(insertQuery, parameters) > 0;
        }

        private (List<MenuNotificationItem> SortedItems, string Recommendation) GetUserPreferredMenu(string message, string userEmail)
        {
            List<MenuNotificationItem> items = JsonSerializer.Deserialize<List<MenuNotificationItem>>(message);

            if (items == null || !items.Any())
            {
                return (new List<MenuNotificationItem>(), "No items available.");
            }

            string itemIds = string.Join(",", items.Select(item => item.ItemId));
            Profile userProfile = GetUserProfile(userEmail);

            if (userProfile == null)
            {
                return (new List<MenuNotificationItem>(), "User profile not found.");
            }

            List<MenuNotificationItem> menuItems = GetMenuItems(itemIds);
            List<MenuNotificationItem> sortedItems = menuItems
                .OrderByDescending(item => item.DietaryType == userProfile.VegetarianPreference)
                .ThenByDescending(item => item.FoodType == userProfile.FoodPreference)
                .ToList();

            string recommendation = AddRecommendations(sortedItems, userProfile);

            return (sortedItems, recommendation);
        }

        private List<MenuNotificationItem> GetMenuItems(string itemIds)
        {
            string query = $"SELECT * FROM menu_item WHERE itemid IN ({itemIds})";
            List<MenuNotificationItem> menuItems = new List<MenuNotificationItem>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    menuItems.Add(new MenuNotificationItem
                    {
                        ItemId = reader.GetInt32("itemid"),
                        ItemName = reader.GetString("name"),
                        Price = reader.GetFloat("price"),
                        Category = reader.GetString("category"),
                        SweetTooth = reader.GetString("sweet_tooth"),
                        DietaryType = reader.GetString("dietary_type"),
                        SpiceLevel = reader.GetString("spice_level"),
                        FoodType = reader.GetString("food_type")
                    });
                }
            }

            return menuItems;
        }

        private Profile GetUserProfile(string email)
        {
            string query = "SELECT * FROM profile WHERE email = @Email";

            MySqlParameter[] parameters = {
                new MySqlParameter("@Email", email)
            };

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query, parameters))
            {
                if (reader.Read())
                {
                    return new Profile
                    {
                        VegetarianPreference = reader.GetString("vegetarian_preference"),
                        SpiceLevel = reader.GetString("spice_level"),
                        FoodPreference = reader.GetString("food_preference"),
                        SweetTooth = reader.GetString("sweet_tooth")
                    };
                }
            }

            return null;
        }

        private string AddRecommendations(List<MenuNotificationItem> items, Profile userProfile)
        {
            List<string> recommendations = new List<string>();
            List<MenuNotificationItem> spicyItems = items.Where(item => item.SpiceLevel == userProfile.SpiceLevel).ToList();

            if (spicyItems.Any())
            {
                recommendations.Add($"Because you like {userProfile.SpiceLevel} spice, you can go with {string.Join(", ", spicyItems.Select(item => item.ItemName))}");
            }

            if (userProfile.SweetTooth == "Yes")
            {
                List<MenuNotificationItem> sweetItems = items.Where(item => item.SweetTooth == "Yes").ToList();
                if (sweetItems.Any())
                {
                    recommendations.Add($"Since you have a sweet tooth, you might enjoy {string.Join(", ", sweetItems.Select(item => item.ItemName))}");
                }
            }

            if (recommendations.Any())
            {
                return string.Join(". ", recommendations);
            }
            else
            {
                return "No items match your preferences.";
            }
        }

    }
}
