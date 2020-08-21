
using Dinner.Base.Repository;
using Dinner.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dinner.DAL.IRepository
{
    public interface IUserRepository : IRepositoryBase<UsersModel> 
    {
        Task<UsersModel> GetUsers();
    }
}
