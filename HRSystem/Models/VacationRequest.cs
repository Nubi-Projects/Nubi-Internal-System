//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRSystem.Models
{
    using System;
    using System.Collections.Generic;
    using Resources;
    using System.ComponentModel.DataAnnotations;

    public partial class VacationRequest
    {
        public long Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(NubiHR), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NubiHR), Name = "StartDate")]
        [DataType(DataType.Date)]
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string Address { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string Note { get; set; }
        public System.DateTime ResumeDate { get; set; }
        public bool IsDeleted { get; set; }
        public string EmployeeNo { get; set; }
        public int Duration { get; set; }
        public Nullable<bool> LeaderApprovement { get; set; }
        public Nullable<bool> ManagerApprovement { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public int VacationTypeNo { get; set; }
        public string MedicalReport { get; set; }
        public Nullable<bool> LeaderHasSeen { get; set; }
        public Nullable<bool> ManagerHasSeen { get; set; }
        public string AlternativeEmp { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual VacationType VacationType { get; set; }
    }
}
