using MTCG.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class UserService
    {
        private readonly UserRepository? _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public UserService()
        {
            _userRepository = new UserRepository();
        }



    }
}
