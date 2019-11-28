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

        public int amount { get; set; }//金额限制由商户平台不同而不同 默认最低0.3 最高5000

        public string partner_trade_no { get; set; }//商户订单号

        public string re_user_name { get; set; }//收款用户名

        public string spbill_create_ip { get; set; }//可以为客户端或者服务端的IP
        public string desc { get; set; }//必传 理赔 提现 转账等描述

        public PackageParamModel GetApiParameters()
        {
            PackageParamModel apiParam = new PackageParamModel();
            apiParam.AddValue("partner_trade_no", partner_trade_no);
            apiParam.AddValue("openid", openid);
            apiParam.AddValue("check_name", "NO_CHECK");//如果是FORCE_CHECK re_user_name必须为真实姓名
            apiParam.AddValue("amount", amount);
            apiParam.AddValue("desc", desc);
            apiParam.AddValue("spbill_create_ip", spbill_create_ip);
            apiParam.AddValue("re_user_name", re_user_name);
            return apiParam;
        }
    }
}
