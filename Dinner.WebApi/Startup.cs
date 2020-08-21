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
            //services.Configure<MemoryCacheEntryOptions>(options => options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5));//���û�����Чʱ��Ϊ5����
            #region JWT��֤

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//�Ƿ���֤Issuer
                        ValidateAudience = true,//�Ƿ���֤Audience
                        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                        ValidAudience = AppSettings.AdminConfig.Audience,//Audience
                        ValidIssuer = AppSettings.AdminConfig.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.AdminConfig.JwtSecurityKey))//�õ�SecurityKey
                    };
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
                options.OperationFilter<SecurityRequirementsOperationFilter>();

                // �������ơ�Blog.Core�����Զ��壬����һ�¼���
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization", // jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header, // jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
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
            //ע��ʵ��
            builder.RegisterAssemblyTypes(service).Where(t => t.Name.EndsWith("Model")).AsImplementedInterfaces();
             
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


            app.UseRouting();
            //��Ȩ
            app.UseAuthentication();
            //��֤
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
