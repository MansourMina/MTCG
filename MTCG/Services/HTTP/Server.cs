using System.Net;
using System.Net.Sockets;

namespace MTCG.Services.HTTP
{
    public class Server
    {
        private readonly TcpListener _server;
        public IPAddress IpAddress { get; }
        public static int Port { get; private set; }

        public Server(IPAddress ipAddress, int port = 8000)
        {
            IpAddress = ipAddress;
            Port = port;
            _server = new TcpListener(IpAddress, Port);
        }

        public void Start()
        {
            try
            {
                _server.Start();
                Console.WriteLine($"Server {IpAddress}:{Port} waiting for connections...");
                IncomingConnections();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Server Error: {e.Message}");
            }
            
        }

        private void IncomingConnections()
        { 
            while (true)
            {
                var client = _server.AcceptTcpClient();
                using var reader = new StreamReader(client.GetStream());
                using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                HandleConnection(reader, writer);
            }
        }

        private static void HandleConnection(StreamReader reader, StreamWriter writer)
        {
            try
            {
                HttpRequest request = new (reader);
                HttpResponse response = new (writer);
                HttpHandler handler = new (request, response);

                Console.WriteLine($"Response Status: {response.Status}");
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine($"Invalid data received: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
    }
}
