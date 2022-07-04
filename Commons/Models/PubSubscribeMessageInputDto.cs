using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WechatSDKCore.Commons.Models
{
    public class PubSubscribeMessageInputDto
    {
        /// <summary>
        /// 公众号openid
        /// </summary>
        public string touser { get; set; }
        public string template_id { get; set; }
        public string page { get; set; }
        public miniprogram miniprogram { get; set; }
        public object data { get; set; }
    }
}
