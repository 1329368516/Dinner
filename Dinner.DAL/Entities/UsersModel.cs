using Dinner.Base;
using SqlSugar;
using System;

namespace Dinner.DAL.Entities
{
    [SugarTable("T_User")]
    public class UsersModel : BaseModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(ColumnName = "UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(ColumnName = "UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 性别（0女，1男）
        /// </summary>
        [SugarColumn(ColumnName = "Gender")]
        public int? Gender { get; set; }

        /// <summary>
        /// 出生年月日
        /// </summary>
        [SugarColumn(ColumnName = "Birthday")]
        public DateTime? Birthday { get; set; }

 

 
    }
}
