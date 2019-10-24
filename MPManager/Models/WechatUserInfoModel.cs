using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.MPManager.Models
{
    /// <summary>
    /// 用户信息    小程序模型实体都是小写
    /// </summary>
    public  class WechatUserInfoModel
    {
        public string openId { get; set; }
        public string nickName { get; set; }
        public int gender { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string avatarUrl { get; set; }
        public string unionId { get; set; }
        public Watermark watermark { get; set; }

        public class Watermark
        {
            public string appid { get; set; }
            public string timestamp { get; set; }
        }
    }
}
