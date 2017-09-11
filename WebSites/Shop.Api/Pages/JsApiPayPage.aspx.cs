using Shop.Api.WxPayAPI;
using Shop.Api.WxPayAPI.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Shop.Api.Pages
{
    public partial class JsApiPayPage : System.Web.UI.Page
    {
        //public static string wxJsApiParam { get; set; } //H5调起JS API参数

        protected void Page_Load(object sender, EventArgs e)
        {
            GetWxJsApiParam(Request.Params["total_fee"]);
        }
        /// <summary>
        /// 从微信服务器获取OpenId
        /// </summary>
        /// <returns></returns>
        private string GetOpenId()
        {
            string openId = "";
            JsApiPay jsApiPay = new JsApiPay(this);
            try
            {
                //调用【网页授权获取用户信息】接口获取用户的openid和access_token
                jsApiPay.GetOpenidAndAccessToken();
                openId = jsApiPay.openid;
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().ToString(), "从微信获取OpenId,页面传参出错,请返回重试");
            }
            return openId;
        }

        /// <summary>
        /// 返回前端js 支付需要的支付参数
        /// </summary>
        public void GetWxJsApiParam(string total_fee)
        {
            string wxJsApiParam = string.Empty;

            string openid = GetOpenId();
            //检测是否给当前页面传递了相关参数
            if (string.IsNullOrEmpty(openid) || string.IsNullOrEmpty(total_fee))
            {
                Log.Error(this.GetType().ToString(), "This page have not get params, cannot be inited, exit...");
                Response.Write("<span style='color:#FF0000;font-size:20px'>" + "页面传参出错,请返回重试" + "</span>");
            }

            //若传递了相关参数，则调统一下单接口，获得后续相关接口的入口参数
            JsApiPay jsApiPay = new JsApiPay(this);
            jsApiPay.openid = openid;
            jsApiPay.total_fee = int.Parse(total_fee);

            //JSAPI支付预处理
            try
            {
                WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult();
                wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
                Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
                //在页面上显示订单信息
                //Response.Write("<span style='color:#00CD00;font-size:20px'>订单详情：</span><br/>");
                //Response.Write("<span style='color:#00CD00;font-size:20px'>" + unifiedOrderResult.ToPrintStr() + "</span>");

            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:#FF0000;font-size:20px'>" + "下单失败，请返回重试" + "</span>");
            }
            Response.Write(wxJsApiParam);
        }

        
    }
}