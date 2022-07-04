using Newtonsoft.Json.Linq;
using PXLibCore.Base.Application;
using PXLibCore.Extensions;
using PXLibCore.Extensions.Json;
using PXLibCore.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WechatSDKCore.AuthManager.Models;
using WechatSDKCore.Commons.Models;

namespace WechatSDKCore.Commons
{
    public class WechatCommonUtlis
    {
        /// <summary>
        /// 验证服务器是否有效 每个公众号就验证一次
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool CheckServerSignature(ServerCheckModel model, string token)
        {
            if (model.signature.IsNullOrEmpty())
                return false;
            //创建数组，将 token, timestamp, nonce 三个参数加入数组
            string[] array = { token, model.timestamp, model.nonce };
            Array.Sort(array);  //进行排序
            string tempStr = string.Join("", array);            //拼接为一个字符串
            tempStr = GetSHA1(tempStr);
            return model.signature == tempStr;    //判断signature 是否正确
        }
        /// <summary>
        /// 此方法的签名和SecurityHelper.GetSHA1少几位           注意微信的SHA1的签名坑
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string GetSHA1(string strSource)
        {
            string strResult = "";
            using (SHA1 md5 = SHA1.Create())
            {
                byte[] bytResult = md5.ComputeHash(Encoding.UTF8.GetBytes(strSource));    //注意编码UTF8、UTF7、Unicode等的选择 
                for (int i = 0; i < bytResult.Length; i++)  //字节类型的数组转换为字符串 
                {
                    strResult += bytResult[i].ToString("X");//16进制转换 
                }
                return strResult.ToLower();
            }
        }
        /// <summary>
        /// 获取普通的AccessToken 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static async Task<AccessTokenModel> GetAccessTokenAsync(string appId, string appSecret)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={appSecret}";
            string resJson = await WebHelper.HttpGetAsync(url);
            AccessTokenModel accessTokenModel = resJson.ToObject<AccessTokenModel>();//{"errcode":40164,"errmsg":"invalid ip 202.98.205.87 ipv6 ::ffff:202.98.205.87, not in whitelist hint: [li9tVA04332029]"}
            if (accessTokenModel.access_token.IsNullOrEmpty())
                throw new Exception(resJson);
            return accessTokenModel;
        }
        /// <summary>
        /// 从内存缓存里获取普通的AccessToken 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static async Task<AccessTokenModel> GetAccessTokenFromCacheAsync(string appId, string appSecret)
        {
             string cacheKey = "WechatAccessTokenCacheKey_"+ appId;
            AccessTokenModel accessTokenModel = CacheHelper.GetCache<AccessTokenModel>(cacheKey);
            if (accessTokenModel.IsNullOrEmpty())
            {
                accessTokenModel = await GetAccessTokenAsync(appId, appSecret);
                CacheHelper.WriteCache(cacheKey, accessTokenModel, (accessTokenModel.expires_in - 60) / 60);
            }
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
        /// <summary>
        /// 微信签名算法  经过验证此签名和微信提供的测试签名工具是一样的 
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="nonceStr"></param>
        /// <param name="timestamp"></param>
        /// <param name="link">分享链接，该链接域名或路径必须与当前页面对应的公众号JS安全域名一致 最容易出错的就是前段传来的url 必须保持两次一样 为避免出错 前端需要用js获取当前页面除去'#'hash部分的链接（可用location.href.split('#')[0]获取,而且需要encodeURIComponent）重点encodeURIComponent</param>
        /// <returns></returns>
        public static string GetShareSignature(string ticket, string nonceStr, long timestamp, string link)
        {
            string param = "jsapi_ticket=" + ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + link;  //拼接字符串准备生成签名
            string signature = SecurityHelper.GetSHA1(param, Encoding.UTF8); //微信签名
            return signature;
            /*
             invalid signature签名错误。建议按如下顺序检查：
            确认签名算法正确，可用http://mp.weixin.qq.com/debug/cgi-bin/sandbox?t=jsapisign 页面工具进行校验。
            确认config中nonceStr（js中驼峰标准大写S）, timestamp与用以签名中的对应noncestr, timestamp一致。
            确认url是页面完整的url(请在当前页面alert(location.href.split('#')[0])确认)，包括'http(s)://'部分，以及'？'后面的GET参数部分,但不包括'#'hash后面的部分。
            确认 config 中的 appid 与用来获取 jsapi_ticket 的 appid 一致。
            确保一定缓存access_token和jsapi_ticket。
            确保你获取用来签名的url是动态获取的，动态页面可参见实例代码中php的实现方式。如果是html的静态页面在前端通过ajax将url传到后台签名，前端需要用js获取当前页面除去'#'hash部分的链接（可用location.href.split('#')[0]获取,而且需要encodeURIComponent），因为页面一旦分享，微信客户端会在你的链接末尾加入其它参数，如果不是动态获取当前链接，将导致分享后的页面签名失败。
             */
        }
        /// <summary>
        /// 生成小程序二维码 如果成功 result直接是是纯图片流 不成功则result可以转换成json描述
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="scene">最大32个可见字符，只支持数字，大小写英文以及部分特殊字符</param>
        /// <param name="page">已经发布的小程序存在的页面（否则报错），例如 pages/index/index, 根路径前不要填加 /,不能携带参数（参数请放在scene字段里），如果不填写这个字段，默认跳主页面</param>
        /// <param name="env_version"> "release"   否 要打开的小程序版本。正式版为 release，体验版为 trial，开发版为 develop</param>
        /// <param name="width">二维码的宽度，默认为 430px，最小 280px，最大 1280px</param>
        /// <param name="auto_color">自动配置线条颜色，如果颜色依然是黑色，则说明不建议配置主色调，默认 false</param>
        /// <param name="line_color">auto_color 为 false 时生效，使用 rgb 设置颜色 例如 {"r":0,"g":0,"b":0} 十进制表示，默认全 0</param>
        /// <param name="is_hyaline">是否需要透明底色</param>
        /// <returns></returns>
        public static async Task<byte[]> GetWXACodeUnlimitAsync(string accessToken, string scene, string page, string env_version = "release", int width = 430, bool auto_color = true, object line_color = null, bool is_hyaline = false)
        {
            bool check_path = true;
            if (WebHelper.IsDevelopment())
            {
                env_version = "trial";
                check_path = false;
            }
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
                check_path,
                env_version,
                auto_color,
                line_color,
                is_hyaline,
            }.ToJson();
            byte[] result = await WebHelper.HttpPostAsync(url, postData);
            return result;
        }
        /// <summary>
        /// 敏感词检查
        /// </summary>
        /// <param name="content"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<bool> ContentCheck(string content, string accessToken)
        {
            //文档地址 https://mp.weixin.qq.com/cgi-bin/announce?action=getannouncement&key=&version=1&lang=zh_CN&platform=2
            var url = string.Format("https://api.weixin.qq.com/wxa/msg_sec_check?access_token={0}", accessToken);
            var postData = new
            {
                content
            }.ToJson();
            string result = await WebHelper.HttpPostAsync(url, postData, null);
            JObject jObj = result.ToJObject();
            int errcode = jObj["errcode"].ToInt();
            if (errcode == 0)
                return true;
            else if (errcode == 87014)
            {
                throw new ExceptionEx("你输入的内容包含命名词汇");
            }
            else
            {
                throw new Exception(jObj["errmsg"].ToString());
            }
        }
        /// <summary>
        /// 下发小程序和公众号统一的服务消息   小程序订阅发送在本接口里已经下线  公众号使用次接口
        /// </summary>
        /// <returns></returns>
        public static async Task<string> SendUniformMessage(string access_token, UniformSendInputDto input)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/message/wxopen/template/uniform_send?access_token={access_token}";
            string jsonRes = await WebHelper.HttpPostAsync(url, input.ToJson(), null);
            JObject jObject = jsonRes.ToJObject();
            if (jObject["errcode"].ToInt() != 0)
            {
                throw new ExceptionEx(jsonRes);
            }
            return jsonRes;
        }

        #region====================================支付使用签名====================================
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
        public static string GetMD5Sign(PackageParamModel packageDic, string appKey)//这里就使用MD5加密 还有另外一个加密方式 HMAC-SHA256
        {
            string signStr = string.Join("&", packageDic.GetValues()
                .Where(m => "sign".CompareTo(m.ToString()) != 0 && m.Value != null && (m.Value as string) != "")
                .Select(m => m.Key + "=" + m.Value)) + "&key=" + appKey;//拼接加密参数
            return signStr.ToMd5();
        }
        #endregion

        #region 新版公众号开发
        /// <summary>
        /// 批量获取帐号的关注者列表  一次拉取调用最多拉取10000个关注者的OpenID 取消关注后就不会拉取到了  排序和管理后台的排序并不一致
        ///  用于用户取消关注后不在批量拉取的列表里 那以前取消的OpenId还在数据库里 方式1可使用服务器通知的方式获取最新的关注订阅状态
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> BatchGetOpenIdList(string accessToken, string nextOpenId = "")
        {
            List<string> list = new List<string>();
            do
            {
                string url = "https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}".FormatWith(accessToken, nextOpenId);
                string result = await WebHelper.HttpGetAsync(url);
                BatchOpenIdListModel obj = result.ToObject<BatchOpenIdListModel>();
                if (obj.count != 0 && obj.data != null)
                    list.AddRange(obj.data.openid);
                nextOpenId = obj.next_openid;
            } while (!nextOpenId.IsNullOrEmpty());
            return list;
        }
        /// <summary>
        /// 批量获取用户基本信息 每次最多100条  lang国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语，默认为zh-CN
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SubscribeUserInfoModel>> BatchGetSubscribeUserInfo(string accessToken,List<string> openIds,string lang= "zh-CN")
        {
            if (openIds.IsNullOrEmpty())
                return new List<SubscribeUserInfoModel>();//不为空好循环
            string url = "https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token=" + accessToken;
            List<object> postOendIdList = new();
            foreach (var item in openIds) 
            {
                postOendIdList.Add(new { openid = item , lang });
            }
            string result = await WebHelper.HttpPostAsync(url, (new { user_list= postOendIdList }).ToJson(),null);
            //如果混有其他公众号的openId会报错 {\"errcode\":40003,\"errmsg\":\"invalid openid hint: [igBd6BsQf-ooZIPA] rid: 62c27800-3132e7eb-4d784582\"
            SubscribeUserInfoModelList infoList = result.ToObject<SubscribeUserInfoModelList>();
            if (infoList.user_info_list== null)
                throw new ExceptionEx(result);
            return infoList.user_info_list;
        }
        /// <summary>
        /// send发送订阅通知消息  是订阅通知 不是消息模板 接口不一样
        /// </summary>
        /// <returns></returns>
        public static async Task SendPubSubscribeMessage(string access_token, PubSubscribeMessageInputDto input) 
        {
            string url = "https://api.weixin.qq.com/cgi-bin/message/subscribe/bizsend?access_token=" + access_token;
            string jsonRes = await WebHelper.HttpPostAsync(url, input.ToJson(), null);
            JObject jObject = jsonRes.ToJObject();
            if (jObject["errcode"].ToInt() != 0)
            {
                throw new ExceptionEx(jObject.ToJson());
            }
        }
        /// <summary>
        /// 发送模板消息  
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task SendPubTemplateMessage(string access_token, PubTemplateMessageInputDto input)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + access_token;
            string jsonRes = await WebHelper.HttpPostAsync(url, input.ToJson(), null);
            JObject jObject = jsonRes.ToJObject();
            if (jObject["errcode"].ToInt() != 0)
            {
                throw new ExceptionEx(jObject.ToJson());
            }
        }
        #endregion
    }
}
