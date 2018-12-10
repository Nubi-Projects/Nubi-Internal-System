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
        //Employee table

        public string IdEmployee { get; set; }
        

        [Display(ResourceType = typeof(NubiHR), Name = "FirstName")]
        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "LastName")]
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

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailEmployee { get; set; }

        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Address")]
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

        //[Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "AccountNumber")]
        public string AccountNumber { get; set; }

        //[Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "BankName")]
        public string BankName { get; set; }

        //[Required]
        [Display(ResourceType = typeof(NubiHR), Name = "BankBranch")]
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
        public IList<SelectListItem> DepartmentNameEn { get; set; }
        public IList<SelectListItem> DepartmentNameAr { get; set; }
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
        public string PositionNameEn { get; set; }
        public string PositionNameAr { get; set; }


        //TypesOfAttachment table

        
        [Display(ResourceType = typeof(NubiHR), Name = "AttachmentType")]
        public string AttachmentType { get; set; }

        //emergency contact table
        public List<EmergencyContact> EmergencyContactList { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "RelationshipType")]
        public string IdRelationship { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "Name")]
        public string NameOfSibling { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "Mobile")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?((0){1})\)?[-. ]?((1|9){1})[-. ]?([0-9]{8})$", ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "MobileValidation")]
        public string MobileEmergencyContact { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "RelationshipType")]
        public string Other { get; set; }

    }
}