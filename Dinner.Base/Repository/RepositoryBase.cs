using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.Base.Repository
{
    public abstract partial class RepositoryBase<TEntity> : DataBaseConfig<TEntity>, IRepositoryBase<TEntity> where TEntity : class, new()
    {

        /// <summary>
        /// 条件查询带分页
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<List<TEntity>, int, int>> FindList(Expression<Func<TEntity, bool>> condition, int pageIndex, int pageSize)
        {
            int totalNumber = 0;
            int totalCount = 0;
            var data = await Task.Run(() => Db.Queryable<TEntity>().Where(condition).ToPageList(pageIndex, pageSize, ref totalNumber, ref totalCount));
            return new Tuple<List<TEntity>, int, int>(data, totalNumber, totalCount);
        }


        /// <summary>
        /// 条件查询
        /// </summary>
        /// <returns></returns>
        public async Task<List<TEntity>> FindList(Expression<Func<TEntity, bool>> condition)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().WhereIF(condition != null, condition).ToList());
        }


        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Add(TEntity model)
        {
            var i = await Task.Run(() => Db.Insertable(model).ExecuteCommand());
            //返回的i是long类型,这里你可以根据你的业务需要进行处理
            bool isok = false;
            if (i > 0)
            {
                isok = true;
            }

            return isok;
        }

        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByIds(object[] ids)
        {
            var i = await Task.Run(() => Db.Deleteable<TEntity>().In(ids).ExecuteCommand());
            return i > 0;
        }

        /// <summary>
        /// 根据ID查询一条数据
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        public async Task<TEntity> QueryByID(object objId)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().InSingle(objId));
        }


        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity model)
        {
            //这种方式会以主键为条件
            var i = await Task.Run(() => Db.Updateable(model).ExecuteCommand());
            return i > 0;
        }



    }
}
