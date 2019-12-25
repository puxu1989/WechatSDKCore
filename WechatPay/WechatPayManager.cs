using PXLibCore.Extensions;
using PXLibCore.Helpers;
using System;
using System.Threading.Tasks;
using WechatSDKCore.Commons;
using WechatSDKCore.Commons.Models;
using WechatSDKCore.WechatPay.Models;

namespace WechatSDKCore.WechatPay
{
    public class WechatPayManager
    {
        public string AppId { get; set; }//支付应用（小程序）的appId  
        private readonly string _mchId;//商户的Id
        private readonly string _notifyUrl;//通知回调地址
        private readonly string _appKey;//用户在商户平台自定义的appKey
        private readonly string _signType = "MD5";
        public WechatPayManager( string mchId, string appKey, string notifyUrl, string appId)
        {
            _mchId = mchId;
            if (string.IsNullOrEmpty(_mchId))
                throw new Exception("商户平台的MchId不能为空");
            _notifyUrl = notifyUrl;
            if (string.IsNullOrEmpty(_notifyUrl))
                throw new Exception("商户平台通知支付成功回调地址不能为空");
            _appKey = appKey;
            if (string.IsNullOrEmpty(_appKey))
                throw new Exception("商户平台自定义的AppKey不能为空");
            AppId = appId;
            if (string.IsNullOrEmpty(AppId))
                throw new Exception("应用AppId不能为空");
        }
        #region JsApiUnified统一下单 提交支付
        /// <summary>
        /// JsApiUnified统一下单  JSAPI一般用于微信浏览器内支付 用于公众号和小程序
        /// </summary>
        /// <param name="payInput"></param>
        /// <returns></returns>
        public async Task<PayReturnModel> SubmitUnifiedOrderPay(PayInputModel payInput)
        {
            string unifiedorderPayUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";//统一支付地址
            string nonceStr = WechatCommonUtlis.GetNonceStr();
            var xmlPackage = this.CreatePayPackageAndGetPackageXml(payInput, nonceStr);
            string xmlResult = await WebHelper.HttpPostAsync(unifiedorderPayUrl, xmlPackage, null, 10);
            string prepay_id = string.Format("prepay_id={0}", GetPrepayId(xmlResult));//坑3
            return CreateJsApiPayReturnModel(nonceStr, prepay_id);
        }

        private string CreatePayPackageAndGetPackageXml(PayInputModel payInput, string nonceStr)
        {
            int total_fee = Convert.ToInt32(payInput.TotalAmount * 100);
            //****************************************************************获取预支付订单编号***********************
            PackageParamModel packageDic = new PackageParamModel();
            packageDic.AddValue("appid", this.AppId);//应用ID 
            packageDic.AddValue("mch_id", this._mchId);//商户号 
            packageDic.AddValue("nonce_str", nonceStr);//随机字符串 
            packageDic.AddValue("body", payInput.Body);//商品描述 String(128) 做二次支付的时候订单号不能重复 如果要重复 第二次和第一次的商品描述要一样   package.Add("detail",body);//商品详细  
            packageDic.AddValue("out_trade_no", payInput.OrderId);//商户订单号 
            packageDic.AddValue("total_fee", total_fee);//支付总金额
            packageDic.AddValue("spbill_create_ip", WebHelper.GetRemoteIpAddress());//终端IP
            packageDic.AddValue("notify_url", this._notifyUrl);//通知地址
            packageDic.AddValue("trade_type", payInput.TradeType.GetEnumDescription());//交易类型 小程序使用此类型
            packageDic.AddValue("fee_type", "CNY");//币种，人民币  
            packageDic.AddValue("attach", payInput.Attach);//自定义参数  
            if (payInput.TradeType == TradeType.JSAPI) 
            {
                if (payInput.OpenId.IsNullOrEmpty())
                    throw new WxPayException("JSAPI必须指定OpenId");
            }
            if (!payInput.OpenId.IsNullOrEmpty()) 
            {
                packageDic.AddValue("openid", payInput.OpenId);//用户标识JSAPI毕传
            }
            var paySign = WechatCommonUtlis.GetMD5Sign(packageDic, this._appKey);//使用Md5签名
            packageDic.AddValue("sign", paySign);
            return packageDic.ToXml();
        }
        private string GetPrepayId(string xmlResult) //处理微信支付返回的Xml 获取prepay_id
        {
            PackageParamModel packageParam = new PackageParamModel();
            packageParam.FromXml(xmlResult);
            return packageParam.GetValue("prepay_id").ToString();
        }
        private PayReturnModel CreateJsApiPayReturnModel(string nonceStr, string prepay_id)
        {
            string timeStamp = WechatCommonUtlis.GetTimestamp();
            PackageParamModel packageDic = new PackageParamModel();//坑2 参数区分大小写 
            packageDic.AddValue("appId", this.AppId);// 注意这里的大小写 应用AppID 
            packageDic.AddValue("timeStamp", timeStamp);
            packageDic.AddValue("nonceStr", nonceStr);
            packageDic.AddValue("package", prepay_id);
            packageDic.AddValue("signType", this._signType);
            var paySign = WechatCommonUtlis.GetMD5Sign(packageDic, this._appKey);//坑1 APPKey可能也有错 
            PayReturnModel model = new PayReturnModel
            {
                timeStamp = timeStamp,
                nonceStr = nonceStr,
                package = prepay_id,
                paySign = paySign,
                signType = this._signType,
                appId = AppId
            };
            return model;//最终返回前端的参数
        }
        #endregion
        #region  订单查询
        /// <summary>
        /// 该接口提供所有微信支付订单的查询，商户可以通过查询订单接口主动查询订单状态，完成下一步的业务逻辑。
        /// </summary>
        /// <param name="transaction_id">商户订单号out_trade_no二选一</param>
        /// <returns></returns>
        public async Task<bool> QueryOrder(string transaction_id) 
        {
            string url = "   https://api.mch.weixin.qq.com/pay/orderquery";
            PackageParamModel packageDic = new PackageParamModel();
            packageDic.AddValue("appid", this.AppId);//应用ID 
            packageDic.AddValue("mch_id", this._mchId);//商户号 
            packageDic.AddValue("transaction_id", transaction_id);//商户号 
            packageDic.AddValue("nonce_str", WechatCommonUtlis.GetNonceStr());
            packageDic.AddValue("sign_type", this._signType);
            packageDic.AddValue("sign", WechatCommonUtlis.GetMD5Sign(packageDic, this._appKey));//签名
            string xmlPackage = packageDic.ToXml();
            string response = await WebHelper.HttpPostAsync(url, xmlPackage,null,10);
            PackageParamModel newResult = new PackageParamModel();
            newResult.FromXml(response);
            if (newResult.GetValue("return_msg").ToString() == "OK") 
            {
                return true;
            }
            return false;
        }
        #endregion
        #region  提现转账到零钱
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="certPathName">证书地址 最好不要放到网站目录</param>
        /// <param name="certPwd">默认是商户号(坑) 不是管理证书的密码</param>
        /// <returns></returns>
        public async Task<TransfersReturnModel> SubmitTransfers(TransfersPayModel inputData,string certPathName,string certPwd)
        {
            var url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";
            PackageParamModel packageParam = inputData.GetApiParameters();
            packageParam.AddValue("mch_appid", this.AppId);//公众账号ID?
            packageParam.AddValue("mchid", this._mchId);//商户号
            packageParam.AddValue("nonce_str", WechatCommonUtlis.GetNonceStr());//随机字符串
            packageParam.AddValue("sign", WechatCommonUtlis.GetMD5Sign(packageParam, this._appKey));//签名
            string xmlPackage = packageParam.ToXml();
            string response = await WebHelper.HttpPostCertAsync(url, xmlPackage, certPathName, certPwd, 10);//证书默认密码为微信商户号
            PackageParamModel newResult = new PackageParamModel();
            newResult.FromXml(response);
            if (newResult.GetValue("result_code").ToString().ToUpper() == "SUCCESS")
            {
                TransfersReturnModel transfersReturnModel = new TransfersReturnModel
                {
                    partner_trade_no = newResult.GetValue("partner_trade_no").ToString(),
                    payment_no = newResult.GetValue("payment_no").ToString(),
                    payment_time = newResult.GetValue("payment_time").ToDate(),
                };
                return transfersReturnModel;
            }
            throw new WxPayException(newResult.GetValue("err_code_des").ToString());
        }
        #endregion
        #region 企业付款到银行卡
        public async Task<PayBankReturnModel> SubmitPayBank(PayBankModel inputData, string pubkey,string certFilePath,string certPwd)
        {
            string url = "https://api.mch.weixin.qq.com/mmpaysptrans/pay_bank";
            PackageParamModel packageParam = inputData.GetApiParameters(pubkey);
            packageParam.AddValue("mch_id", this._mchId);
            packageParam.AddValue("nonce_str", WechatCommonUtlis.GetNonceStr());
            packageParam.AddValue("sign", WechatCommonUtlis.GetMD5Sign(packageParam, this._appKey));
            string xmlPackage = packageParam.ToXml();
            string response = await WebHelper.HttpPostCertAsync(url, xmlPackage, certFilePath, certPwd, 10);//证书默认密码为微信商户号
            PackageParamModel newResult = new PackageParamModel();
            newResult.FromXml(response);
            if (newResult.GetValue("result_code").ToString().ToUpper() == "SUCCESS") 
            {
                PayBankReturnModel model = new PayBankReturnModel
                {
                    partner_trade_no = newResult.GetValue("partner_trade_no").ToString(),
                    payment_no = newResult.GetValue("payment_no").ToString(),
                    cmms_amt = (newResult.GetValue("cmms_amt").ToInt()/100).ToDecimal(2),
                };
                return model;
            }
            throw new WxPayException(newResult.GetValue("err_code_des").ToString());
        }


        #endregion
        #region 获取公钥 微信测提供
        /// <summary>
        /// 获取RSA加密公钥API
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPublicKey(string certPathName, string certPwd)
        {
            string url = "https://fraud.mch.weixin.qq.com/risk/getpublickey";
            PackageParamModel packageParam = new PackageParamModel();
            packageParam.AddValue("mch_id", this._mchId);
            packageParam.AddValue("nonce_str", WechatCommonUtlis.GetNonceStr());
            packageParam.AddValue("sign_type", this._signType);
            packageParam.AddValue("sign", WechatCommonUtlis.GetMD5Sign(packageParam, this._appKey));//签名
            string xmlPackage = packageParam.ToXml();
            string response = await WebHelper.HttpPostCertAsync(url, xmlPackage, certPathName, certPwd, 10);//证书默认密码为微信商户号
            PackageParamModel newResult = new PackageParamModel();
            newResult.FromXml(response);
            if (newResult.GetValue("result_code").ToString().ToUpper() == "SUCCESS")
            {
                return newResult.GetValue("pub_key").ToString();
            }
            throw new WxPayException(newResult.GetValue("err_code_des").ToString());
        }
        #endregion 
    }
}
