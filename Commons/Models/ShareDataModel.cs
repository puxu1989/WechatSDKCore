using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.Commons.Models
{
    public class ShareDataModel
    {
        public string AppId { get; set; }// 必填，公众号的唯一标识
        public long Timestamp { get; set; }// 必填，生成签名的时间戳
        public string NonceStr { get; set; } // 必填，生成签名的随机串
        public string Signature { get; set; }// 必填，签名
    }
}
