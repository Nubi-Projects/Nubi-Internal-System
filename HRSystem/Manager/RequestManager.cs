using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRSystem.Models;
namespace HRSystem.Manager
{
    public class RequestManager
    {
        NubiDBEntities db = new NubiDBEntities();
        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        public bool GetPermission(string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            List<PermissionRequest> perReq = db.PermissionRequests.Where(e => e.PermissionDate.Month == DateTime.Now.Month &&
            e.PermissionDate.Year == DateTime.Now.Year && e.IsDeleted == false && (e.IsRejected == false || e.IsRejected == null)
            && e.ManagerApprovement == true || e.PermissionDate.Month == DateTime.Now.Month && e.PermissionDate.Year == DateTime.Now.Year &&
            e.IsDeleted == false && (e.IsRejected == false || e.IsRejected == null) && e.ManagerApprovement == null).ToList();
            if (perReq.Count < 2)
            {
                return true;
            }
            else
                return false;
        }

        public bool CheckFivedaysVaction(string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var diff = GetMonthDifference(DateTime.Now, emp.StartDate);
            if (diff >= 1)
                return true;
            else
                return false;
        }

        // this for calculate current user years
        public int NoOfEmployeeDays(string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var diff = (DateTime.Now - emp.StartDate).Days;
            //double result = diff / 12.0;
            return diff;
        }

        // this for calculate current user years
        public double NoOfEmployeeYears(string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var diff = GetMonthDifference(DateTime.Now, emp.StartDate);
            double result = diff / 12.0;
            return result;
        }
        //this for calculate each employee years
        public double NoOfEmpYears(string id)
        {
            var emp = db.Employees.Where(e => e.Id == id).FirstOrDefault();
            var diff = GetMonthDifference(DateTime.Now, emp.StartDate);
            double result = diff / 12.0;
            return result;
        }
        //public bool CheckVaction(string id)
        //{
        //    var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
        //    var diff = GetMonthDifference(DateTime.Now, emp.StartDate);
        //    if (diff >= 12)
        //        return true;
        //    else
        //        return false;
        //}

        // this to calculate all vacation days that current user has took
        public double? totalVacationDuration(string id)
        {
            int? TotalDuration = 0;
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;

            List<VacationRequest> vac = db.VacationRequests.Where(e => e.VacationTypeNo == 1 &&
            e.IsDeleted == false && (e.IsRejected == false || e.IsRejected == null) && e.ManagerApprovement == true).ToList();
            foreach (var item in vac)
            {
                TotalDuration = item.Duration + TotalDuration;
            }
            return TotalDuration / 30.0;
        }

        // this to calculate all vacation days that employee has took
        public double? totalVacDuration(string id)
        {
            int? TotalDuration = 0;
            //var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;

            List<VacationRequest> vac = db.VacationRequests.Where(e => e.VacationTypeNo == 1 &&
            e.IsDeleted == false && (e.IsRejected == false || e.IsRejected == null) && e.ManagerApprovement == true && e.EmployeeNo == id).ToList();
            foreach (var item in vac)
            {
                TotalDuration = item.Duration + TotalDuration;
            }
            return TotalDuration;
        }

        // this to calculate all vacation days that current user has took
        public double? totalVaccDuration(string id)
        {
            int? TotalDuration = 0;
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;

            List<VacationRequest> vac = db.VacationRequests.Where(e => e.VacationTypeNo == 1 &&
            e.IsDeleted == false && (e.IsRejected == false || e.IsRejected == null) && e.ManagerApprovement == true).ToList();
            foreach (var item in vac)
            {
                TotalDuration = item.Duration + TotalDuration;
            }
            return TotalDuration;
        }
        // this to calculate Employee vacations balance
        public int? DueLeave (string id)
        {
            var AnnualLeave = 30;
            var DaysOfYear = 365;
            var emp = db.Employees.Where(e => e.Id == id).FirstOrDefault();
            var TotalWorkDays = (DateTime.Now - emp.StartDate).Days;
            var DueLeaveForEmployee = AnnualLeave * TotalWorkDays / DaysOfYear;
            return DueLeaveForEmployee;
        }

        // this to calculate current user vacations balance
        public int? UserDueLeave(string id)
        {
            var AnnualLeave = 30;
            var DaysOfYear = 365;
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var TotalWorkDays = (DateTime.Now - emp.StartDate).Days;
            var DueLeaveForEmployee = AnnualLeave * TotalWorkDays / DaysOfYear;
            return DueLeaveForEmployee;
        }

        //public bool EligbleVacation ()
        //{
        //    if (totalVacationDuration < CheckVaction)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public bool EligibleAccLeav ()
        //{
        //    if(NoOfAccidentalLeave < CheckVaction)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public int? TotalVacationDays(string id)
        {
            int? TotalDuration = 0;
            //var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            List<VacationRequest> vac = db.VacationRequests.Where(e => e.VacationTypeNo == 1 &&
            e.IsDeleted == false && (e.IsRejected == false || e.IsRejected == null) && e.ManagerApprovement == true && e.EmployeeNo == id).ToList();
            foreach (var item in vac)
            {
                TotalDuration = item.Duration + TotalDuration;
            }
            if (TotalDuration > 30)
            {
                var balance = TotalDuration % 30;
                return 30 - balance;
            }
            else
            {
                return 30 - TotalDuration;
            }
            
        }
        public bool AvailableVacation(string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var VacReq = db.VacationRequests.Where(e => e.VacationTypeNo == 1 && e.IsDeleted == false &&
            (e.IsRejected == false || e.IsRejected == null) && e.ManagerApprovement == true).OrderByDescending(f=>f.Id).
            FirstOrDefault(e => e.EmployeeNo == emp.Id);

            if (VacReq != null && VacReq.ResumeDate.AddDays(14) <= DateTime.Today || VacReq == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EmployeeHaveFatherDeathVacation (string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var FDV = db.VacationRequests.Where(e => e.VacationTypeNo == 5 && e.IsDeleted == false &&
            (e.IsRejected == false || e.IsRejected == null) && e.ManagerApprovement == true).ToList();
            if (FDV != null && FDV.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EmployeeHaveMotherDeathVacation(string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var MDV = db.VacationRequests.Where(e => e.VacationTypeNo == 6 && e.IsDeleted == false &&
            (e.IsRejected == false || e.IsRejected == null) && e.ManagerApprovement == true).ToList();
            if (MDV != null && MDV.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AvailableAccidentalLeave (string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            //var AccLeave = db.VacationRequests.Where(e => e.VacationTypeNo == 3).LastOrDefault();
            var AccLeave = db.VacationRequests.OrderByDescending(e => e.VacationTypeNo == 3).FirstOrDefault(e => e.EmployeeNo == emp.Id);
            if (AccLeave != null && AccLeave.ResumeDate.AddDays(30) < DateTime.Today && AccLeave.IsDeleted == false &&
                (AccLeave.IsRejected == false || AccLeave.IsRejected == null) && AccLeave.ManagerApprovement == true || AccLeave == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public double? NoOfAccidentalLeave (string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var NoOfAccLeave = db.VacationRequests.Count(e => e.VacationTypeNo == 3 && e.IsDeleted == false && 
            (e.IsRejected == false || e.IsRejected == null) && e.ManagerApprovement == true);
            return NoOfAccLeave / 3.0; 
            
        }

        public int NoOfLeaderVacationRequests ()
        {
            var EmpReq = db.VacationRequests.Count(e => e.LeaderApprovement == null && e.IsDeleted == false);
            if (EmpReq > 0)
            {
                return EmpReq;
            }
            else
            {
                return 0;
            }
                
        }

        public int NoOfManagerVacationRequests()
        {
            var EmpReq = db.VacationRequests.Count(e => e.LeaderApprovement == true && e.IsDeleted == false);
            if (EmpReq > 0)
            {
                return EmpReq;
            }
            else
            {
                return 0;
            }

        }

        public int NoOfLeaderPermissionRequests()
        {
            var EmpReq = db.PermissionRequests.Count(e => e.LeaderApprovement == null && e.IsDeleted == false);
            if (EmpReq > 0)
            {
                return EmpReq;
            }
            else
            {
                return 0;
            }

        }

        public int NoOfManagerPermissionRequests()
        {
            var EmpReq = db.PermissionRequests.Count(e => e.LeaderApprovement == true && e.IsDeleted == false);
            if (EmpReq > 0)
            {
                return EmpReq;
            }
            else
            {
                return 0;
            }

        }

        public bool HasLeaderSeenVacationRequests()
        {
            var EmpReq = db.VacationRequests.Where(e => e.LeaderApprovement == null && e.LeaderHasSeen == true).ToList();
            if (EmpReq != null && EmpReq.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool HasLeaderSeenPermissionRequests()
        {
            var EmpReq = db.PermissionRequests.Where(e => e.LeaderApprovement == null && e.LeaderHasSeen == true).ToList();
            if (EmpReq != null && EmpReq.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool HasManagerSeenVacationRequests()
        {
            var EmpReq = db.VacationRequests.Where(e => e.ManagerApprovement == null && e.ManagerHasSeen == true).ToList();
            if (EmpReq != null && EmpReq.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool HasManagerSeenPermissionRequests()
        {
            var EmpReq = db.PermissionRequests.Where(e => e.ManagerApprovement == null && e.ManagerHasSeen == true).ToList();
            if (EmpReq != null && EmpReq.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}