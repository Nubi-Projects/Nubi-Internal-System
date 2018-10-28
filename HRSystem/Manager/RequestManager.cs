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

            if (VacReq != null && VacReq.ResumeDate.AddDays(14) >= DateTime.Today)
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
            if (FDV != null)
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
            if (MDV != null)
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