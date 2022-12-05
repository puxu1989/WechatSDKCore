using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WechatSDKCore.Commons.Models
{
    /// <summary>
    /// 公众号订阅信息  https://developers.weixin.qq.com/doc/offiaccount/User_Management/Get_users_basic_information_UnionID.html#UinonId
    /// </summary>
    public class SubscribeUserInfoModel
    {
        public bool subscribe { get; set; }
        public string openid { get; set; }
        public string language{get;set;}
        public long subscribe_time { get; set; }
        public string unionid { get; set; }
        public string remark { get; set; }
        public string subscribe_scene { get; set; }
          
        //        "groupid": 0,
       //    "tagid_list":[128,2],
    }
    public class SubscribeUserInfoModelList
    { 
       public List<SubscribeUserInfoModel> user_info_list { get; set; }
    }
}
