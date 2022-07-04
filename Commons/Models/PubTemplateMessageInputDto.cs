using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WechatSDKCore.Commons.Models
{
    public class PubTemplateMessageInputDto
    {
        /// <summary>
        /// 公众号openid
        /// </summary>
        public string touser { get; set; }
        public string template_id { get; set; }
        public miniprogram miniprogram { get; set; }
        /// <summary>
        ///    是 模板数据
        /// </summary>
        public object data { get; set; }
        /// <summary>
        /// 否   模板内容字体颜色，不填默认为黑色
        /// </summary>
        public string color { get;set; }
        /// <summary>
        ///  否 防重入id。对于同一个openid + client_msg_id, 只发送一条消息,10分钟有效,超过10分钟不保证效果。若无防重入需求，可不填
        /// </summary>
        public string client_msg_id { get; set; }

    }
}