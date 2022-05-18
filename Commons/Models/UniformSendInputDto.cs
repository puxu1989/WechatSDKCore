using System;
using System.Collections.Generic;
using System.Text;

namespace WechatSDKCore.Commons.Models
{
    public class UniformSendInputDto
    {
        public string touser { get; set; }// 是   用户openid，可以是小程序的openid，也可以是mp_template_msg.appid对应的公众号的openid
        public weapp_template_msg weapp_template_msg { get; set; }///  否   小程序模板消息相关的信息，可以参考小程序模板消息接口; 有此节点则优先发送小程序模板消息；（小程序模板消息已下线，不用传此节点）
        public mp_template_msg mp_template_msg { get; set; }   //是 公众号模板消息相关的信息，可以参考公众号模板消息接口；有此节点并且没有weapp_template_msg节点时，发送公众号模板消息
    }
    public class weapp_template_msg
    {
        public string template_id { get; set; }// 是   小程序模板ID
        public string page { get; set; }//是   小程序页面路径
        public string form_id { get; set; }// 是   小程序模板消息formid
        public object data { get; set; } //是   小程序模板数据
        public string emphasis_keyword { get; set; } //   是   小程序模板放大关键词

    }
    public class mp_template_msg
    {
        public string appid { get; set; } //是   公众号appid，要求与小程序有绑定且同主体
        public string template_id { get; set; } //是   公众号模板id
        public string url { get; set; } //  公众号模板消息所要跳转的url
        public miniprogram miniprogram { get; set; }// 是   公众号模板消息所要跳转的小程序，小程序的必须与公众号具有绑定关系
        public object data { get; set; } //是   公众号模板消息的数据
    }
    public class miniprogram
    {
        public string appid { get; set; }
        public string pagepath { get; set; }
    }
}
