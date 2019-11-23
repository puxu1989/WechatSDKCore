using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.WechatPay.Models
{
  public  class PayBankReturnModel
    {
       public string partner_trade_no { get; set; }
       public string payment_no { get; set; }//微信企业付款单号	payment_no
       public decimal  cmms_amt { get; set; }//手续费金额(元)	微信返回的单位是分
    }
}
