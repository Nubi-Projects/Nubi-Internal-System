using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRSystem.Models;
using OfficeOpenXml;

namespace HRSystem.Manager
{
    public class Helper
    {
        NubiDBEntities db = new NubiDBEntities();
        public AspNetUser GetName(string id)
        {
            return db.AspNetUsers.Where(a => a.Id == id).FirstOrDefault();
        }

        public string GetUserRole(string userId)
        {
            var user = db.AspNetUsers.FirstOrDefault(a => a.Id == userId);
            if (user != null)
            {
                var emp = user.Employee;
                if (emp != null)
                {
                    var position = emp.Position;
                    if (position != null)
                    {
                        var positionName = position.PositionNameAr;
                        return positionName;
                    }
                }
            }

            return "This Employee Does Not Have A Position";
        }

        public static List<ExcelWorksheet> GetWorksheets(ExcelPackage package)
        {
            var attempts = 0;
            while (attempts < 3)
            {
                try
                {
                    return package.Workbook.Worksheets.Select(s => s).ToList();
                }
                catch (Exception ex)
                {
                    
                }

                attempts++;
            }

            return new List<ExcelWorksheet>();
        }
    }
}