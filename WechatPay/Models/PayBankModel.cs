using PXLibCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using WechatSDKCore.Commons.Models;

namespace WechatSDKCore.WechatPay.Models
{
    /// <summary>
    /// 企业转账到银行卡
    /// </summary>
    public class PayBankModel
    {
        public string partner_trade_no { get; set; }
        public string BankNo { get; set; }//明文收款银行卡号 这里提交请求时需要采用标准RSA算法，公钥由微信侧提供
        public string RealName { get; set; }//明文真实姓名  这里提交请求时需要采用标准RSA算法，公钥由微信侧提供
        public string BankCode { get; set; }//收款方开户行 该code从微信支持的提现银行来
        public decimal Amount { get; set; }//付款金额：RMB 元（大于0 支付总额，不含手续费）提交请求时需要转换成=>分
        public string desc { get; set; }//操作描述

        public PackageParamModel GetApiParameters(string pubKey)
        {
            PackageParamModel apiParam = new PackageParamModel();
            apiParam.AddValue("partner_trade_no", partner_trade_no);
            apiParam.AddValue("enc_bank_no", SecurityHelper.RSAEncrypt(BankNo, pubKey,false));    
            apiParam.AddValue("enc_true_name", SecurityHelper.RSAEncrypt(RealName, pubKey, false));
            apiParam.AddValue("bank_code", BankCode);
            apiParam.AddValue("desc", desc);
            apiParam.AddValue("amount", Convert.ToInt32(Amount*100));
            return apiParam;
        }
    }
}
