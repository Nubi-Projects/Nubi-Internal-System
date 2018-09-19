using Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRSystem.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        public string Provider { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(NubiHR), Name = "Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [EmailAddress]
        [Display(ResourceType = typeof(NubiHR), Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PasswordLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(NubiHR), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(NubiHR), Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "ComparePassword")]
        public string ConfirmPassword { get; set; }

        //[Required]
        //[Display(Name = "Job Title")]
        //public int JobTitleRoleNo { get; set; }

        //[Required]
        //[Display(Name = "RoleName")]
        //public string RoleNo{ get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [EmailAddress]
        [Display(ResourceType = typeof(NubiHR), Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PasswordLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(NubiHR), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(NubiHR), Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "ComparePassword")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [EmailAddress]
        [Display(ResourceType = typeof(NubiHR), Name = "Email")]
        public string Email { get; set; }
    }
}
