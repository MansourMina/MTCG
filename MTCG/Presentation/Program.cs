using MTCG.Models;
using MTCG.Services;
using MTCG.Services.HTTP;
using System;
using System.Collections;
using System.Net;

namespace MTCG.Presentation
{
    internal class Program
    {
        static void Main()
        {
            StartServer();
        }

        static void StartServer()
        {
            Server server = new (IPAddress.Any, 10001);
            server.Start();
        }
    }
}
