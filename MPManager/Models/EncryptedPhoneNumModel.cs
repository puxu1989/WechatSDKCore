using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.MPManager.Models
{
    /// <summary>
    ///  微信小程序获取手机信息结构 
    /// </summary>
    public class EncryptedPhoneNumModel
    {     
        public string code { get; set; }
        public string encryptedData { get; set; }
        public string iv { get; set; }
        public bool IsBingding { get; set; }
    }
}
