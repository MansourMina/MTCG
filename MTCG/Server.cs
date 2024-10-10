using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public class Server
    {
        private TcpListener _listener;
        public IPAddress IpAddress { get; }
        public int Port { get; }

        public Server(IPAddress ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
            _listener = new TcpListener(IpAddress, Port);
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine($"Server {IpAddress} waiting for connections...");
            IncomingConnections();
        }

        private void IncomingConnections()
        {
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                using var reader = new StreamReader(client.GetStream());
                using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                HandleConnection(client, reader, writer);
            }
        }

        private void HandleConnection(TcpClient client, StreamReader reader, StreamWriter writer)
        {
            HttpRequest request = new HttpRequest(reader);
            HttpResponse response = new HttpResponse(writer);
            HttpHandler handler = new HttpHandler(request, response);

            Console.WriteLine($"Response Status: {response.Status} ");

        }
    }
}
