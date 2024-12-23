using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Presentation;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net;
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
        public Dictionary<Route, Func<HttpRequest, ResponseFormat>> Routes = [];

        public HttpHandler(HttpRequest request, HttpResponse response)
        {
            InitializeRequests();
            Handle(request, response);
        }

        private void InitializeRequests()
        {
            // Default
            Routes[new Route("/", "GET", AuthorizationTypes.All)] = (request) =>
                new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = "Whats Poppin -Server" };

            // Sessions
            Routes[new Route("/sessions", "POST", AuthorizationTypes.All)] = CreateSession;

            // Users
            Routes[new Route("/users", "POST", AuthorizationTypes.All)] = RegisterUser;
            Routes[new Route("/users", "GET", AuthorizationTypes.OwnUser)] = GetUser;
            Routes[new Route("/users", "DELETE", AuthorizationTypes.All)] = DeleteUser;
            Routes[new Route("/users", "PUT", AuthorizationTypes.OwnUser)] = UpdateUser;
            Routes[new Route("/packages", "POST", AuthorizationTypes.Admin)] = CreatePackage;
            Routes[new Route("/packages", "GET", AuthorizationTypes.All)] = GetPackages;
        }

        private void Handle(HttpRequest request, HttpResponse response)
        {
            Console.WriteLine("-------------------------------------------");
            string path = GetMainRoute(request.Path ?? string.Empty);
            var clientRoute = new Route(path, request.Method, GetAuthorizationType(GetAuthorizationRole(request.Authorization)));
            Console.WriteLine(request.Method + " " + request.Path);

            var finalResponse = new ResponseFormat { Status = (int)HTTPStatusCode.NotFound, Body = "Path Not found" };

            var foundRoute = ContainsRoute(clientRoute);
            if (foundRoute != null)
            {
                var (route, func) = foundRoute.Value;
                if (!IsAuthorized(request) || clientRoute.Authorization != route.Authorization)
                {
                    finalResponse.Status = (int)HTTPStatusCode.Unauthorized;
                    finalResponse.Body = "Authentication failed";
                }
                else
                {
                    try
                    {
                        var responseFormat = func(request);
                        finalResponse.Status = responseFormat.Status;
                        finalResponse.Body = responseFormat.Body;
                    }
                    catch (DuplicateNameException e)
                    {
                        finalResponse.Status = (int)HTTPStatusCode.Conflict;
                        finalResponse.Body = e.Message;
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
                
            }
            SendResponse(response, finalResponse.Status, finalResponse.Body);
        }

        private AuthorizationTypes? GetAuthorizationType(string? authorization)
        {
            if (string.IsNullOrEmpty(authorization)) return AuthorizationTypes.All;
            switch (authorization)
            {
                case "admin":
                    return AuthorizationTypes.Admin;
                default:
                    return AuthorizationTypes.OwnUser;
            }
        }

        private string? GetAuthorizationRole(string auth)
        {
            var properties = GetAuthorizationParts(auth);
            if (properties == null) return null;
            string role = properties[0];
            return role;
        }

        private string[]? GetAuthorizationParts(string auth)
        {
            if (string.IsNullOrEmpty(auth)) return null;

            var parts = auth.Split(' ');
            if (parts.Length != 2) return null;

            // Split am ersten Bindestrich begrenzen
            var properties = parts[1].Split(new[] { '-' }, 2);
            if (properties.Length != 2) return null;

            return properties;
        }
        private string? GetAuthorizationToken(string auth)
        {
            var properties = GetAuthorizationParts(auth);
            if (properties == null) return null;
            string token = properties[1];
            return token;
        }

        private string? GetAuthorizationHolder(string auth)
        {
            var parts = auth.Split(' ');
            return parts[0];
        }


        public bool IsAuthorized(HttpRequest request)
        {
            if (string.IsNullOrEmpty(request.Authorization))
                return true;

            string? role = GetAuthorizationRole(request.Authorization);
            string? token = GetAuthorizationToken(request.Authorization);
            string? holder = GetAuthorizationHolder(request.Authorization);

            AuthorizationTypes? authType = GetAuthorizationType(role);
            if (authType == AuthorizationTypes.All)
                return true;

            

            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(holder))
                return false;

            var sessionService = new SessionService();
            string? userId = sessionService.GetUserIdByToken(token);
            if (string.IsNullOrEmpty(userId))
                return false;

            var userRepository = new UserRepository();
            User? sessionUser = userRepository.GetById(userId);
            if (sessionUser == null)
                return false;

            if(authType == AuthorizationTypes.OwnUser)
            {
                string? username = ExtractPath(request.Path ?? string.Empty);
                if (sessionUser.Username == role && username == sessionUser.Username)
                    return true;
            }
            else if (authType == AuthorizationTypes.Admin && sessionUser.Role == role) return true;
            return false;
        }



        private (Route route, Func<HttpRequest, ResponseFormat> handler)? ContainsRoute(Route request)
        {
            foreach (var route in Routes)
            {
                if (route.Key.Path == request.Path && route.Key.Method == request.Method)
                {
                    return (route.Key, route.Value);
                }
            }
            return null;
        }

        private ResponseFormat RegisterUser(HttpRequest request)
        {
            RegisterService registerService = new();
            var dUser = JsonSerializer.Deserialize<User>(request.Body.ToString()) ?? throw new ArgumentException($"Failed to register user");
            registerService.Register(dUser.Username, dUser.Password, dUser.Role);
            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "User created successfully" };
        }

        private ResponseFormat CreateSession(HttpRequest request)
        {
            LoginService loginService = new();
            var dUser = JsonSerializer.Deserialize<User>(request.Body.ToString()) ?? throw new ArgumentException($"Failed to create session");
            string token = loginService.Login(dUser.Username, dUser.Password);
            return new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = token };
        }

        private ResponseFormat GetUser(HttpRequest request)
        {
            UserRepository dbUser = new();
            ResponseFormat response = new() { Status = (int)HTTPStatusCode.OK };
            string? username = ExtractPath(request.Path ?? string.Empty);
            if (username != null)
            {
                User? user = dbUser.Get(username) ?? throw new KeyNotFoundException($"User '{username}' does not exist");
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
            if (string.IsNullOrEmpty(request.Path) || request.Path.Split('/').Length <= 2)
                throw new ArgumentException($"Failed to delete user");

            string[] pathParts = request.Path.Split('/');

            string username = pathParts[2];
            UserRepository dbUser = new();
            int rowsAffected = dbUser.Delete(username);
            if (rowsAffected == 0)
                throw new KeyNotFoundException($"User '{username}' does not exist");
            return new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = "User deleted successfully" };
        }

        private ResponseFormat CreatePackage(HttpRequest request)
        {
            var dPackage = JsonSerializer.Deserialize<List<Card>>(request.Body.ToString()) ?? throw new ArgumentException($"Failed to create package");
            PackageService packageService = new();
            packageService.Add(new Package(dPackage, Guid.NewGuid().ToString()));
            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "Package created successfully" };
        }

        private ResponseFormat GetPackages(HttpRequest request)
        {
            PackageRepository dbPackage = new();
            ResponseFormat response = new() { Status = (int)HTTPStatusCode.OK };
            string? query = ExtractPath(request.Path ?? string.Empty);
            if (query != null)
            {
                Card? package = dbPackage.Get(query) ?? throw new KeyNotFoundException($"Package '{query}' does not exist");
                response.Body = JsonSerializer.Serialize(package);
            }
            else
            {
                var packages = dbPackage.GetAll();
                response.Body = JsonSerializer.Serialize(packages);
            }
            return response;
        }

        private ResponseFormat UpdateUser(HttpRequest request)
        {
            var body = JsonSerializer.Deserialize<User>(request.Body.ToString());
            if (string.IsNullOrEmpty(request.Path) || request.Path.Split('/').Length <= 2 || body == null)
                throw new ArgumentException("Failed to update user");

            string[] pathParts = request.Path.Split('/');
            string username = pathParts[2];

            UserRepository dbUser = new();
            var user = dbUser.Get(username) ?? throw new KeyNotFoundException($"User '{username}' does not exist");
            ChangeUserProperties(user, body);
            dbUser.Update(username, user);

            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "User updated successfully" };
        }

        private static void ChangeUserProperties(User user, User body)
        {
            bool propertiesChanged = false;
            if (!string.IsNullOrEmpty(body.Password))
            {
                user.ChangePassword(BCrypt.Net.BCrypt.EnhancedHashPassword(body.Password));
                propertiesChanged = true;
            }
            if (!string.IsNullOrEmpty(body.Username))
            {
                UserRepository dbUser = new();
                if(dbUser.Get(body.Username) != null) throw new DuplicateNameException($"Username '{body.Username}' is already taken");
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

        private static void SendResponse(HttpResponse response, int status, string body)
        {
            response.Status = status.ToString();
            response.Body = string.IsNullOrEmpty(body) ? "" : body;
            response.Send();
            Console.WriteLine("Body: " + response.Body);
        }
        public static string? ExtractPath(string url)
        {
            var uri = new Uri("http://dummy.com" + url);
            var path = uri.AbsolutePath;

            // Entferne den führenden Slash
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            // Nimm das letzte Segment nach dem letzten '/'
            var segments = path.Split('/');
            return segments.Length > 0 ? segments[^1] : null;
        }
    }
}
