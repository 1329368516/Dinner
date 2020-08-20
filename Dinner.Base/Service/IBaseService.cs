using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.Base.Service
{
    public interface IBaseService<T> where T : BaseModel
    {
        /// <summary>
        /// 根据ID获取单条记录
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<T> Get(T t); 
        /// <summary>
        /// 根据ID逻辑删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> LogicDelete(List<Guid> ids);
        /// <summary>
        /// 根据实体新增
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<T> Add(T t);

        /// <summary>
        /// 单体更新，主键作为条件
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<T> Update(T t);



    }
}
