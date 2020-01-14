using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.AuthManager.Models
{
   public class AuthCodeDto
    {
        public string ProviderName { get; set; }  //目前支持WechatApp,WechatPub
        public string Code { get; set; }
    }
}
