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

        //�޸ķ���ֵΪIServiceProvider��ʹ��Autofac����ע������\

        //public void ConfigureContainer(ContainerBuilder builder)
        //{
        //    //Autofac �Զ�ע��
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
            //services.Configure<MemoryCacheEntryOptions>(options => options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5));//���û�����Чʱ��Ϊ5����
            #region JWT��֤

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

            #region  ���SwaggerUI

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Dinner API�ӿ��ĵ�",
                    Version = "v1",
                    Description = "RESTful API for Dinn er",
                    Contact = new OpenApiContact { Name = "guojiahui", Email = "gxy_jh@yeah.net" }
                }); ;
                options.IgnoreObsoleteActions();
                options.DocInclusionPredicate((docName, description) => true);

                #region Swagger����ʾ������������ע��
                // ��ȡxml�ļ���
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // ��ȡxml�ļ�·��
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ��ӿ�������ע�ͣ�true��ʾ��ʾ������ע��
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





            #region ����ע��

            var builder = new ContainerBuilder();//ʵ��������
            //ע������ģ��module
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            //��ȡ���еĳ���
            //var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            //var assemblys = RuntimeHelper.GetAllAssemblies().ToArray();

            var repository = Assembly.Load("Dinner.DAL");
            var service = Assembly.Load("Dinner.BLL");

            //ע��ִ�������IRepository�ӿڵ�Repository��ӳ��
            builder.RegisterAssemblyTypes(repository).Where(t => t.Name.EndsWith("Repository") && !t.Name.StartsWith("I")).AsImplementedInterfaces();

            //ע���������IApplicationService��ApplicationService��ӳ��
            builder.RegisterAssemblyTypes(service).Where(t => t.Name.EndsWith("Service") && !t.Name.StartsWith("I")).AsImplementedInterfaces();
            //builder.RegisterAssemblyTypes(assemblys).Where(t => t.Name.EndsWith("AppService") && !t.Name.StartsWith("I")).AsImplementedInterfaces();


            builder.Populate(services);
            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer); //������IOC�ӹ� core����DI���� 
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
            #region ʹ��SwaggerUI

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dinner API V1");
            });

            #endregion

            app.UseRouting();
            //��Ȩ
            app.UseAuthentication();
            //��֤
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



        }
    }
}
