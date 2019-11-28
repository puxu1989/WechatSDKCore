using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.Commons.Models
{
   public class WxPayException:Exception
    {
        public WxPayException(string msg) : base(msg)
        {
        }
    }
}
