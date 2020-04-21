using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.AuthManager.Models
{
   public class ProviderAuthCodeDto
    {
        public string ProviderName { get; set; } = "WechatPub"; //目前支持WechatApp,WechatPub 如果是APP传WechatApp
        public string Code { get; set; }
    }
}
