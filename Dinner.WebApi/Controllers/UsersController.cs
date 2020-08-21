using Castle.Core.Logging;
using Dinner.Base;
using Dinner.Base.Model;
using Dinner.BLL.UserService;
using Dinner.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dinner.WebApi.Controllers
{

    [Route("api/[controller]/[Action]")]
    public class UsersController : BaseController<UsersModel, IUserService>
    {
        private readonly IUserService userService;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        public UsersController(IUserService _userService, ILogger<UsersController> _logger) : base(_userService)
        {
            logger = _logger;
            userService = _userService;
        }
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GetUsers()
        {
            logger.LogInformation("记录错误日志！");
            return Ok(new SuccessResultModel<UsersModel>(userService.GetUsers().Result));
        }

        [HttpPut]
        public async Task<ActionResult> upload(string id, IFormFile file)
        {
            return Json(await userService.GetUsers());
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(string name, string pass)
        {
            string jwtStr = string.Empty;
            bool suc = false;
            // 获取用户的角色名，请暂时忽略其内部是如何获取的，可以直接用 var userRole="Admin"; 来代替更好理解。
            var userRole = "admin";
            if (userRole != null)
            {
                // 将用户id和角色名，作为单独的自定义变量封装进 token 字符串中。
                TokenModelJwt tokenModel = new TokenModelJwt { Uid = 1, Role = userRole };
                // 登录，获取到一定规则的 Token 令牌
                jwtStr = JwtHelper.IssueJwt(tokenModel);
                suc = true;
            }
            else
            {
                jwtStr = "login fail!";
            }
            return Ok(new
            {
                success = suc,
                token = jwtStr,
            });
        }


    }
}