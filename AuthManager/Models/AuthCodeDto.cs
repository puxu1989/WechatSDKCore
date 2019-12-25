using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.AuthManager.Models
{
   public class AuthCodeDto
    {
        public string Code { get; set; }
        public bool IsAPP { get; set; }
    }
}
