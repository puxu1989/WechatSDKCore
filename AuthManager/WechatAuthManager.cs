using PXLibCore.Extensions;
using PXLibCore.Extensions.Json;
using PXLibCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WechatSDKCore.AuthManager.Models;

namespace WechatSDKCore.AuthManager
{
    /// <summary>
    /// 授权登录  支持公众号 APP
    /// </summary>
    public class WechatAuthManager
    {
        //public string AppId { get; set; }
        //public string AppSecret { get; set; }
        public List<AuthLoginProviderModel> Providers { get; }
        //public WechatAuthManager(string appId, string appSecret)
        //{
        //    if (string.IsNullOrEmpty(appId))
        //        throw new Exception("AppId不能为空");
        //    this.AppId = appId;
        //    if (string.IsNullOrEmpty(appSecret))
        //        throw new Exception("ApSecret不能为空");
        //    this.AppSecret = appSecret;
        //}
        public WechatAuthManager()
        {
            Providers = new List<AuthLoginProviderModel>();
        }
        /// <summary>
        ///  获取授权登录的AccessToken 开发步骤 1.验证服务器是有有效 2.获取code 这两部直接在控制器或者api里调用 3.直接调用GetUserInfo
        /// </summary>
        /// <param name="providerInfo"></param>
        /// <param name="code"></param>
        /// <param name="grant_type"></param>
        /// <returns></returns>
        public async Task<AuthAccessTokenModel> GetAccessTokenAsync(AuthLoginProviderModel providerInfo, string code, string grant_type = "authorization_code")
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type={3}".FormatWith(providerInfo.AppId, providerInfo.AppSecret, code, grant_type);
            string resJson = await WebHelper.HttpGetAsync(url);
            AuthAccessTokenModel tokenModel = resJson.ToObject<AuthAccessTokenModel>();
            return tokenModel;
        }
        public async Task<AuthUserInfoModel> GetUserInfoByAccessTokenAsync(string access_token,string openid)//需scope为 snsapi_userinfo
        {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN".FormatWith(access_token, openid);
            string resJson = await WebHelper.HttpGetAsync(url);
            AuthUserInfoModel model = resJson.ToObject<AuthUserInfoModel>();
            return model;
        }
        /// <summary>
        /// 通过code获取用户拉取微信授权登录信息 以上两步可以合为一步 方便调用 但是返回errcode和系统自定义的不同步
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<AuthUserInfoModel> GetUserInfoAsync(string providerName, string code) 
        {
            var authLoginProviderModel = this.Providers.FirstOrDefault(p => p.ProviderName == providerName);
            if (authLoginProviderModel == null) 
            {
                throw new Exception("未知的授权登录方式:" + providerName);
            }
            AuthAccessTokenModel tokenModel = await this.GetAccessTokenAsync(authLoginProviderModel,code);
            if (tokenModel.errcode == 0)
            {
                var authUserInfo = await this.GetUserInfoByAccessTokenAsync(tokenModel.access_token, tokenModel.openid);
                return authUserInfo;
            }
            else if (tokenModel.errcode == 40163 || tokenModel.errcode == 40029)
            {
                tokenModel.errmsg = "code已被使用或者无效";
                throw new Exception(new { tokenModel.errcode, tokenModel.errmsg }.ToJson());
            }
            else
            {
                throw new Exception(new { tokenModel.errcode, tokenModel.errmsg }.ToJson());
            }
        }
    }
}
