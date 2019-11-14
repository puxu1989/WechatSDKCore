using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.Commons.Models
{
    class WxPayException:Exception
    {
        public WxPayException(string msg) : base(msg)
        {
        }
    }
}
