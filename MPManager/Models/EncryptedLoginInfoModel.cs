using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WechatSDKCore.MPManager.Models
{
    /// <summary>
    /// 小程序前端拉取的加密用户信息  已经加密 需要后端解密
    /// </summary>
   public class EncryptedLoginInfoModel
    {
        [Required(ErrorMessage ="jscode不能为空")]
        public string code { get; set; }
        public string encryptedData { get; set; }
        public string iv { get; set; }
        public string rawData { get; set; }
        public string signature { get; set; }
    }
}
