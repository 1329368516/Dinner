using Dinner.Base.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.Base.Service
{
    public abstract class BaseService<T, M> : IBaseService<T> where T : BaseModel, new() where M : IRepositoryBase<T>
    {
        protected M _Repository;
        protected RedisHelper _RedisHelper;
        public BaseService(M Repository, RedisHelper redis)
        {
            _RedisHelper = redis;
            _Repository = Repository;
        }

        public Task<T> Get(T t)
        {
            T record = queryCache(t.Id.ToString());
            if (record == null)
            {
                record = _Repository.QueryByID(t.Id).Result;
                insertCache(record);
            }
            return Task.Run(() => record);
        }  

        public Task<bool> LogicDelete(List<Guid> ids)
        {
            return Task.Run<bool>(async () =>
          {
              foreach (var item in ids)
              {
                  T t = new T();
                  t.Id = item;
                  t.IsDelete = true;
                  await this._Repository.Update(t);
                  this.deleteCache(t);
                  await this.Get(t);
              }
              return true;
          }
                );
        }


        public Task<T> Add(T t)
        {
            t.Id = Guid.NewGuid();
            t.CreateDate = DateTime.Now;
            t.IsDelete = false;
            this.deleteCache(t);
            _Repository.Add(t);
            insertCache(t);
            return Task.Run(() => t);
        }

        public Task<T> Update(T t)
        {
            this.deleteCache(t);
            _Repository.Update(t);
            insertCache(t);
            return Task.Run(() => t);
        }




        //根据Key获取一条数据
        protected T queryCache(string Key)
        {
            return _RedisHelper.StringGet<T>(Key);
        }
        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>

        protected bool insertCache(T record)
        {
            if (record == null)
            {
                return false;
            }
            return _RedisHelper.StringSet<T>(record.Id.ToString(), record, null);
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="record"></param>
        protected void deleteCache(T record)
        {
            if (record == null)
            {
                return;
            }
            _RedisHelper.KeyDelete(record.Id.ToString());
        }


    }
}
