using MTCG.Database;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class RegisterService
    {
        private readonly LoginService _loginService = new LoginService();
        public string Register(string name, string password)
        {
            var user = MTCGData.getUser(name.Trim());
            if(user != null) throw new InvalidOperationException("User already exists");
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password)) 
                throw new ArgumentException("Username or password cannot be empty.");
            MTCGData.addUser(name, password);
            return _loginService.Login(name, password);
        }

    }
}
