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
            AppVersion appVersion = null;
            var json = ReadJsonFile("appVersion.json");
            appVersion = JsonMapper.ToObject<AppVersion>(json);
            
            return new AppVersionResponse
            {
                Version=appVersion.Version,
                Content= appVersion.Content
            };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("App/HomeBanner")]
        public BaseApiResponse HomeBanner()
        {
            HomeBanners homeBanners = null;
            var json = ReadJsonFile("homeBanners.json");
            homeBanners = JsonMapper.ToObject<HomeBanners>(json);

            return new HomeBannerResponse
            {
                Banners = homeBanners.Banners.Select(x=>new HomeBanner {
                    Img=x.Img,
                    Link=x.Link
                }).ToList()
            };
        }

        private string ReadJsonFile(string fileName)
        {
            string json = string.Empty;
            //读取json文件
            var jsonFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Json");
            jsonFilePath += "/"+ fileName;

            StreamReader streamReader = null;
            if (!File.Exists(jsonFilePath))
            {
                throw new Exception("没有找到json文件");
            }
            try
            {
                streamReader = new StreamReader(jsonFilePath);
                json = streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                streamReader.Close();
            }

            return json;
        }
    }
}