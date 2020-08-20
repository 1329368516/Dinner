using Dinner.Base.Service;
using Dinner.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.BLL.UserService
{
    public interface IUserService : IBaseService<Users>
    {
        Task<Users> GetUsers();

    }
}
