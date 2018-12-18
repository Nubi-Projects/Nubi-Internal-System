using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRSystem.Models
{
    public class AttendanceViewModel
    {
        //[Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "Import")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ExcelFile{ get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "Automatic")]
        public bool Automatic { get; set; }

        [Display(ResourceType = typeof(NubiHR), Name = "Exclude")]
        public bool Exclude { get; set; }

        public string Number { get; set; }
        public string Name { get; set; }
        public string PunchTime { get; set; }
        public string WorkState { get; set; }
        public string Terminal { get; set; }
        public string PunchType { get; set; }
        public string NoOfHours { get; set; }
        public string RemainingHours { get; set; }
    }
}