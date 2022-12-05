using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WechatSDKCore.Commons.Models
{
    public class SubButton
    {
        public string type { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string appid { get; set; }
        public string pagepath { get; set; }
        public string key { get; set; }
    }

    public class Button
    {
        public string type { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public IList<SubButton> sub_button { get; set; }=new List<SubButton>();
    }

    public class PostButtonInputDto
    {
        public IList<Button> button { get; set; }=new List<Button>();
    }
}
