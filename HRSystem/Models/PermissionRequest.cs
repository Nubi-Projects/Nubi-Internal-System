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
    
    public partial class PermissionRequest
    {
        public long Id { get; set; }
        public string EmployeeNo { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public System.DateTime PermissionDate { get; set; }
        public string Note { get; set; }
        public Nullable<bool> LeaderApprovement { get; set; }
        public Nullable<bool> ManagerApprovement { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public int PermissionTypeNo { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual PermissionType PermissionType { get; set; }
    }
}
