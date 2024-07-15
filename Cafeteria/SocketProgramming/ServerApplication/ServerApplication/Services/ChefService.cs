using System.Collections.Generic;
using System.Text.Json;
using MySql.Data.MySqlClient;
using ServerApplication.Models;

namespace ServerApplication.Services
{
    public class ChefService
    {
        private DbHandler dbHandler;
        private RecommendationEngineService recommendationEngineService;

        public ChefService(DbHandler dbHandler)
        {
            this.dbHandler = dbHandler;
            recommendationEngineService = new(dbHandler);
        }

        public List<RecommendedItem> GetRecommendedMenuItems()
        {
            List<RecommendedItem> recommendedItems = recommendationEngineService.GenerateRecommendationMenu();
            string checkQuery = "SELECT COUNT(*) FROM food_recommendation WHERE date_created = CURDATE()";
            int count = dbHandler.ExecuteScalar(checkQuery);
            if (count > 0)
            {
                return recommendedItems;
            }
            else
            {
                string insertQuery = "INSERT INTO food_recommendation (date_created, recommended_items, sentiment_score, sentiments) VALUES (NOW(), @RecommendedItems, @SentimentScore, @Sentiments)";
                foreach (var item in recommendedItems)
                {
                    MySqlParameter[] parameters = {
                        new MySqlParameter("@RecommendedItems", item.MenuItem),
                        new MySqlParameter("@SentimentScore", item.SentimentScore),
                        new MySqlParameter("@Sentiments", item.Sentiments)
                    };
                    dbHandler.ExecuteNonQuery(insertQuery, parameters);
                }
            }

            string deleteQuery = "DELETE FROM food_recommendation WHERE date_created != CURDATE()";
            dbHandler.ExecuteNonQuery(deleteQuery);

            return recommendedItems;
        }

        public List<FullMenuItem> GetFullMenuItems()
        {
            string query = "SELECT itemid, name, price, category, date_created FROM menu_item ORDER BY itemid";
            List<FullMenuItem> menuItems = new List<FullMenuItem>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    menuItems.Add(new FullMenuItem
                    {
                        ItemId = reader.GetInt32("itemid"),
                        Name = reader.GetString("name"),
                        Price = reader.GetInt32("price"),
                        Category = reader.GetString("category"),
                        DateCreated = reader.GetDateTime("date_created")
                    });
                }
            }

            return menuItems;
        }

        public string GetMenuVotes()
        {
            string query = "SELECT vote_yes, vote_no FROM notification ORDER BY notification_date DESC LIMIT 1";
            var menuVotes = new List<object>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    var vote = new
                    {
                        VoteYes = reader.GetInt32("vote_yes"),
                        VoteNo = reader.GetInt32("vote_no")
                    };

                    menuVotes.Add(vote);
                }
            }

            return JsonSerializer.Serialize(menuVotes);
        }

        public List<string> GetDiscardItemFeedback()
        {
            string query = "select feedback from discard_item_feedback order by date_added desc";

            List<string> itemFeedbacks = new List<string>(); 

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    string discardItemFeedback = reader.GetString("feedback");
                    itemFeedbacks.Add(discardItemFeedback);
                }
            }

            return itemFeedbacks;
        }

        public bool SaveChefMenuNotification(string notificationMessage)
        {
            string query = "INSERT INTO notification (message, type, notification_date, vote_yes, vote_no) VALUES (@NotificationMessage, 'chef', NOW(), 0, 0)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@NotificationMessage", notificationMessage)
            };

            return dbHandler.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
