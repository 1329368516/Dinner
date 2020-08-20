using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dinner.Base;
using Dinner.Base.Model; 
using Dinner.WebApi.Models;
using Dinner.WebApi.Unit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

        //修改返回值为IServiceProvider，使用Autofac依赖注入容器\

        //public void ConfigureContainer(ContainerBuilder builder)
        //{
        //    //Autofac 自动注入
        //    Assembly service = Assembly.Load("Dinner.BLL");
        //    Assembly repository = Assembly.Load("Dinner.DAL");
        //    builder.RegisterAssemblyTypes(service).Where(t => t.Name.EndsWith("Service") && !t.Name.StartsWith("I")).AsImplementedInterfaces();
        //    builder.RegisterAssemblyTypes(repository).Where(t => t.Name.EndsWith("Repository") && !t.Name.StartsWith("I")).AsImplementedInterfaces();
        //    builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency();

        //}
         
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            AppSettings.AdminConfig = Configuration.GetSection("AppConfig").Get<TokenEntity>();
            //services.RegisterAssembly("IServices");
            //services.Configure<MemoryCacheEntryOptions>(options => options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5));//设置缓存有效时间为5分钟
            #region JWT认证

            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            JwtSettings setting = new JwtSettings();
            Configuration.Bind("JwtSettings", setting);
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = setting.Audience,
                    ValidIssuer = setting.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey))
                };
                /*
                config.SecurityTokenValidators.Clear();
                config.SecurityTokenValidators.Add(new MyTokenValidate());
                config.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["myToken"];
                        context.Token = token.FirstOrDefault();
                        return Task.CompletedTask;
                    }
                };
                */
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
                #endregion
                 
                //options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Dinner.WebApi.xml"));
                //options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Dinner.DAL.xml"));
                //options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Dinner.BLL.xml"));
                //options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Dinner.Base.xml"));
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            #endregion

            services.AddControllers();
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
            //builder.RegisterAssemblyTypes(assemblys).Where(t => t.Name.EndsWith("AppService") && !t.Name.StartsWith("I")).AsImplementedInterfaces();


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

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dinner API V1");
            });

            #endregion

            app.UseRouting();
            //授权
            app.UseAuthentication();
            //认证
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



        }
    }
}
