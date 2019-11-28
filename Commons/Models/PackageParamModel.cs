using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WechatSDKCore.Commons.Models
{
    /// <summary>
    /// 微信支付协议接口数据类，所有的API接口通信都依赖这个数据结构，
    /// 在调用接口之前先填充各个字段的值，只支出int和string然后进行接口通信，
    /// 这样设计的好处是可扩展性强，用户可随意对协议进行更改而不用重新设计数据结构，
    /// 还可以随意组合出不同的协议数据包，不用为每个协议设计一个数据包结构
    /// 整个微信支付基本采用xml格式 协议要求判断逻辑	 先判断协议字段返回，再判断业务返回，最后判断交易状态
    /// </summary>
    public class PackageParamModel
    {
        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private  SortedDictionary<string, object> _values = new SortedDictionary<string, object>();
        public void AddValue(string key, object value)
        {
            if(value==null||string.IsNullOrEmpty(value.ToString()))
                throw new WxPayException("打包数据不能添加空值!");
            this._values[key] = value;
        }
        public object GetValue(string key)
        {
            if (_values.TryGetValue(key, out object o))
                return o;
            else
                return string.Empty;
        }
        public bool IsSet(string key)
        {
            _values.TryGetValue(key, out object o);
            if (null != o)
                return true;
            else
                return false;
        }
        public SortedDictionary<string, object> GetValues()
        {
            return _values;
        }
        public string ToXml()
        {
            if ( _values.Count==0) //数据为空时不能转化为xml格式
            {
                throw new WxPayException("打包数据不能为空!");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            foreach (KeyValuePair<string, object> pair in _values)
            {
                if (pair.Value.GetType() == typeof(int))
                {
                    sb.AppendFormat("<{0}>{1}</{2}>",pair.Key,pair.Value,pair.Key);
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    sb.AppendFormat("<{0}><![CDATA[{1}]]></{2}>", pair.Key, pair.Value, pair.Key);
                }
                else//除了string和int类型不能含有其他数据类型
                {
                    throw new WxPayException("打包字段数据类型错误!");
                }
            }
            sb.Append("</xml>");
            return sb.ToString();
        }
        public SortedDictionary<string, object> FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new WxPayException("不能将空字符串转换为PackageParamModel");
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                _values[xe.Name] = xe.InnerText;
            }
            if (this.GetValue("return_code").ToString().ToUpper() != "SUCCESS") // 先判断协议字段返回，再判断业务返回，
            {
                throw new WxPayException(this.GetValue("return_msg").ToString());
            }
            return _values;
        }
    }
}
