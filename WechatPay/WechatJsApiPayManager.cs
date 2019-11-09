using PXLibCore.Extensions;
using PXLibCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WechatSDKCore.Commons;
using WechatSDKCore.WechatPay.Models;

namespace WechatSDKCore.WechatPay
{
    public class WechatJsApiPayManager
    {
        private readonly string unifiedorderPayUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";//统一支付地址
        private readonly string _appId;//支付应用（小程序）的appId 
        private readonly string _mchId;//商户的Id
        private readonly string _notifyUrl;//通知回调地址
        private readonly string _appKey;//用户在商户平台自定义的appKey
        public WechatJsApiPayManager(string appId, string mchId, string appKey, string notifyUrl)
        {
            _appId = appId;
            if (string.IsNullOrEmpty(_appId))
                throw new Exception("应用AppId不能为空");
            _mchId = mchId;
            if (string.IsNullOrEmpty(_mchId))
                throw new Exception("商户平台的MchId不能为空");
            _notifyUrl = notifyUrl;
            if (string.IsNullOrEmpty(_notifyUrl))
                throw new Exception("商户平台通知支付成功回调地址不能为空");
            _appKey = appKey;
            if (string.IsNullOrEmpty(_appKey))
                throw new Exception("商户平台自定义的AppKey不能为空");
        }
        public async Task<PayReturnModel> SubmitJsApiUnifiedOrderPay(PayInputModel payInput)
        {
            string nonceStr = CommonUtlis.GetNonceStr();
            var packageDic = this.CreatePayPackage(payInput, nonceStr);
            string packageXml = CreateXmlPackage(packageDic);
            string xmlResult = await WebHelper.HttpPostAsync(unifiedorderPayUrl, packageXml, null, 10);
            string prepay_id = GetPrepayId(xmlResult);        
            return CreatePayReturnModel(nonceStr,prepay_id);
        }

        private IDictionary<string, object> CreatePayPackage(PayInputModel payInput, string nonceStr)
        {
            int total_fee = Convert.ToInt32(payInput.TotalAmount * 100);
            //****************************************************************获取预支付订单编号***********************
            //设置package订单参数
            IDictionary<string, object> packageDic = new Dictionary<string, object>
            {
                { "appid", this._appId },//应用ID 
                { "mch_id", this._mchId },//商户号
                { "nonce_str",nonceStr },//随机字符串
                { "body", payInput.Body },//商品描述 String(128) 做二次支付的时候订单号不能重复 如果要重复 第二次和第一次的商品描述要一样   package.Add("detail",body);//商品详细     
                { "out_trade_no", payInput.OpenId },//商户订单号
                { "total_fee", total_fee },//总金额
                { "spbill_create_ip", WebHelper.GetRemoteIpAddress() },//终端IP
                { "notify_url", this._notifyUrl },//通知地址
                { "trade_type", "JSAPI" },//交易类型
                { "openid", payInput.OpenId },//用户标识
                { "fee_type", "CNY" }, //币种，1人民币   66  
                { "attach", payInput.Attach } //自定义参数  
            };  //请求参数集合
            var paySign = GetMD5Sign(packageDic, this._appKey);//使用Md5签名
            packageDic.Add("sign", paySign);
            return packageDic;
        }
        private string GetMD5Sign(IDictionary<string, object> packageDic, string appKey)//这里就使用MD5加密 还有另外一个加密方式 HMAC-SHA256
        {
            string signStr = string.Join("&", packageDic
                .Where(m => "sign".CompareTo(m.ToString()) != 0 && m.Value != null && (m.Value as string) != "")
                .OrderBy(m => m.Key)
                .Select(m => m.Key + "=" + m.Value)) + "&key=" + appKey;//拼接加密参数
            return signStr.ToMd5();
        }
        private string CreateXmlPackage(IDictionary<string, object> packageDic)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            foreach (KeyValuePair<string, object> kv in packageDic)
            {
                if (kv.Value != null)
                {
                    sb.AppendFormat("<{0}>{1}</{0}>", kv.Key, kv.Value);
                }
            }
            sb.Append("</xml>");
            return sb.ToString();
        }
        private string GetPrepayId(string xmlResult) //处理微信支付返回的Xml 获取prepay_id
        {
            //接收微信返回的xml数据
            var res = XDocument.Parse(xmlResult);
            if (res.Element("xml").Element("return_code").Value.ToUpper() == "SUCCESS")
            {
                try
                {
                    string repayId = res.Element("xml").Element("prepay_id").Value; //无节点就报错   //下单成功返回
                    string prepay_id = string.Format("prepay_id={0}", repayId);
                    return prepay_id;
                }
                catch
                {
                    throw new Exception("支付失败:" + res.Element("xml").Element("err_code_des").Value);
                }
            }
            else
            {
                string return_msg = res.Element("xml").Element("return_msg").Value;
                throw new Exception("支付失败:" + return_msg);
            }
        }
        private PayReturnModel CreatePayReturnModel(string nonceStr,string prepay_id) 
        {
            string timeStamp = CommonUtlis.GetTimestamp();
            IDictionary<string, object> paySignDic = new Dictionary<string, object>
            {
                { "appId", this._appId },
                { "timeStamp", timeStamp },
                { "nonceStr", nonceStr },
                { "package", prepay_id },
                { "signType", "MD5" }
            };//坑2 参数区分大小写 
            var paySign =this.GetMD5Sign(paySignDic, this._appKey);//坑1 APPKey可能也有错 
            PayReturnModel model = new PayReturnModel
            {
                timeStamp = timeStamp,
                nonceStr = nonceStr,
                package = prepay_id,
                paySign = paySign,
                signType = "MD5"
            };
            return model;//最终返回前端的参数
        }
    }
}
