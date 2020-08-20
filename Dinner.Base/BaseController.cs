using Dinner.Base.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks; 

namespace Dinner.Base
{
    [ApiController]
    public abstract class BaseController<T, S> : Controller
        where T : BaseModel, new() where S : IBaseService<T>
    {

        private S service;

        public BaseController(S baseService)
        {
            service = baseService;
        }

        /// <summary>
        /// 根据ID 获取单条数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        //[HttpPost("Get")]
        [HttpPost("Get")]
        public async Task<T> Get(T t)
        {
            return await service.Get(t);
        }

        /// <summary>
        /// 根据ID 获取单条数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<T> Add(T t)
        {
            return await service.Add(t);
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns> 
        [HttpPost("LogicDelete")]
        public async Task<object> LogicDelete(List<Guid> ids)
        {
            return await service.LogicDelete(ids);
        }



        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns> 
        [HttpPost("Update")]
        public async Task<object> Update(T t)
        {
            return await service.Update(t);
        }
    }
}
