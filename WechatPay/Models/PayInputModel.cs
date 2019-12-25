using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.WechatPay.Models
{
    /// <summary>
    /// 支付输入参数 由外部传入 
    /// </summary>
   public class PayInputModel
    {
        public string OrderId { get; set; }//订单号 
        public string OpenId { get; set; }//用户的OpenId  可为空 JSAPI必传
        public decimal TotalAmount { get; set; } //支付金额 单位就是元 支付内部使用分 int类型
        public string Body { get; set; } //商品描述（长度127）
        public string Attach { get; set; }//自定义参数怎么传过去 原封不动返回来（长度127 不能太长 慎用）
        public TradeType TradeType { get; set; }
    }
}
