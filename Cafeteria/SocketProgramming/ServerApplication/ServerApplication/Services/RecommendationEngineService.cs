using MySql.Data.MySqlClient;
using System.Xml.Linq;
using Newtonsoft.Json;
using ServerApplication.Models;

namespace ServerApplication.Services
{
    public class RecommendationEngineService : FeedbackService
    {
        private Dictionary<string, int> presetComments;
        private DbHandler dbHandler;

        public RecommendationEngineService(DbHandler dbHandler) : base(dbHandler)
        {
            presetComments = LoadPresetCommentsFromJson("../../../Data/presetComments.json");
            this.dbHandler = dbHandler;
        }

        private Dictionary<string, int> LoadPresetCommentsFromJson(string filePath)
        {
            Dictionary<string, int> commentsDict = new Dictionary<string, int>();

            try
            {
                string json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<PresetCommentsData>(json);

                foreach (var item in data.PresetComments)
                {
                    commentsDict[item.Comment] = item.Score;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading JSON file: " + ex.Message);
            }

            return commentsDict;
        }

        public List<RecommendedItem> GenerateRecommendationMenu()
        {
            List<RecommendedItem> result = new List<RecommendedItem> ();
            Dictionary<string, List<FeedbackDetail>> foodItemFeedback = GetFeedbackGroupedByItem();
            foreach (var item in foodItemFeedback)
            {
                double score = GetSentimentScores(item.Value);
                string sentimentTexts = GetSentimentTexts(item.Value);
                result.Add(new RecommendedItem
                {
                    MenuItem = item.Key,
                    SentimentScore = score,
                    Sentiments = sentimentTexts
                });
            }

            return result;
        }

        private double GetSentimentScores(List<FeedbackDetail> commentsWithRatings)
        {
            double totalScore = 0;

            foreach (var item in commentsWithRatings)
            {
                int presetScore = 0;
                foreach (var preset in presetComments)
                {
                    if (item.Comment.Contains(preset.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        presetScore = preset.Value;
                        break;
                    }
                }

                totalScore += (presetScore + item.FoodRating) / 2.0;
            }

            return totalScore/commentsWithRatings.Count;
        }

        private string GetSentimentTexts(List<FeedbackDetail> commentsWithRatings)
        {
            List<string> sentimentTexts = new List<string>();

            foreach (var item in commentsWithRatings)
            {
                foreach (var preset in presetComments)
                {
                    if (item.Comment.Contains(preset.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        sentimentTexts.Add(preset.Key);
                        break;
                    }
                }
            }

            if (sentimentTexts.Count == 0)
            {
                return "NA";
            }

            return string.Join(", ", sentimentTexts);
        }
    }
}
