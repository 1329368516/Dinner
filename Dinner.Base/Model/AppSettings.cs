

using Microsoft.Extensions.Configuration;
using System.IO;

namespace Dinner.Base.Model
{
    public class AppSettings
    { /// <summary>
      /// 配置文件的根节点
      /// </summary>
        private static IConfigurationRoot _config { get; set; }

        static string contentPath { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        static AppSettings()
        {
            // 加载appsettings.json，并构建IConfigurationRoot
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json", true, true);
            _config = builder.Build();
        }


        //public AppSettings(string contentPath)
        //{
        //    string Path = "appsettings.json";

        //    //如果你把配置文件 是 根据环境变量来分开了，可以这样写
        //    //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

        //    _config = new ConfigurationBuilder()
        //       .SetBasePath(contentPath)
        //       .Add(new JsonConfigurationSource { Path = Path, Optional = false, ReloadOnChange = true })//这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
        //       .Build();

        //}





        /// <summary>
        /// EnableDb
        /// </summary>
        public static string EnableDb => _config["ConnectionStrings:Enable"];

        /// <summary>
        /// ConnectionStrings
        /// </summary>
        public static string ConnectionStrings => _config.GetConnectionString(EnableDb);



        public static string ApiVersion => _config["ApiVersion"];


        public static string ConnstringSql => _config["ConnstringSql"];




        public static string redisConfiguration => _config.GetSection("RedisCaching").Value;







        public static TokenEntity AdminConfig { get; set; }






        //public static List<T> app<T>(params string[] sections)
        //{
        //    List<T> list = new List<T>();
        //    _config.Bind(string.Join(":", sections), list);
        //    return list;
        //}




        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        //public static string app(params string[] sections)
        //{
        //    try
        //    {

        //        if (sections.Any())
        //        {
        //            return _config[string.Join(":", sections)];
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }

        //    return "";
        //}


        // public static TokenEntity TokenEntity => _config["AppConfig"];





    }
}