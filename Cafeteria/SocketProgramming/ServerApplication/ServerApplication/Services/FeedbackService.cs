using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using ServerApplication.Models;

namespace ServerApplication.Services
{
    public class FeedbackService
    {
        private DbHandler dbHandler;

        public FeedbackService(DbHandler dbHandler)
        {
            this.dbHandler = dbHandler;
        }

        public Dictionary<string, List<FeedbackDetail>> GetFeedbackGroupedByItem()
        {
            string query = @"
            SELECT 
                feedback_item, 
                JSON_ARRAYAGG(JSON_OBJECT('comment', comment, 'food_rating', food_rating)) AS feedback_details
            FROM 
                feedback
            WHERE 
                feedback_date = CURDATE() - INTERVAL 1 DAY
            GROUP BY 
                feedback_item;";

            Dictionary<string, List<FeedbackDetail>> feedbackDictionary = new Dictionary<string, List<FeedbackDetail>>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query, null))
            {
                while (reader.Read())
                {
                    string feedbackItem = reader.GetString("feedback_item");
                    string feedbackDetailsJson = reader.GetString("feedback_details");

                    JArray feedbackDetailsArray = JArray.Parse(feedbackDetailsJson);

                    List<FeedbackDetail> feedbackDetailsList = new List<FeedbackDetail>();
                    foreach (var item in feedbackDetailsArray)
                    {
                        feedbackDetailsList.Add(new FeedbackDetail
                        {
                            Comment = item["comment"].ToString(),
                            FoodRating = int.Parse(item["food_rating"].ToString())
                        });
                    }

                    feedbackDictionary.Add(feedbackItem, feedbackDetailsList);
                }
            }

            return feedbackDictionary;
        }
    }
}
