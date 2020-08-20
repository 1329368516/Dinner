
using Dinner.Base.Repository;
using Dinner.DAL.Entities;
using Dinner.DAL.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dinner.Dapper.Repository
{
    public class UserRepository : RepositoryBase<Users>, IUserRepository
    {
        public Task<Users> GetUsers()
        {
            return this.QueryByID(Guid.Parse("DFFC3CB3-F712-4A65-BC35-4B6A18866A5C"));
        }
    }
}
