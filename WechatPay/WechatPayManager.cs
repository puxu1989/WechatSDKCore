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
        private readonly string _appId;//支付应用（小程序）的appId 
        private readonly string _mchId;//商户的Id
        private readonly string _notifyUrl;//通知回调地址
        private readonly string _appKey;//用户在商户平台自定义的appKey
        private readonly string _signType = "MD5";
        public WechatPayManager(string appId, string mchId, string appKey, string notifyUrl)
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
        #region JsApiUnified统一下单 提交支付
        /// <summary>
        /// JsApiUnified统一下单 提交支付
        /// </summary>
        /// <param name="payInput"></param>
        /// <returns></returns>
        public async Task<PayReturnModel> SubmitJsApiUnifiedOrderPay(PayInputModel payInput)
        {
            string unifiedorderPayUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";//统一支付地址
            string nonceStr = CommonUtlis.GetNonceStr();
            var xmlPackage = this.CreatePayPackageAndGetPackageXml(payInput, nonceStr);
            string xmlResult = await WebHelper.HttpPostAsync(unifiedorderPayUrl, xmlPackage, null, 10);
            string prepay_id = GetPrepayId(xmlResult);
            return CreateJsApiPayReturnModel(nonceStr, prepay_id);
        }

        private string CreatePayPackageAndGetPackageXml(PayInputModel payInput, string nonceStr)
        {
            int total_fee = Convert.ToInt32(payInput.TotalAmount * 100);
            //****************************************************************获取预支付订单编号***********************
            PackageParamModel packageDic = new PackageParamModel();
            packageDic.AddValue("appid", this._appId);//应用ID 
            packageDic.AddValue("mch_id", this._mchId);//商户号 
            packageDic.AddValue("openid", payInput.OpenId);//用户标识
            packageDic.AddValue("nonce_str", nonceStr);//随机字符串 
            packageDic.AddValue("body", payInput.Body);//商品描述 String(128) 做二次支付的时候订单号不能重复 如果要重复 第二次和第一次的商品描述要一样   package.Add("detail",body);//商品详细  
            packageDic.AddValue("out_trade_no", payInput.OrderId);//商户订单号 
            packageDic.AddValue("total_fee", total_fee);//支付总金额
            packageDic.AddValue("spbill_create_ip", WebHelper.GetRemoteIpAddress());//终端IP
            packageDic.AddValue("notify_url", this._notifyUrl);//通知地址
            packageDic.AddValue("trade_type", "JSAPI");//交易类型 小程序使用此类型
            packageDic.AddValue("fee_type", "CNY");//币种，人民币  
            packageDic.AddValue("attach", payInput.Attach);//自定义参数  
            var paySign = CommonUtlis.GetMD5Sign(packageDic, this._appKey);//使用Md5签名
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
            string timeStamp = CommonUtlis.GetTimestamp();
            PackageParamModel packageDic = new PackageParamModel();//坑2 参数区分大小写 
            packageDic.AddValue("appId", this._appId);// 注意这里的大小写 应用AppID 
            packageDic.AddValue("timeStamp", timeStamp);
            packageDic.AddValue("nonceStr", nonceStr);
            packageDic.AddValue("package", prepay_id);
            packageDic.AddValue("signType", this._signType);
            var paySign = CommonUtlis.GetMD5Sign(packageDic, this._appKey);//坑1 APPKey可能也有错 
            PayReturnModel model = new PayReturnModel
            {
                timeStamp = timeStamp,
                nonceStr = nonceStr,
                package = prepay_id,
                paySign = paySign,
                signType = this._signType
            };
            return model;//最终返回前端的参数
        }
        #endregion
        #region  提现转账到零钱
        public async Task<TransfersReturnModel> SubmitTransfers(TransfersPayModel inputData)
        {
            var url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";
            PackageParamModel packageParam = inputData.GetApiParameters();
            packageParam.AddValue("mch_appid", this._appId);//公众账号ID?
            packageParam.AddValue("mchid", this._mchId);//商户号
            packageParam.AddValue("nonce_str", CommonUtlis.GetNonceStr());//随机字符串
            packageParam.AddValue("sign", CommonUtlis.GetMD5Sign(packageParam, this._appKey));//签名
            string xmlPackage = packageParam.ToXml();
            string response = await WebHelper.HttpPostCertAsync(url, xmlPackage, "", "", 10);//证书默认密码为微信商户号
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
        public async Task<PayBankReturnModel> SubmitPayBank(PayBankModel inputData, string pubkey)
        {
            string url = "	https://api.mch.weixin.qq.com/mmpaysptrans/pay_bank";
            PackageParamModel packageParam = inputData.GetApiParameters(pubkey);
            packageParam.AddValue("mch_id", this._mchId);
            packageParam.AddValue("nonce_str", CommonUtlis.GetNonceStr());
            packageParam.AddValue("sign", CommonUtlis.GetMD5Sign(packageParam, this._appKey));
            string xmlPackage = packageParam.ToXml();
            string response = await WebHelper.HttpPostCertAsync(url, xmlPackage, "", "", 10);//证书默认密码为微信商户号
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

        /// <summary>
        /// 获取RSA加密公钥API
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPublicKey()
        {
            string url = "https://fraud.mch.weixin.qq.com/risk/getpublickey";
            PackageParamModel packageParam = new PackageParamModel();
            packageParam.AddValue("mch_id", this._mchId);
            packageParam.AddValue("nonce_str", CommonUtlis.GetNonceStr());
            packageParam.AddValue("sign_type", this._signType);
            packageParam.AddValue("sign", CommonUtlis.GetMD5Sign(packageParam, this._appKey));//签名
            string xmlPackage = packageParam.ToXml();
            string response = await WebHelper.HttpPostCertAsync(url, xmlPackage, "", "", 10);//证书默认密码为微信商户号
            PackageParamModel newResult = new PackageParamModel();
            newResult.FromXml(response);
            if (newResult.GetValue("result_code").ToString().ToUpper() == "SUCCESS")
            {
                return newResult.GetValue("pub_key").ToString();
            }
            throw new WxPayException(newResult.GetValue("err_code_des").ToString());
        }
    }
}
