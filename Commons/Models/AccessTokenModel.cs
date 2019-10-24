using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.Commons.Models
{
   public class AccessTokenModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }//单位 秒
    }
}
