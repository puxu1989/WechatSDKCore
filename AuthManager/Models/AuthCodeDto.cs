using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WechatSDKCore.AuthManager.Models
{
   public class ProviderAuthCodeDto
    {
        public string ProviderName { get; set; } = "WechatPub"; //目前支持WechatApp,WechatPub 如果是APP传WechatApp
        [Required(ErrorMessage ="Code不能为空")]
        public string Code { get; set; }
    }
}
