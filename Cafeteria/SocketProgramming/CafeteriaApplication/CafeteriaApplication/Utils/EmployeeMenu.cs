namespace CafeteriaApplication.Utils
{
    public class EmployeeMenu
    {
        private StreamWriter writer;
        private StreamReader reader;

        public EmployeeMenu(StreamWriter writer, StreamReader reader)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public void ShowEmployeeMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Chef Menu:");
                Console.WriteLine("1. View Notification");
                Console.WriteLine("2. Give Feedback & Rating");
                Console.WriteLine("3. Logout");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ViewNotification();
                        break;
                    case "2":
                        GiveFeedback();
                        break;
                    case "3":
                        Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
            }
        }

        private void ViewNotification()
        {

        }

        private void GiveFeedback()
        {

        }

        private void Logout()
        {

        }
    }
}
