using Dinner.Base.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dinner.Base
{
    public class BaseModel : IEntity<Guid?>
    {

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, ColumnName = "ID")]    //是主键, 还是标识列
        public Guid? Id { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [SugarColumn(ColumnName = "CreateDate")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 是否删除（0正常，1删除）
        /// </summary>
        [SugarColumn(ColumnName = "IsDelete")]
        public bool? IsDelete { get; set; }

    }
}
