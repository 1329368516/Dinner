using Dinner.Base;
using Dinner.Base.Model;
using Dinner.BLL.UserService;
using Dinner.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Dinner.WebApi.Controllers
{

    [Route("api/[controller]")]
    public class UsersController : BaseController<Users, IUserService>
    {
        private readonly IUserService userService;
        public UsersController(IUserService _userService) : base(_userService)
        {
            userService = _userService;
        }
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns> 
        [HttpGet("GetUsers")]
        public async Task<JsonResult> GetUsers()
        {
            return Json(new SuccessResultModel<Users>(await userService.GetUsers()));
        }

        [HttpPut("Upload/{id}")]
        public async Task<ActionResult> upload(string id, IFormFile file)
        {
            return Json(await userService.GetUsers());
        }




    }
}