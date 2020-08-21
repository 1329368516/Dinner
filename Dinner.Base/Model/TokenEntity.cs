using System;
using System.Collections.Generic;
using System.Text;

namespace Dinner.Base.Model
{
    public class TokenEntity
    {

        public string JwtKeyName { get; set; }
         
        public string Issuer { get; set; }
        public string Audience { get; set; }

        public string JwtSecurityKey { get; set; }

        public Guid AdminRoleID { get; set; }

        public Guid SysMenuID { get; set; }

        public string ConnstringSql { get; set; }

        public string ConnectionRedisString { get; set; }






    }
}