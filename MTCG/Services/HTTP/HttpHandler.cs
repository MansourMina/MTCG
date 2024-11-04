using MTCG.Database.Repositories;
using MTCG.Models;
using MTCG.Presentation;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Web;
using static System.Collections.Specialized.BitVector32;
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

            // Sessions
            Routes[Tuple.Create("/sessions", "POST")] = CreateSession;

            // Users
            Routes[Tuple.Create("/users", "POST")] = RegisterUser;
            Routes[Tuple.Create("/users", "GET")] = GetUser;
            Routes[Tuple.Create("/users", "DELETE")] = DeleteUser;
            Routes[Tuple.Create("/users", "PATCH")] = UpdateUser;
        }

        private void Handle(HttpRequest request, HttpResponse response)
        {
            Console.WriteLine("-------------------------------------------");

            string path = GetMainRoute(request.Path);
            var clientRequest = Tuple.Create(path, request.Method);

            Console.WriteLine(request.Method + " " + request.Path);

            var finalResponse = new ResponseFormat { Status = (int)HTTPStatusCode.NotFound, Body = "Path Not found" };

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
                catch (KeyNotFoundException e)
                {
                    finalResponse.Status = (int)HTTPStatusCode.NotFound;
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
            if (dUser == null)
                throw new ArgumentException($"Failed to register user");
            string token = registerService.Register(dUser.Username, dUser.Password);
            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "User created successfully" };
        }

        private ResponseFormat CreateSession(HttpRequest request)
        {
            LoginService loginService = new LoginService();
            var dUser = JsonSerializer.Deserialize<User>(request.Body.ToString());
            if (dUser == null)
                throw new ArgumentException($"Failed to create session");
            string token = loginService.Login(dUser.Username, dUser.Password);
            return new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = token };
        }

        private ResponseFormat GetUser(HttpRequest request)
        {
            UserRepository dbUser = new UserRepository();
            ResponseFormat response = new ResponseFormat() { Status = (int)HTTPStatusCode.OK };
            string? query = ExtractQuery(request.Path, "Username");
            if (query != null)
            {
                User? user = dbUser.Get(query);
                if (user == null)
                    throw new KeyNotFoundException($"User '{query}' does not exist");
                response.Body = JsonSerializer.Serialize(user);
            }
            else
            {
                var users = dbUser.GetAll();
                response.Body = JsonSerializer.Serialize(users);
            }
            return response;
        }

        private ResponseFormat DeleteUser(HttpRequest request)
        {
            string[] pathParts = request.Path.Split('/');
            if (pathParts.Length <= 2)
                throw new ArgumentException($"Failed to delete user");

            string username = pathParts[2];
            UserRepository dbUser = new UserRepository();
            int rowsAffected = dbUser.Delete(username);
            if (rowsAffected == 0)
                throw new KeyNotFoundException($"User '{username}' does not exist");
            return new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = "User deleted successfully" };
        }

        private ResponseFormat UpdateUser(HttpRequest request)
        {
            string[] pathParts = request.Path.Split('/');
            var body = JsonSerializer.Deserialize<User>(request.Body.ToString());
            if (body == null || pathParts.Length <= 2)
                throw new ArgumentException($"Failed to update user");

            string username = pathParts[2];

            UserRepository dbUser = new UserRepository();
            var user = dbUser.Get(username);
            if (user == null)
                throw new KeyNotFoundException($"User '{username}' does not exist");


            ChangeUserProperties(user, body);
            dbUser.Update(username, user);

            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "User updated successfully" };
        }

        private void ChangeUserProperties(User user, User body)
        {
            bool propertiesChanged = false;
            if (!string.IsNullOrEmpty(body.Password))
            {
                user.ChangePassword(BCrypt.Net.BCrypt.EnhancedHashPassword(body.Password));
                propertiesChanged = true;
            }
            if (!string.IsNullOrEmpty(body.Username))
            { 
                user.ChangeUsername(body.Username);
                propertiesChanged = true;
            }
            if (body.Elo.HasValue)
            { 
                user.SetElo(body.Elo.Value);
                propertiesChanged = true;
            }
            if (body.Coins.HasValue)
            { 
                user.SetCoins(body.Coins.Value);
                propertiesChanged = true;
            }
            if(!propertiesChanged) throw new ArgumentException($"Failed to update user");
        }

        public static string GetMainRoute(string url)
        {
            var path = url.Split('?')[0];
            var segments = path.Split('/');
            return segments.Length > 1 ? "/" + segments[1] : "/";
        }

        private void SendResponse(HttpResponse response, int status, string body)
        {
            response.Status = status.ToString();
            response.Body = string.IsNullOrEmpty(body) ? "" : body;
            response.Send();
            Console.WriteLine("Body: " + response.Body);
        }
        public string? ExtractQuery(string url, string queryName)
        {
            var uri = new Uri("http://dummy.com" + url);
            var query = HttpUtility.ParseQueryString(uri.Query);

            if (query.AllKeys.Contains(queryName))
                return query[queryName];
            return null;
        }
    }
}
