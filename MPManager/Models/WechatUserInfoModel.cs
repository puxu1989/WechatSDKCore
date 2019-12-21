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
        public string unionId { get; set; }//小程序必须绑定在微信开放平台上，不绑定是没有的unionId（PS：绑定开放平台需要开发者资质认证，认证收费的奥）
        //同一个微信开放平台下的相同主体的 App、公众号、小程序，如果用户已经关注公众号，或者曾经登录过App或公众号，则用户打开小程序时，开发者可以直接通过 wx.login 获取到该用户UnionID，无须用户再次授权。（解读：用户如果没有登录过app，也没有登录过公众号，也没有关注过公众号的情况下，小程序中通过 wx.login 是获取不到 unionid的
        public Watermark watermark { get; set; }

        public class Watermark
        {
            public string appid { get; set; }
            public string timestamp { get; set; }
        }
    }
}
