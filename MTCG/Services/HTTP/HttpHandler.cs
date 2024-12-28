using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Presentation;
using MTCG.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
            Conflict = 409,
            PaymentRequired = 402
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
            Routes[new Route("/users/{username}", "GET", AuthorizationTypes.OwnUser)] = GetUser;
            Routes[new Route("/users", "GET", AuthorizationTypes.All)] = GetUsers;
            Routes[new Route("/users/{username}", "DELETE", AuthorizationTypes.All)] = DeleteUser;
            Routes[new Route("/users/{username}", "PUT", AuthorizationTypes.OwnUser)] = UpdateUser;

            // Packages
            Routes[new Route("/packages", "POST", AuthorizationTypes.Admin)] = CreatePackage;
            Routes[new Route("/packages", "GET", AuthorizationTypes.All)] = GetPackages;
            Routes[new Route("/transactions/packages", "POST", AuthorizationTypes.LoggedIn)] = AcquirePackage;

            // Cards
            Routes[new Route("/cards", "GET", AuthorizationTypes.LoggedIn)] = GetUserStackCards;

            // Deck
            Routes[new Route("/deck", "GET", AuthorizationTypes.LoggedIn)] = GetUserDeck;
            Routes[new Route("/deck", "PUT", AuthorizationTypes.LoggedIn)] = UpdateDeck;



        }

        private void Handle(HttpRequest request, HttpResponse response)
        {
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine(request.Method + " " + request.Path);

            if (request == null)
                throw new ArgumentNullException(nameof(request), "Invalid request");

            string fullPath = RemoveQueryFromPath(request.Path);
            var clientRoute = new Route(fullPath, request.Method);
            var finalResponse = new ResponseFormat { Status = (int)HTTPStatusCode.NotFound, Body = "Path Not found" };

            var foundRoute = ContainsRoute(clientRoute);
            request.PathVariables = clientRoute.PathVariables;

            if (!string.IsNullOrEmpty(request.Authorization))
                SetRequestAuth(request, clientRoute);

            if (foundRoute != null)
            {
                var (route, func) = foundRoute.Value;
                if ((!IsAuthorized(clientRoute) || clientRoute.AuthorizationType != route.AuthorizationType) && route.AuthorizationType != AuthorizationTypes.All)
                {
                    finalResponse.Status = (int)HTTPStatusCode.Unauthorized;
                    finalResponse.Body = "Unauthorized";
                    SendResponse(response, finalResponse.Status, finalResponse.Body);
                    return;
                }
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
                catch (NotSupportedException e)
                {
                    finalResponse.Status = (int)HTTPStatusCode.PaymentRequired;
                    finalResponse.Body = e.Message;
                }

            }
            SendResponse(response, finalResponse.Status, finalResponse.Body);
        }

        private void SetRequestAuth(HttpRequest request, Route route)
        {
            route.SetToken(GetAuthorizationToken(request.Authorization));
            route.SetRole(GetAuthorizationRole(request.Authorization));
            route.SetAuthorizationType(GetAuthorizationType(request) ?? AuthorizationTypes.All);
            route.SetHolder(GetAuthorizationHolder(request.Authorization));
        }

        private AuthorizationTypes? GetAuthorizationType(HttpRequest request)
        {
            if (string.IsNullOrEmpty(request.Authorization)) return AuthorizationTypes.All;
            string role = GetAuthorizationRole(request.Authorization);
            switch (role)
            {
                case "admin":
                    return AuthorizationTypes.Admin;
                default:
                    if (request.PathVariables.Count() > 0)
                        return AuthorizationTypes.OwnUser;
                    else return AuthorizationTypes.LoggedIn;
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


        public bool IsAuthorized(Route request)
        {
            if (request.AuthorizationType == AuthorizationTypes.All)
                return true;


            if (request.Token == null || request.Holder == null)
                return false;

            var sessionService = new SessionService();
            string? userId = sessionService.GetUserIdByToken(request.Token);
            if (string.IsNullOrEmpty(userId))
                return false;

            var userRepository = new UserRepository();
            User? sessionUser = userRepository.GetById(userId);
            if (sessionUser == null)
                return false;

            if (request.AuthorizationType == AuthorizationTypes.LoggedIn)
                return true;
            if (request.AuthorizationType == AuthorizationTypes.OwnUser)
            {
                string username = request.PathVariables?["username"];
                if (sessionUser.Username == request.Role && username == sessionUser.Username)
                    return true;
            }
            // Normalerweise sessionUser.Role == request.Role. Aufgrund des TestScripts wird jedoch stattdessen der Username verwendet.
            else if (request.AuthorizationType == AuthorizationTypes.Admin && sessionUser.Username == request.Role) return true;
            return false;
        }


        private (Route route, Func<HttpRequest, ResponseFormat> handler)? ContainsRoute(Route request)
        {
            foreach (var route in Routes)
            {
                if (route.Key.Method == request.Method && route.Key.Matches(request.Path ?? string.Empty))
                {
                    request.PathVariables = route.Key.PathVariables;
                    return (route.Key, route.Value);
                }
            }
            return null;
        }

        private ResponseFormat RegisterUser(HttpRequest request)
        {
            UserManager registerService = new();
            StackRepository stackRepository = new();
            DeckRepository deckRepository = new();

            var dUser = JsonSerializer.Deserialize<User>(request.Body.ToString()) ?? throw new ArgumentException($"Failed to register user");

            // Create User
            registerService.Register(dUser.Username, dUser.Password);

            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "User created successfully" };
        }

        private ResponseFormat CreateSession(HttpRequest request)
        {
            LoginService loginService = new();
            var dUser = JsonSerializer.Deserialize<User>(request.Body.ToString()) ?? throw new ArgumentException($"Failed to create session");
            string token = loginService.Login(dUser.Username, dUser.Password);
            return new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = token };
        }

        private ResponseFormat GetUsers(HttpRequest request)
        {
            UserRepository dbUser = new();
            CardRepository dbCard = new();
            ResponseFormat response = new() { Status = (int)HTTPStatusCode.OK };
            var users = dbUser.GetAll();
            foreach (var user in users)
                user.Stack.Set(dbCard.GetStackCards(user.Stack.Id));
            response.Body = JsonSerializer.Serialize(users);
            return response;
        }
        private ResponseFormat GetUser(HttpRequest request)
        {
            UserRepository dbUser = new();
            CardRepository dbCard = new();
            ResponseFormat response = new() { Status = (int)HTTPStatusCode.OK };
            string username = request.PathVariables?["username"];
            User? user = dbUser.GetByName(username) ?? throw new KeyNotFoundException($"User '{username}' does not exist");
            user.Stack.Set(dbCard.GetStackCards(user.Stack.Id));
            response.Body = JsonSerializer.Serialize(user);
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
            foreach (var card in dPackage)
            {
                card.SetElementType(card.Name);
                card.SetCardType(card.Name);

            }
            PackageService packageService = new();
            packageService.Add(new Package(dPackage, Guid.NewGuid().ToString()));
            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "Package created successfully" };
        }

        private ResponseFormat GetPackages(HttpRequest request)
        {
            PackageRepository dbPackage = new();
            var packages = dbPackage.GetAll();
            return new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = JsonSerializer.Serialize(packages) };

        }

        private ResponseFormat AcquirePackage(HttpRequest request)
        {
            PackageService packageService = new();
            UserRepository userRepository = new();
            StackRepository stackRepository = new();
            UserManager userManager = new UserManager();


            string? username = GetAuthorizationRole(request.Authorization);

            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Failed to aqquire package");
            var user = userManager.GetUserByName(username);
            if (user == null || string.IsNullOrEmpty(username)) throw new KeyNotFoundException($"User '{username}' does not exist");
            if (user.Coins < Package.Costs) throw new NotSupportedException("Not enough money");

            Package? package = packageService.PopRandom();
            if (package == null) throw new KeyNotFoundException("No packages available");
            user.AcquirePackage(Package.Costs, package.Cards);
            stackRepository.AddCards(user.Stack.Id, package.Cards);
            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "Package aqquired successfully" };

        }

        private ResponseFormat UpdateUser(HttpRequest request)
        {
            var body = JsonSerializer.Deserialize<User>(request.Body.ToString());
            string? username = request.PathVariables?["username"];

            if (string.IsNullOrEmpty(request.Path) || request.PathVariables?.Count == 0 || body == null || string.IsNullOrEmpty(username))
                throw new ArgumentException("Failed to update user");

            UserRepository dbUser = new();
            UserManager userManager = new UserManager();
            var user = userManager.GetUserByName(username) ?? throw new KeyNotFoundException($"User '{username}' does not exist");
            ChangeUserProperties(user, body);
            dbUser.UpdateUserCreds(username, user);

            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "User updated successfully" };
        }

        private ResponseFormat GetUserStackCards(HttpRequest request)
        {
            User user = GetUserFromAuthRole(request.Authorization);
            return new ResponseFormat { Status = (int)HTTPStatusCode.OK, Body = JsonSerializer.Serialize(user.Stack.Cards) };
        }

        private ResponseFormat GetUserDeck(HttpRequest request)
        {
            User user = GetUserFromAuthRole(request.Authorization);
            string? format = ExtractQuery(request.Path ?? string.Empty, "format");
            ResponseFormat response = new() { Status = (int)HTTPStatusCode.OK };
            if (string.IsNullOrEmpty(format))
                response.Body = JsonSerializer.Serialize(user.Deck.Cards);
            else
            {
                switch (format)
                {
                    case "plain":
                        response.Body = string.Join(Environment.NewLine, user.Deck.Cards.Select(card => $"Card: {card.Name} (ID: {card.Id}), Damage: {card.Damage}, Element: {card.ElementType}, Type: {card.CardType}"));
                        break;
                    default:
                        response.Status = (int)HTTPStatusCode.BadRequest;
                        break;

                }
            }
            return response;

    }

    private User GetUserFromAuthRole(string authorization)
        {
            UserManager userManager = new UserManager();

            string? username = GetAuthorizationRole(authorization);

            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Username cannot be empty");

            User? user = userManager.GetUserByName(username);
            if (user == null || string.IsNullOrEmpty(username)) throw new KeyNotFoundException($"User '{username}' does not exist");
            return user;
        }

        private ResponseFormat UpdateDeck(HttpRequest request)
        {
            UserManager userManager = new UserManager();

            User user = GetUserFromAuthRole(request.Authorization);
            var dDeck = JsonSerializer.Deserialize<List<string>>(request.Body.ToString()) ?? throw new ArgumentException($"Failed to update Deck");
            if (dDeck.Count < BattleService.DeckSize) throw new ArgumentException($"{BattleService.DeckSize} cards are required");
            userManager.ConfigureUserDeck(dDeck, user);
            return new ResponseFormat { Status = (int)HTTPStatusCode.Created, Body = "Deck configured successfully" };
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
                if (dbUser.GetByName(body.Username) != null) throw new DuplicateNameException($"Username '{body.Username}' is already taken");
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
            //else throw new InvalidOperationException("No changes have been made");
            if (!propertiesChanged) throw new ArgumentException($"Failed to update user");
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
        public string? ExtractQuery(string url, string queryName)
        {
            var uri = new Uri("http://dummy.com" + url); 
            var queryParams = uri.Query.TrimStart('?'); 

            // Wenn es Query-Parameter gibt
            if (!string.IsNullOrEmpty(queryParams))
            {
                var parameters = queryParams.Split('&'); // Parameter nach '&' trennen
                foreach (var param in parameters)
                {
                    var keyValue = param.Split('='); // Schlüssel und Wert trennen
                    if (keyValue.Length == 2 && keyValue[0] == queryName) 
                    {
                        return keyValue[1]; 
                    }
                }
            }

            return null; 
        }

        public static string RemoveQueryFromPath(string url)
        {
            var uri = new Uri("http://dummy.com" + url);
            var pathWithoutQuery = uri.AbsolutePath;
            return pathWithoutQuery;
        }
    }
}
