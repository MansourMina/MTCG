using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Route
    {
        public string Path { get; }
        public string Method { get; }

        public AuthorizationTypes Authorization { get; }

        public Route(string path, string method, AuthorizationTypes? authorization)
        {
            Path = path;
            Method = method;
            Authorization = (AuthorizationTypes)authorization;
        }

        public override string ToString()
        {
            return $"[{Method}] {Path}";
        }
    }
}
