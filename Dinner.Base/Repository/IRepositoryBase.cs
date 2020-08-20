using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.Base.Repository
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {

        /// <summary>
        /// 条件查询带分页
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<Tuple<List<TEntity>, int, int>> FindList(Expression<Func<TEntity, bool>> condition, int pageIndex, int pageSize); 
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<List<TEntity>> FindList(Expression<Func<TEntity, bool>> condition);

        /// <summary>
        /// 根据ID查询实体数据
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<TEntity> QueryByID(object objId);


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Add(TEntity model);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Update(TEntity model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> DeleteByIds(object[] ids);
    }
}
