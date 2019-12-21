using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.AuthManager.Models
{
    /// <summary>
    /// 授权的AccessToken 
    /// </summary>
    public class AuthAccessTokenModel
    {
        public string access_token { get; set; }//此access_token与基础支持的access_token不同
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }
}
