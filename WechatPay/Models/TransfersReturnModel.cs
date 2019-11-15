using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.WechatPay.Models
{
    /// <summary>
    /// 提现返回model 根据文档描述以下字段在return_code 和result_code都为SUCCESS的时候有返回
    /// </summary>
    public class TransfersReturnModel
    {
       public string partner_trade_no { get; set; }
        public string payment_no { get; set; }
        public DateTime payment_time { get; set; }
    }
}
