using Shop.Api.WxPayAPI.lib;

namespace Shop.Api.WxPayAPI
{
    public class ApiPay
    {
        public int total_fee { get; set; }

        public ApiPay(int totalFee)
        {
            total_fee = totalFee;
        }
        public string GeneratePrepayId()
        {
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", "test");
            data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());
            data.SetValue("total_fee", total_fee);
            data.SetValue("trade_type", "APP");

            WxPayData result = WxPayApi.UnifiedOrder(data);
            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                Log.Error(this.GetType().ToString(), "UnifiedOrder response error!");
                throw new WxPayException("UnifiedOrder response error!");
            }

            return result.GetValue("prepay_id").ToString();
        }
        
    }
}