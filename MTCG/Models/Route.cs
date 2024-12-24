using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Route
    {
        public string Path { get; }
        public string Method { get; }
        public AuthorizationTypes AuthorizationType { get; private set; }

        public string Role { get; private set; }

        public string? Token { get; private set; }

        public string? Holder { get; private set; }
        private readonly Regex _pathRegex;
        public Dictionary<string, string> PathVariables { get; set; } = new Dictionary<string, string>();

        public Route(string path, string method, AuthorizationTypes authorization = AuthorizationTypes.All)
        {
            Path = path;
            Method = method;
            AuthorizationType = authorization;

            _pathRegex = new Regex("^" + Regex.Replace(path, @"\{(\w+)\}", @"(?<$1>[^/]+)") + "$");

            if (!Matches(path))
                PathVariables = new Dictionary<string, string>();
        }

        public bool Matches(string incomingPath)
        {
            var match = _pathRegex.Match(incomingPath);
            if (!match.Success)
                return false;

            PathVariables = _pathRegex.GetGroupNames()
                .Where(name => name != "0") // "0" ist der gesamte Match
                .ToDictionary(name => name, name => match.Groups[name].Value);

            return true;
        }

        public override string ToString()
        {
            return $"[{Method}] {Path}";
        }

        public void SetHolder(string holder)
        {
            Holder = holder;
        }

        public void SetToken(string token)
        {
            Token = token;
        }

        public void SetRole(string role)
        {
            Role = role;
        }

        public void SetAuthorizationType(AuthorizationTypes auth)
        {
            AuthorizationType = auth;
        }
    }

}
