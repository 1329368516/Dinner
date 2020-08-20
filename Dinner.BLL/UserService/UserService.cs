using Dinner.Base;
using Dinner.Base.Service;
using Dinner.DAL.Entities;
using Dinner.DAL.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.BLL.UserService
{
    public class UserService : BaseService<Users, IUserRepository>, IUserService
    {
        private readonly IUserRepository userRepository;
        public UserService(IUserRepository _userRepository, RedisHelper redis) : base(_userRepository, redis)
        {
            userRepository = _userRepository;

        }

        public Task<Users> GetUsers()
        { 
            return userRepository.GetUsers();
        }

        

    }
}
