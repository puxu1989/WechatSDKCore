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
        [Description("H5")]//尚未使用过
        H5
    }
}
