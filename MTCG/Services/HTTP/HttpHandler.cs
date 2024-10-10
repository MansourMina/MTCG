using System;
using System.Collections.Generic;
using System.Linq;
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
        public Dictionary<Tuple<string, string>, Action<HttpRequest, HttpResponse>> RequestTypes = new();
        public HttpHandler(HttpRequest request, HttpResponse response)
        {
            var usersGetRequest = Tuple.Create("/users", "GET");
            var loginRequest = Tuple.Create("/sessions", "POST");
            RequestTypes[usersGetRequest] = GetUsers;
            RequestTypes[loginRequest] = CreateSession;
            Handle(request, response);
        }
        private void Handle(HttpRequest request, HttpResponse response)
        {
            var key = Tuple.Create(request.Path, request.Method);
            Console.WriteLine(request.Path + ": " + request.Method);
            if (RequestTypes.ContainsKey(key))
            {
                RequestTypes[key](request, response); // Ruft die zugehörige Methode auf
            }
            else
            {
                response.Status = "404 Not Found";
                response.Body = "<html><body>Page not found</body></html>";
            }
            response.Send();
        }

        private void CreateSession(HttpRequest request, HttpResponse response)
        {
            response.Status = "200";
            LoginService loginService = new LoginService();
            string json = request.Body.ToString();
            var dUser = JsonConvert.DeserializeObject<DeserializeUser>(json);
            string token = loginService.Login(dUser.Username, dUser.Password);
            response.Body = token;
        }

        private void GetUsers(HttpRequest request, HttpResponse response)
        {
            response.Status = "200";
            response.Body = $"Here are alle the Users ";
        }
    }
}
