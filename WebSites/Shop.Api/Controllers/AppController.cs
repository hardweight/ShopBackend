using LitJson;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Apps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Shop.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class AppController:ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("App/CheckVersion")]
        public BaseApiResponse CheckVersion()
        {
            //读取json文件
            var jsonFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Json");
            jsonFilePath += "/appVersion.json";

            AppVersion appVersion = null;
            StreamReader streamReader = null;
            if (!File.Exists(jsonFilePath))
            {
                return new BaseApiResponse { Code = 400, Message = "没有json文件" };
            }

            try
            {
                streamReader = new StreamReader(jsonFilePath);
                string json = streamReader.ReadToEnd();
                appVersion = JsonMapper.ToObject<AppVersion>(json);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                streamReader.Close();
            }
            return new AppVersionResponse
            {
                Version=appVersion.Version,
                Content= appVersion.Content
            };
        }
    }
}