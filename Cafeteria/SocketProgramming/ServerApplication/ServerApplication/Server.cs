using ServerApplication.Services;
using System.Net;
using System.Net.Sockets;
using ServerApplication.Models;

namespace ServerApplication
{
    class Server
    {
        private TcpListener server;
        private Authentication authentication;
        private DbHandler dbHandler;

        public Server(string ipAddress, int port, Authentication authentication, DbHandler dbHandler)
        {
            server = new TcpListener(IPAddress.Parse(ipAddress), port);
            this.authentication = authentication;
            this.dbHandler = dbHandler;
        }

        public void Start()
        {
            server.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected...");
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            ClientHandler clientHandler = new ClientHandler(client, authentication, dbHandler);
            clientHandler.Process();
        }
    }
}
