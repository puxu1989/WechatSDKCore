using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WechatSDKCore.Commons.Models
{
    internal class BatchOpenIdListModel
    {
        public int total { get; set; }
        public int count { get; set; }
        public OpenIdList data { get; set; }
        public string next_openid { get; set; }
    }
    internal class OpenIdList
    { 
    public string[] openid { get; set; }
    }
}
