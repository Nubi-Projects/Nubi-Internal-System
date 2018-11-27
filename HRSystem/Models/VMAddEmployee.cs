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
        public string Mobile1 { get; set; }

        
        [Display(ResourceType = typeof(NubiHR), Name = "Mobile2")]
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



        //training certificate table
        
        
        public int IdTrainingCertificate { get; set; }
        public List<TrainingCertificate> TrainingCertificateList { get; set; }
        public string EmployeeNoTrainingCertificate { get; set; }


        [Display(ResourceType = typeof(NubiHR), Name = "TrainingCertificateTitle")]
        public string TrainingCertificateName { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "TrainingCertificate")]
        public HttpPostedFileBase TrainingCertificateUrl { get; set; }

        public System.DateTime DateTrainingCertificate { get; set; }
        public bool IsDeletedTrainingCertificate { get; set; }


       
        //attachment table

        public int IdAttachment { get; set; }
        public Attachment EmpAttachment { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "NationalIDTitle")]

        //[Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        public string NIdTitle { get; set; }

        //[Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "NationalID")]
        //[FileExtensions("jpg,jpeg,png")]
       // [DataType(DataType.Upload)]
        public HttpPostedFileBase NationalId { get; set; }


        [Display(ResourceType = typeof(NubiHR), Name = "PassportTitle")]
        public string PassportTitle { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "PassportNumber")]
        public HttpPostedFileBase PassportNumber { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "LastCertificateTitle")]
        public string LastCertTitle { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "LastCertificate")]
        public HttpPostedFileBase LastCertificate { get; set; }

        //[Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "ImageTitle")]
        public string ImageTitle { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "Image")]
        //[Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        public HttpPostedFileBase ImageUrl { get; set; }
        
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
           
        

    }
}