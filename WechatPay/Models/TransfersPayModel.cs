using System;
using System.Collections.Generic;
using System.Text;
using WechatSDKCore.Commons.Models;

namespace WechatSDKCore.WechatPay.Models
{
    /// <summary>
    /// 提现转账Model
    /// </summary>
    public class TransfersPayModel
    {
        public string openid { get; set; }

        public int amount { get; set; }

        public string partner_trade_no { get; set; }

        public string re_user_name { get; set; }

        public string spbill_create_ip { get; set; }

        public PackageParamModel GetTransfersApiParameters()
        {
            PackageParamModel apiParam = new PackageParamModel();
            apiParam.AddValue("partner_trade_no", partner_trade_no);
            apiParam.AddValue("openid", openid);
            apiParam.AddValue("check_name", "NO_CHECK");
            apiParam.AddValue("amount", amount);
            apiParam.AddValue("desc", "提现");
            apiParam.AddValue("spbill_create_ip", spbill_create_ip);
            apiParam.AddValue("re_user_name", re_user_name);
            return apiParam;
        }
    }
}
