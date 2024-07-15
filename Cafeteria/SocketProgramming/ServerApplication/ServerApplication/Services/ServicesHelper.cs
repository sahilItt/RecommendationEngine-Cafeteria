using System.Dynamic;
using System.Text.Json;
using MySql.Data.MySqlClient;

namespace ServerApplication.Services
{
    public static class ServicesHelper
    {
        public static List<dynamic> GetFullMenuItems(DbHandler dbHandler)
        {
            string query = "SELECT itemid, name, price, category, date_created FROM menu_item ORDER BY itemid";
            List<dynamic> menuItems = new List<dynamic>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    dynamic menuItem = new ExpandoObject();
                    menuItem.ItemId = reader.GetInt32("itemid");
                    menuItem.Name = reader.GetString("name");
                    menuItem.Price = reader.GetInt32("price");
                    menuItem.Category = reader.GetString("category");
                    menuItem.DateCreated = reader.GetDateTime("date_created");
                    menuItems.Add(menuItem);
                }
            }

            return menuItems;
        }

        public static string FetchDiscardMenu(DbHandler dbHandler)
        {
            string query = "SELECT food_item, average_rating FROM discard_menu";
            var discardMenu = new List<object>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    var menuItem = new
                    {
                        FoodItem = reader.GetString("food_item"),
                        AverageRating = reader.GetDouble("average_rating")
                    };

                    discardMenu.Add(menuItem);
                }
            }

            return JsonSerializer.Serialize(discardMenu);
        }

        private static List<(string, double, string)> FetchDiscardRecommendationItems(DbHandler dbHandler)
        {
            string query = "select recommended_items, sentiment_score, sentiments from food_recommendation where sentiment_score <= 2";
            List<(string, double, string)> recommendations = new List<(string, double, string)>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    string recommendedItems = reader.GetString("recommended_items");
                    double sentimentScore = reader.GetDouble("sentiment_score");
                    string sentiments = reader.GetString("sentiments");
                    recommendations.Add((recommendedItems, sentimentScore, sentiments));
                }
            }

            return recommendations;
        }

        public static string ViewDiscardItems(DbHandler dbHandler)
        {
            string query = "select food_item, average_rating, sentiments from discard_menu";
            var discardMenu = new List<object>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    var menuItem = new
                    {
                        FoodItem = reader.GetString("food_item"),
                        AverageRating = reader.GetDouble("average_rating"),
                        Sentiments = reader.GetString("sentiments")
                    };

                    discardMenu.Add(menuItem);
                }
            }

            return JsonSerializer.Serialize(discardMenu);
        }

        public static void AddDiscardItems(DbHandler dbHandler)
        {
            List<(string, double, string)> discardedRecommendationItems = FetchDiscardRecommendationItems(dbHandler);
            foreach (var recommendation in discardedRecommendationItems)
            {
                if (!IsItemInDiscardMenu(recommendation.Item1, dbHandler))
                {
                    string query = "INSERT INTO discard_menu (food_item, average_rating, sentiments, date_added) VALUES (@FoodItem, @AverageRating, @Sentiments, NOW())";

                    MySqlParameter[] parameters = {
                        new MySqlParameter("@FoodItem", recommendation.Item1),
                        new MySqlParameter("@AverageRating", recommendation.Item2),
                        new MySqlParameter("@Sentiments", recommendation.Item3)
                    };

                    dbHandler.ExecuteNonQuery(query, parameters);
                }  
            }
        }

        private static bool IsItemInDiscardMenu(string foodItem, DbHandler dbHandler)
        {
            string query = "SELECT COUNT(*) FROM discard_menu WHERE food_item = @FoodItem";

            MySqlParameter[] parameters = {
                new MySqlParameter("@FoodItem", foodItem)
            };

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query, parameters))
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0) > 0;
                }
            }

            return false;
        }

        public static bool RemoveDiscardFoodItem(DbHandler dbHandler, string removableFoodItemName)
        {
            string query = "delete from discard_menu where food_item = @FoodItem";

            MySqlParameter[] parameters = {
                new MySqlParameter("@FoodItem", removableFoodItemName)
            };

            return dbHandler.ExecuteNonQuery(query, parameters) > 0;
        }

        public static string GetDiscardNotification(DbHandler dbHandler)
        {
            string query = "select message from notification where type = 'discard' order by notification_date desc limit 1";
            string message = "";
            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    message += reader.GetString("message");
                }
            }
            return message;
        }

        public static bool AddDiscardItemFeedback(DbHandler dbHandler, string discardItemFeedback)
        {
            string feedbackFoodItem = GetDiscardNotification(dbHandler);

            string query = @"INSERT INTO discard_item_feedback (food_item, feedback, date_added) 
                             VALUES (@FoodItem, @Feedback, NOW())";
            MySqlParameter[] parameters = {
                new MySqlParameter("@FoodItem", feedbackFoodItem),
                new MySqlParameter("@Feedback", discardItemFeedback)
            };

            return dbHandler.ExecuteNonQuery (query, parameters) > 0;
        }

        public static bool SaveDiscardMenuNotification(DbHandler dbHandler, string notificationMessage)
        {
            string query = "INSERT INTO notification (message, type, notification_date, vote_yes, vote_no) VALUES (@NotificationMessage, 'discard', NOW(), 0, 0)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@NotificationMessage", notificationMessage)
            };

            return dbHandler.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
