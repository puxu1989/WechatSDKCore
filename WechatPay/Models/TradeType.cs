using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WechatSDKCore.WechatPay.Models
{
    public enum TradeType
    {
        [Description("JSAPI")]
        JSAPI=1,
        [Description("APP")]
        APP,
        [Description("MWEB")] 
        H5
    }
}
