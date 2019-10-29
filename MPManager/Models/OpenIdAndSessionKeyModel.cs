using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.MPManager.Models
{

   public class OpenIdAndSessionKeyModel
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public int errcode { get; set; }//errcode=0成功
        public string errmsg { get; set; }
    }
}
