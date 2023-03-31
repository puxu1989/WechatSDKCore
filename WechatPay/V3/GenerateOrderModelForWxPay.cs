using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WechatSDKCore.WechatPay.V3
{
    public class GenerateOrderModelForWxPay
    {
        public string appid { get; set; }
        public string mchid { get; set; }
        public string description { get; set; }
        public WxPayAmountModel amount { get; set; }
        public string out_trade_no { get; set; }
        public string notify_url { get; set; }
        public Payer Payer { get; set; }
    }
    public class WxPayAmountModel
    {
        public int total { get; set; }
        public string currency { get; set; } = "CNY";
    }
    public class Payer 
    { 
       public string openid { get; set; }
    }
}
