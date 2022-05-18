using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WechatSDKCore.MPManager.Models
{
   public class SendSubscribeInputDto
    {
        [Required]
        public string touser { get; set; }//用户的OpenId
        [Required]
        public string template_id { get; set; }//所需下发的订阅模板id
        [Required]
        public object data { get; set; }//模板内容，格式形如 { "key1": { "value": any }, "key2": { "value": any } }
        public string page { get; set; }//点击模板卡片后的跳转页面，仅限本小程序内的页面。支持带参数,（示例index?foo=bar）。该字段不填则模板无跳转。
    }
    public class TemplateMsgDto 
    {
    public string TemplateId { get; set; }
        public object Keys { get;set;}
    }
}
