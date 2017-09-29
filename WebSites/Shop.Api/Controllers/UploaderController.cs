using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Uploader;
using Shop.Api.Utils.Oss;
using Shop.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class UploaderController:ApiController
    {

        /// <summary>
        /// 单文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Uploader/SingleFileUpload")]
        [AllowAnonymous]
        public BaseApiResponse SingleFileUpload()
        {
            string ossHost = "http://goodsdetails.wftx666.com";
            string ossFileName = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var postedFile = httpRequest.Files[0];
                var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + postedFile.FileName);
                //保存到本地
                postedFile.SaveAs(filePath);
                //上传到Oss
                ossFileName=OssUploader.PutObjectFromFile("wftx-goods-img-details","goodsDetails", filePath);
            }
            else
            {
                return new BaseApiResponse { Code = 400, Message = "没有文件" };
            }
            return new SingleFileUploadResponse { Url= ossHost + "/"+ossFileName.ToOssStyleUrl(OssImageStyles.GoodsDetailPic.ToDescription()) };
        }
    }
}