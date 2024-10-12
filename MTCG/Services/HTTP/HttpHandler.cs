using System;
using System.Collections.Generic;
using System.Text.Json;
using MTCG.Models;
using Newtonsoft.Json.Linq;
using static MTCG.Services.HTTP.HttpHandler;
namespace MTCG.Services.HTTP
{
    public class HttpHandler
    {
        enum HTTPStatusCode
        {
            NotFound = 404,
            OK = 200,
            Created = 201,
            NoContent = 204,
            BadRequest = 400,
            Unauthorized = 401,
            Conflict = 409
        }
        public struct ResponseFormat
        {
            public int Status;
            public string Body;
        }
        public Dictionary<Tuple<string, string>, Func<HttpRequest, ResponseFormat>> Routes = new();

        public HttpHandler(HttpRequest request, HttpResponse response)
        {
            initializeRequests();
            Handle(request, response);
        }

        private void initializeRequests()
        {
            Routes[Tuple.Create("/", "GET")] = (request) =>
                new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = "Whats Poppin -Server" };

            Routes[Tuple.Create("/sessions", "POST")] = CreateSession;
            Routes[Tuple.Create("/users", "POST")] = RegisterUser;
        }

        private void Handle(HttpRequest request, HttpResponse response)
        {
            Console.WriteLine("-------------------------------------------");
            var clientRequest = Tuple.Create(request.Path, request.Method);
            Console.WriteLine(request.Method + " " + request.Path);

            var finalResponse = new ResponseFormat { Status = (int)HTTPStatusCode.NotFound, Body = "Not found" };

            if (Routes.ContainsKey(clientRequest))
            {
                try
                {
                    var responseFormat = Routes[clientRequest](request);
                    finalResponse.Status = responseFormat.Status;
                    finalResponse.Body = responseFormat.Body;
                }
                catch (InvalidOperationException e)
                {
                    finalResponse.Status = (int)HTTPStatusCode.Conflict;
                    finalResponse.Body = e.Message;
                }
                catch (ArgumentException e)
                {
                    finalResponse.Status = (int)HTTPStatusCode.BadRequest;
                    finalResponse.Body = e.Message;
                }
                catch (UnauthorizedAccessException e)
                {
                    finalResponse.Status = (int)HTTPStatusCode.Unauthorized;
                    finalResponse.Body = e.Message;
                }
            }
            SendResponse(response, finalResponse.Status, finalResponse.Body);
        }

        private ResponseFormat RegisterUser(HttpRequest request)
        {
            RegisterService registerService = new RegisterService();
            var dUser = JsonSerializer.Deserialize<User>(request.Body.ToString());
            string token = registerService.Register(dUser.Username, dUser.Password);
            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "User created successfully" };
        }

        private ResponseFormat CreateSession(HttpRequest request)
        {
            LoginService loginService = new LoginService();
            var dUser = JsonSerializer.Deserialize<User>(request.Body.ToString());
            string token = loginService.Login(dUser.Username, dUser.Password);
            return new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = token };
        }

        private void SendResponse(HttpResponse response, int status, string body)
        {
            response.Status = status.ToString();
            response.Body = string.IsNullOrEmpty(body) ? "" : body;
            response.Send();
            Console.WriteLine("Body: " + response.Body);
        }
    }
}
