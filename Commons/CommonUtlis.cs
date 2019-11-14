using PXLibCore.Extensions;
using PXLibCore.Extensions.Json;
using PXLibCore.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        /// <summary>
        /// 生成小程序二维码 如果成功 result直接是是纯图片流 不成功则result可以转换成json描述
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="scene">最大32个可见字符，只支持数字，大小写英文以及部分特殊字符</param>
        /// <param name="page">已经发布的小程序存在的页面（否则报错），例如 pages/index/index, 根路径前不要填加 /,不能携带参数（参数请放在scene字段里），如果不填写这个字段，默认跳主页面</param>
        /// <param name="width">二维码的宽度，默认为 430px，最小 280px，最大 1280px</param>
        /// <param name="auto_color">自动配置线条颜色，如果颜色依然是黑色，则说明不建议配置主色调，默认 false</param>
        /// <param name="line_color">auto_color 为 false 时生效，使用 rgb 设置颜色 例如 {"r":0,"g":0,"b":0} 十进制表示，默认全 0</param>
        /// <param name="is_hyaline">是否需要透明底色</param>
        /// <returns></returns>
        public static async Task<byte[]> GetWXACodeUnlimitAsync(string accessToken,string scene, string page, int width=430, bool auto_color=true, object line_color =null, bool is_hyaline=false) 
        {
            var url = string.Format("https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token={0}", accessToken);
            if (line_color == null || string.IsNullOrEmpty(line_color.ToString())) 
            {
                line_color = new { r = 0, g = 0, b = 0 };
            }
            var postData = new
            {
                scene,
                page,
                width,
                auto_color,
                line_color,
                is_hyaline,
            }.ToJson();
            byte[] result =await  WebHelper.HttpPostAsync(url, postData);
            return result;
        }
        //====================================支付使用====================================
        /// <summary>  
        /// 获取时间时间戳Timestamp  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimestamp()
        {
            DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return (DateTime.Now - DateStart).TotalSeconds.ToString();
        }
        /// <summary>
        /// 获取随机字符
        /// </summary>
        /// <returns></returns>
        public static string GetNonceStr() 
        {
           return Guid.NewGuid().ToString("N");//32位
        }
        public static string GetMD5Sign(PackageParamModel packageDic, string appKey)//这里就使用MD5加密 还有另外一个加密方式 HMAC-SHA256
        {
            string signStr = string.Join("&", packageDic.GetValues()
                .Where(m => "sign".CompareTo(m.ToString()) != 0 && m.Value != null && (m.Value as string) != "")
                .Select(m => m.Key + "=" + m.Value)) + "&key=" + appKey;//拼接加密参数
            return signStr.ToMd5();
        }
    }
}
