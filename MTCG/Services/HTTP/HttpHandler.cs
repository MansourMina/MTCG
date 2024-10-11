using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using MTCG.Models;
using Newtonsoft.Json;
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
            Unauthorized= 401,
            Conflict= 409
        }

        public struct ResponseFormat
        {
            public int Status;
            public string Body;
        }

        public Dictionary<Tuple<string, string>, Action<HttpRequest, HttpResponse>> Requests = new();
        //public Dictionary<string, string> Responses = new();
        public HttpHandler(HttpRequest request, HttpResponse response)
        {
            initializeRequests();
            //initializeResponses();
            Handle(request, response);
        }

        //private void initializeResponses()
        //{
        //    Responses["404"] = "Page Not Found";
        //    Responses["200"] = "OK";
        //}
        private void initializeRequests()
        { 
            //Requests[Tuple.Create("/users", "GET")] = GetUsers;
            Requests[Tuple.Create("/sessions", "POST")] = CreateSession;
            Requests[Tuple.Create("/users", "POST")] = RegisterUser;
        }
        private void Handle(HttpRequest request, HttpResponse response)
        {
            Console.WriteLine("-------------------------------------------");
            var clientRequest = Tuple.Create(request.Path, request.Method);
            Console.WriteLine(request.Method + " " + request.Path);

            if (Requests.ContainsKey(clientRequest))
            {
                try
                {
                    Requests[clientRequest](request, response);
                }
                catch (InvalidOperationException e)
                {
                    SetResponse(response, (int)HTTPStatusCode.Conflict, e.Message);
                }
                catch (ArgumentException e)
                {
  
                    SetResponse(response, (int)HTTPStatusCode.BadRequest, e.Message);

                }
                catch (UnauthorizedAccessException e)
                {
                    SetResponse(response, (int)HTTPStatusCode.Unauthorized, e.Message);
                }
            }
            else
            {
                SetResponse(response, (int)HTTPStatusCode.NotFound, "Page not Found");
            }
            response.Send();
        }
        private void RegisterUser(HttpRequest request, HttpResponse response)
        {
            RegisterService registerService = new RegisterService();
            string jsonBody = request.Body.ToString();
            var dUser = JsonConvert.DeserializeObject<DeserializeUser>(jsonBody);
            string token = registerService.Register(dUser.Username, dUser.Password);
            SetResponse(response, (int)HTTPStatusCode.Created, "User created successfully");
        }
        private void CreateSession(HttpRequest request, HttpResponse response)
        {
            LoginService loginService = new LoginService();
            string json = request.Body.ToString();
            var dUser = JsonConvert.DeserializeObject<DeserializeUser>(json);
            string token = loginService.Login(dUser.Username, dUser.Password);
            SetResponse(response, (int)HTTPStatusCode.OK, token);
        }

        private void SetResponse(HttpResponse response, int status, string body)
        {
            response.Status = $"{status.ToString()}";
            response.Body = string.IsNullOrEmpty(body) ? "" : body;
            Console.WriteLine("Body: " + response.Body);
        }
    }
}
