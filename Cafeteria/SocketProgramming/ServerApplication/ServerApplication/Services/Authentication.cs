using MySql.Data.MySqlClient;
using ServerApplication.Models;
using System.Data;

namespace ServerApplication.Services
{
    public class Authentication
    {
        private DbHandler dbHandler;

        public Authentication(DbHandler dbHandler)
        {
            this.dbHandler = dbHandler;
        }

        public User Login(string email, string password)
        {
            string query = "SELECT * FROM user WHERE email = @Email AND password = @Password";
            MySqlParameter[] parameters = {
            new MySqlParameter("@Email", email),
            new MySqlParameter("@Password", password)
        };

            DataTable result = dbHandler.ExecuteQuery(query, parameters);

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                return new User
                {
                    PersonId = Convert.ToInt32(row["personid"]),
                    Email = row["email"].ToString(),
                    Password = row["password"].ToString(),
                    Role = row["role"].ToString()
                };
            }
            return null;
        }

        public void Logout(string email)
        {
            Console.WriteLine($"User {email} logged out.");
        }
    }
}
