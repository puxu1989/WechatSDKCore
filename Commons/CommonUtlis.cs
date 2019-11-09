using PXLibCore.Extensions.Json;
using PXLibCore.Helpers;
using System;
using System.Text;
using System.Threading.Tasks;
using WechatSDKCore.Commons.Models;

namespace WechatSDKCore.Commons
{
    public class CommonUtlis
    {
        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static async Task<AccessTokenModel> GetAccessTokenAsync(string appId, string appSecret)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={appSecret}";
            string resJson = await WebHelper.HttpGetAsync(url);
            AccessTokenModel accessTokenModel = resJson.ToObject<AccessTokenModel>();
            return accessTokenModel;
        }
        /// <summary>
        /// 通过AccessToken换取ticket
        /// </summary>
        /// <param name="accessToken">上面获取到的Token</param>
        /// <returns></returns>
        public static async Task<TicketModel> GetTicketAsync(string accessToken)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + accessToken + "&type=jsapi";
            string resJson = await WebHelper.HttpGetAsync(url);
            TicketModel ticketModel = resJson.ToObject<TicketModel>();
            return ticketModel;
        }
        public static string GetShareSignature(string ticket, string nonceStr, long timestamp, string link)
        {
            string param = "jsapi_ticket=" + ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + link;  //拼接字符串准备生成签名
            string signature = SecurityHelper.GetSHA1(param, Encoding.UTF8); //微信签名
            return signature;
        }
        //====================================支付使用====================================
        /// <summary>  
        /// 获取时间时间戳Timestamp  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimestamp()
        {
            DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt64((DateTime.Now - DateStart).TotalSeconds).ToString();
        }
        /// <summary>
        /// 获取随机字符
        /// </summary>
        /// <returns></returns>
        public static string GetNonceStr() 
        {
           return Guid.NewGuid().ToString("N");//32位
        }
    }
}
