using PXLibCore.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace WechatSDKCore.MPManager.Models
{
    /// <summary>
    /// 微信小程序完善用户信息
    /// </summary>
    public class CompleteMPInputDto
    {
        [Required(ErrorMessage = "昵称不能为空")]
        public string NickName { get; set; }
        [Required(ErrorMessage = "头像不能为空")]
        public string AvatarUrl { get; set; }
        [IsTel]
        public string Tel { get; set; }
    }
    public class MPLoginInptDto
    {
        [Required(ErrorMessage = "code不能为空")]
        public string code { get; set; }
    }
    /// <summary>
    /// 微信小程序获取手机信息结构 
    /// </summary>
    public class EncryptedPhoneNumModel : MPLoginInptDto
    {
        public string encryptedData { get; set; }
        public string iv { get; set; }
    }
    public class BindingEncryptedPhoneNumModel : EncryptedPhoneNumModel
    {
        public bool IsBingding { get; set; }
    }
}
