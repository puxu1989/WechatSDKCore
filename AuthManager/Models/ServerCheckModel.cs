using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.AuthManager.Models
{
    public  class ServerCheckModel
    {
        public string signature { get; set; }
        public string timestamp { get; set; }
        public string nonce { get; set; }
        public string echostr { get; set; }
    }
}
