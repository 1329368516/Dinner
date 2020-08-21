using Castle.Core.Logging;
using Dinner.Base;
using Dinner.Base.Model;
using Dinner.BLL.UserService;
using Dinner.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Dinner.WebApi.Controllers
{

    [Route("api/[controller]")]
    public class UsersController : BaseController<UsersModel, IUserService>
    {
        private readonly IUserService userService;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        public UsersController(IUserService _userService,ILogger<UsersController> _logger) : base(_userService)
        {
            logger = _logger;
            userService = _userService;
        }
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns> 
        [HttpGet("GetUsers")]
        public async Task<JsonResult> GetUsers()
        { 
            logger.LogInformation("记录错误日志！");
            return Json(new SuccessResultModel<UsersModel>(await userService.GetUsers()));
        }

        [HttpPut("Upload/{id}")]
        public async Task<ActionResult> upload(string id, IFormFile file)
        {
            return Json(await userService.GetUsers());
        }
         

    }
}