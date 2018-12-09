using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRSystem.Manager
{
    public class Enums
    {
        public enum VacationType
        {
            
        }


        public enum PermissionType
        {
            Emergency = 1,
            Sick = 2,
        }
        public enum AttachmentType
        {
            //[Display(ResourceType = typeof(Resources.NubiHR), Name = "Accepted")]
            //unspecified = 0,

            [Display(ResourceType = typeof(Resources.NubiHR), Name = "NationalID")]
            Nid = 1,

            [Display(ResourceType = typeof(Resources.NubiHR), Name = "PassportNumber")]
            Passport = 2,

            [Display(ResourceType = typeof(Resources.NubiHR), Name = "LastCertificate")]
            LastCertificate = 3,

            [Display(ResourceType = typeof(Resources.NubiHR), Name = "Image")]
            Image = 4,

            [Display(ResourceType = typeof(Resources.NubiHR), Name = "TrainingCertificate")]
            TrainingCertificate = 5,


        }
    }
}