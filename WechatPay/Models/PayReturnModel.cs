
using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.WechatPay.Models
{
    /// <summary>
    ///JSAPI和小程序 统一支付接口的 返回的Model
    /// </summary>
    public class PayReturnModel
    {
        public string timeStamp { get; set; }
        public string nonceStr { get; set; }
        public string package { get; set; }
        public string signType { get; set; }
        public string paySign { get; set; }
        public string appId { get; set; }//公众号需要使用
    }
}
