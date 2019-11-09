using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.MPManager.Models
{
    /// <summary>
    /// 解密的phoneNumber
    /// </summary>
    public class PhoneNumModel
    {
        public string phoneNumber { get; set; }
        public string purePhoneNumber { get; set; }
        public string countryCode { get; set; }
        public Watermark watermark { get; set; }
        public class Watermark
        {
            public string appid { get; set; }
            public string timestamp { get; set; }
        }
    }
}
