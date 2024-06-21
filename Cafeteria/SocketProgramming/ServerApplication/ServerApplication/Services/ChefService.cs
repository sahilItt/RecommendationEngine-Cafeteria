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
            return recommendationEngineService.GenerateRecommendationMenu();
        }

        public bool SaveNotification(string notificationMessage)
        {
            string query = "INSERT INTO notification (message, date_created) VALUES (@NotificationMessage, NOW())";
            MySqlParameter[] parameters = {
            new MySqlParameter("@NotificationMessage", notificationMessage)
        };

            return dbHandler.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
