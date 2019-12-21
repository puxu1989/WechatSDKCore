using PXLibCore.Extensions;
using PXLibCore.Extensions.Json;
using PXLibCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WechatSDKCore.AuthManager.Models;

namespace WechatSDKCore.AuthManager
{
    /// <summary>
    /// 公众号授权登录
    /// </summary>
    public class WechatAuthManager
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public WechatAuthManager(string appId, string appSecret)
        {
            if (string.IsNullOrEmpty(appId))
                throw new Exception("AppId不能为空");
            this.AppId = appId;
            if (string.IsNullOrEmpty(appSecret))
                throw new Exception("ApSecret不能为空");
            this.AppSecret = appSecret;
        }
        //开发步骤 1.验证服务器是有有效 2.获取code 这两部直接在控制器或者api里调用 3.直接调用GetUserInfo
        public async Task<AuthAccessTokenModel> GetAccessTokenAsync(string code, string grant_type = "authorization_code")
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type={3}".FormatWith(AppId, AppSecret, code, grant_type);
            string resJson = await WebHelper.HttpGetAsync(url);
            AuthAccessTokenModel model = resJson.ToObject<AuthAccessTokenModel>();
            return model;
        }
        public async Task<AuthUserInfoModel> GetUserInfoByAccessTokenAsync(string access_token,string openid)//需scope为 snsapi_userinfo
        {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN".FormatWith(access_token, openid);
            string resJson = await WebHelper.HttpGetAsync(url);
            AuthUserInfoModel model = resJson.ToObject<AuthUserInfoModel>();
            return model;
        }
        /// <summary>
        /// 通过code获取用户拉取微信授权登录信息 以上两步可以合为一步
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<AuthUserInfoModel> GetUserInfo(string code) 
        {
            AuthAccessTokenModel tokenModel = await this.GetAccessTokenAsync(code);
            if (tokenModel.errcode == 0)
            {
                var authUserInfo = await this.GetUserInfoByAccessTokenAsync(tokenModel.access_token, tokenModel.openid);
                return authUserInfo;
            }
            else 
            {
                throw new Exception(new { tokenModel.errcode, tokenModel.errmsg }.ToJson());
            }
        }
    }
}
