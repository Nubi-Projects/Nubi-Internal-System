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
            List<PermissionRequest> perReq = db.PermissionRequests.Where(e => e.PermissionDate.Month == DateTime.Now.Month && e.PermissionDate.Year == DateTime.Now.Year).ToList();
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
        public bool CheckVaction(string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var diff = GetMonthDifference(DateTime.Now, emp.StartDate);
            if (diff >= 12)
                return true;
            else
                return false;
        }
        public int? totalVacationDuration(string id)
        {
            int? TotalDuration = 0;
            List<VacationRequest> vac = db.VacationRequests.Where(e => e.EmployeeNo == id).ToList();
            foreach (var item in vac)
            {
                TotalDuration = item.Duration + TotalDuration;
            }
            return TotalDuration;
        }
        public int? TotalVacationDays(string id)
        {
            int? TotalDuration = 0;
            List<VacationRequest> vac = db.VacationRequests.Where(e => e.EmployeeNo == id).ToList();
            foreach (var item in vac)
            {
                TotalDuration = item.Duration + TotalDuration;
            }
            return 30 - TotalDuration;
        }
        public bool AvailableVacation(string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var VacReq = db.VacationRequests.OrderByDescending(f=>f.Id).FirstOrDefault(e => e.EmployeeNo == emp.Id);

            if (VacReq != null && VacReq.ResumeDate.AddDays(14) <= DateTime.Today && VacReq.IsDeleted == false)
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
            var FDV = db.VacationRequests.Where(e => e.VacationTypeNo == 5).ToList();
            if (FDV != null && FDV.Count>0)
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
            var MDV = db.VacationRequests.Where(e => e.VacationTypeNo == 6).ToList();
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
            if (AccLeave != null && AccLeave.ResumeDate.AddDays(30) < DateTime.Today || AccLeave == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool NoOfAccidentalLeave (string id)
        {
            var emp = db.AspNetUsers.Where(e => e.Id == id).FirstOrDefault().Employee;
            var NoOfAccLeave = db.VacationRequests.Where(e => e.VacationTypeNo == 3).ToList();
            if (NoOfAccLeave != null && NoOfAccLeave.Count > 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int NoOfLeaderVacationRequests ()
        {
            var EmpReq = db.VacationRequests.Count(e => e.LeaderApprovement == null);
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
            var EmpReq = db.VacationRequests.Count(e => e.LeaderApprovement == true);
            if (EmpReq >= 1)
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
            var EmpReq = db.PermissionRequests.Count(e => e.LeaderApprovement == null);
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
            var EmpReq = db.PermissionRequests.Count(e => e.LeaderApprovement == true);
            if (EmpReq >= 1)
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