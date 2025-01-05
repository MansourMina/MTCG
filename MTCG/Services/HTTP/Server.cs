using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MTCG.Services.HTTP
{
    public class Server
    {
        private readonly TcpListener _server;
        public IPAddress IpAddress { get; }
        public static int Port { get; private set; }
        private static readonly object _lock = new();

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
                Task.Run(() => HandleClient(client));
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                using var reader = new StreamReader(client.GetStream());
                using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                HandleConnection(reader, writer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
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
