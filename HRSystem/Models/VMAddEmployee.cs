using Foolproof;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRSystem.Models
{
    public class VMAddEmployee
    {

        //asp table
      /*  [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PasswordLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(NubiHR), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(NubiHR), Name = "ConfirmPassword")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "ComparePassword")]
        public string ConfirmPassword { get; set; } */


        //Employee table

        public string IdEmployee { get; set; }
        

        [Display(ResourceType = typeof(NubiHR), Name = "FirstName")]
        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PleaseInsertEnglishLettersOnly")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "LastName")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PleaseInsertEnglishLettersOnly")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Mobile")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?((0){1})\)?[-. ]?((1|9){1})[-. ]?([0-9]{8})$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "MobileValidation")]
        public string Mobile1 { get; set; }

        
        [Display(ResourceType = typeof(NubiHR), Name = "Mobile2")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?((0){1})\)?[-. ]?((1|9){1})[-. ]?([0-9]{8})$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "MobileValidation")]
        public string Mobile2 { get; set; }

        ///[Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "PersonalEmail")]
        [EmailAddress]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "EmailValidation")]
       // [EmailAddress(ErrorMessage = "Invalid Email Address")]
       // [RegularExpression(@"^([0-9a-zA-Z]+[-._+&amp;])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "EmailValidation")]
        public string EmailEmployee { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Address")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PleaseInsertEnglishLettersOnly")]
        public string Address { get; set; }
        public int DepartmentNoEmployee { get; set; }
        public int PositionNoEmployee { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "FunctionalNumber")]
        public int FunctionalNumber { get; set; }
        public int AttachmentNo { get; set; }
       // public Nullable<int> BankNo { get; set; }
        public int SalaryNoEmployee { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "StartDate")]
        [DataType(DataType.Date)]        
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public bool IsDeletedEmployee { get; set; }

        public System.DateTime DateEmployee { get; set; }

       

        //salary table
        public int IdSalary { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Salary")]
        public double BasicSalary { get; set; }
        public Nullable<double> Bonus { get; set; }
        public bool IsDeletedSalary { get; set; }
        public System.DateTime DateSalary { get; set; }


        
        //bank table
        public int IdBank { get; set; }

        [RequiredIfNotEmpty("BankBranch", DependentPropertyDisplayName = "BankName", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "AccountNumber")]
        public string AccountNumber { get; set; }

        
        [RequiredIfNotEmpty("AccountNumber", DependentPropertyDisplayName = "AccountNumber", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "BankName")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PleaseInsertEnglishLettersOnly")]
        public string BankName { get; set; }

        [RequiredIfNotEmpty("BankName", DependentPropertyDisplayName = "BankName", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "BankBranch")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PleaseInsertEnglishLettersOnly")]
        public string BankBranch { get; set; }
        public Nullable<bool> IsDeletedBank { get; set; }
        //public Nullable<System.DateTime> DateBank { get; set; }



        //attachment table
        
        public List<Attachment> AttachmentList { get; set; }
        public int IdAttachment { get; set; }
        public Attachment EmpAttachment { get; set; }
        [Display(ResourceType = typeof(NubiHR), Name = "Name")]
        //[FileExtensions("jpg,jpeg,png")]
        // [DataType(DataType.Upload)]
        public HttpPostedFileBase Name { get; set; }
        

        [Display(ResourceType = typeof(NubiHR), Name = "ExpirationDate")]

        //[Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        public System.DateTime ExpirationDate { get; set; }
        public Nullable<bool> IsExpired { get; set; }
        
        public bool IsDeletedAttachment { get; set; }
        public System.DateTime DateAttachment { get; set; }

        



        //Department table

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Department")]
        public int IdDepartment { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "DepartmentInEnglish")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PleaseInsertEnglishLettersOnly")]
        public string DepartmentNameEn { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "DepartmentInArabic")]
        [RegularExpression(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FF ]*$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PleaseInsertArabicLettersOnly")]
        public string DepartmentNameAr { get; set; }
        bool IsDeletedDepartment { get; set; }
        public System.DateTime DateDepartment { get; set; }

        
        
        //extra salary table
        public int IdExtraSalary { get; set; }
        public int SalaryNoExtraSalary { get; set; }
        public double ExtraHours { get; set; }
        public double Amount { get; set; }
        public bool IsDeletedExtraSalary { get; set; }
        public System.DateTime DateExtraSalary { get; set; }


        //position table

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Position")]
        public int IdPosition { get; set; }
        public int DepartmentNoPosition { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "PositionInEnglish")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PleaseInsertEnglishLettersOnly")]
        public string PositionNameEn { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "PositionInArabic")]
        [RegularExpression(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FF ]*$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "PleaseInsertArabicLettersOnly")]
        public string PositionNameAr { get; set; }


        //TypesOfAttachment table

        
        [Display(ResourceType = typeof(NubiHR), Name = "AttachmentType")]
        public string AttachmentType { get; set; }

        //emergency contact table
        public List<EmergencyContact> EmergencyContactList { get; set; }

        public int IdEmergencyContact { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "Name")]
        public string NameOfSibling { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "Mobile")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?((0){1})\)?[-. ]?((1|9){1})[-. ]?([0-9]{8})$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "MobileValidation")]
        public string MobileEmergencyContact { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "RelationshipType")]
        public string Other { get; set; }
        public bool IsDeletedEmergencyContact { get; set; }
        public System.DateTime DateEmergencyContact { get; set; }

        //RelationshipType table

       // [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "RelationshipType")]
        public int? IdRelationshipType { get; set; }
        public string RelationEn { get; set; }
        public string RelationAr { get; set; }
    }
}