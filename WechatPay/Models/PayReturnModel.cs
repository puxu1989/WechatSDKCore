
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
        public string prepayId { get; set; }//APP支付prepayId赋值给此字段 且package=Sign = WXPay，区别
        public string partnerId { get; set; }//APP支付使用
    }
}
