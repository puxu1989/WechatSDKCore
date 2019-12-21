using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.AuthManager.Models
{
    /// <summary>
    /// 通过access_token获取用户信息的返回结果
    /// </summary>
    public class AuthUserInfoModel
    {
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public string unionid { get; set; }
    }
}
