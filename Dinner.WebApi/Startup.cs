using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dinner.Base;
using Dinner.Base.Filter;
using Dinner.Base.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace Dinner.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }
         
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            AppSettings.AdminConfig = Configuration.GetSection("AppConfig").Get<TokenEntity>();
            //services.RegisterAssembly("IServices");
            //services.Configure<MemoryCacheEntryOptions>(options => options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5));//设置缓存有效时间为5分钟
            #region JWT认证

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = AppSettings.AdminConfig.Audience,//Audience
                        ValidIssuer = AppSettings.AdminConfig.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.AdminConfig.JwtSecurityKey))//拿到SecurityKey
                    };
                });


            #endregion

            #region  添加SwaggerUI

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Dinner API接口文档",
                    Version = "v1",
                    Description = "RESTful API for Dinn er",
                    Contact = new OpenApiContact { Name = "guojiahui", Email = "gxy_jh@yeah.net" }
                }); ;
                options.IgnoreObsoleteActions();
                options.DocInclusionPredicate((docName, description) => true);

                #region Swagger中显示控制器方法的注释
                // 获取xml文件名
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // 获取xml文件路径
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 添加控制器层注释，true表示显示控制器注释
                options.IncludeXmlComments(xmlPath, true);
                options.OperationFilter<SecurityRequirementsOperationFilter>();

                // 方案名称“Blog.Core”可自定义，上下一致即可
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization", // jwt默认的参数名称
                    In = ParameterLocation.Header, // jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });



                #endregion

                //options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Dinner.WebApi.xml"));
                //options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Dinner.DAL.xml"));
                //options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Dinner.BLL.xml"));
                //options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Dinner.Base.xml"));

            });

            #endregion

            services.AddControllers(options =>
            {
                options.Filters.Add(new CustomerExceptionFilter());
            });


            services.AddSingleton(new RedisHelper());

            

            #region 依赖注入

            var builder = new ContainerBuilder();//实例化容器
            //注册所有模块module
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            //获取所有的程序集
            //var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            //var assemblys = RuntimeHelper.GetAllAssemblies().ToArray();

            var repository = Assembly.Load("Dinner.DAL");
            var service = Assembly.Load("Dinner.BLL");

            //注册仓储，所有IRepository接口到Repository的映射
            builder.RegisterAssemblyTypes(repository).Where(t => t.Name.EndsWith("Repository") && !t.Name.StartsWith("I")).AsImplementedInterfaces();
            //注册服务，所有IApplicationService到ApplicationService的映射
            builder.RegisterAssemblyTypes(service).Where(t => t.Name.EndsWith("Service") && !t.Name.StartsWith("I")).AsImplementedInterfaces(); 
            //注册实体
            builder.RegisterAssemblyTypes(service).Where(t => t.Name.EndsWith("Model")).AsImplementedInterfaces();
             
            builder.Populate(services);
            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer); //第三方IOC接管 core内置DI容器 
                                                                     //return services.BuilderInterceptableServiceProvider(builder => builder.SetDynamicProxyFactory());
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            #region 使用SwaggerUI


            app.UseRouting();
            //授权
            app.UseAuthentication();
            //认证
            app.UseAuthorization();


            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dinner API V1");
            });

            #endregion

          
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



        }
    }
}
