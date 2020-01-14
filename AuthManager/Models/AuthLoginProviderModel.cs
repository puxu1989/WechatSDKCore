using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WechatSDKCore.AuthManager.Models
{
   public class AuthLoginProviderModel
    {
        [Required]
        public string ProviderName { get; set; }
        [Required]
        public string AppId { get; set; }
        [Required]
        public string AppSecret { get; set; }
        public AuthLoginProviderModel(string providerName, string appId, string appSecret)
        {
            ProviderName = providerName;
            AppId = appId;
            AppSecret = appSecret;
        }
    }
}
