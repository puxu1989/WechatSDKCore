using Newtonsoft.Json.Linq;
using PXLibCore.Extensions;
using PXLibCore.Extensions.Json;
using PXLibCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WechatSDKCore.MPManager.Models;

namespace WechatSDKCore.MPManager
{
   public class WechatMPManager
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public WechatMPManager(string appId,string appSecret) 
        {
            if (string.IsNullOrEmpty(appId))
                throw new Exception("AppId不能为空");
            this.AppId = appId;
            if (string.IsNullOrEmpty(appSecret))
                throw new Exception("ApSecret不能为空");
            this.AppSecret = appSecret;
        }
        /// <summary>
        /// auth.code2Session 小程序拉去加密的用户信息里的code换取session 
        /// </summary> 
        private async Task<OpenIdAndSessionKeyModel> GetOpenIdAndSessionKeyAsync(string jscode) 
        {
            string postUrl = $"https://api.weixin.qq.com/sns/jscode2session?appid={this.AppId}&secret={this.AppSecret}&js_code={jscode}&grant_type=authorization_code";
            string res = await WebHelper.HttpPostAsync(postUrl);
            return res.ToObject<OpenIdAndSessionKeyModel>();
        }
        /// <summary>  
        ///小程序登录第一步 小程序直接拉去加密的用户信息  根据微信小程序平台提供的解密算法解密数据，推荐直接使用此方法  
        /// </summary>  
        /// <param name="loginInfo">登陆信息</param>  
        /// <returns>用户信息</returns>  
        public async Task<WechatUserInfoModel> GetDecryptUserInfoAsync(EncryptedLoginInfoModel input)
        {
            if (input == null)
                throw new Exception("获取登录信息请求数据不能为空");
            OpenIdAndSessionKeyModel model =await GetOpenIdAndSessionKeyAsync(input.code);
            if(model.errcode!=0)
                throw new Exception($"获取session_key失败,code:{model.errcode} 信息:{model.errmsg}");
            if (!VaildateSignature(input.rawData,input.signature, model.session_key))
                throw new Exception("signature校验失败");
            string result = SecurityHelper.AESDecryptString(input.encryptedData, input.iv, model.session_key);
            //反序列化结果，生成用户信息实例  
            return result.ToObject<WechatUserInfoModel>();
        }
        /// <summary>
        /// 校验签名  小程序签名校验经常第一次失败 第二次成功 前端要检查session_key是否在有效期内
        /// </summary>
        /// <param name="rawData">公开用户资料</param>
        /// <param name="signature">公开资料携带的签名信息</param>
        /// <param name="sessionKey">从服务端获取的SessionKey</param>
        /// <returns>是否有效</returns>
        private bool VaildateSignature(string rawData, string signature, string sessionKey)
        {
            return SecurityHelper.GetSHA1(rawData + sessionKey, Encoding.UTF8)==signature.ToLower();
        }
        /// <summary>
        /// 小程序获取手机号 需要认证的公众号才有权限
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PhoneNumModel> GetDecryptPhoneNumAsync(EncryptedPhoneNumModel input)
        {
            if (input == null)
                throw new Exception("获取手机号请求数据不能为空");
            OpenIdAndSessionKeyModel model = await GetOpenIdAndSessionKeyAsync(input.code);
            if (model.errcode!=0)
                throw new Exception($"获取session_key失败,code:{model.errcode} 信息:{model.errmsg}");
            string result = SecurityHelper.AESDecryptString(input.encryptedData, input.iv, model.session_key);
            return result.ToObject<PhoneNumModel>();
        }
        /// <summary>
        /// 发送订阅消息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SendSubscribeMsg(string access_token,SendSubscribeInputDto input) 
        {
            string url = "https://api.weixin.qq.com/cgi-bin/message/subscribe/send?access_token={0}".FormatWith(access_token);
            string jsonRes = await WebHelper.HttpPostAsync(url, input.ToJson(),null);
            JObject jObject = jsonRes.ToJObject();
            if (jObject["errcode"].ToInt() != 0) 
            {
                throw new Exception(jObject["errmsg"].ToString());
            }
        }

    }
}
