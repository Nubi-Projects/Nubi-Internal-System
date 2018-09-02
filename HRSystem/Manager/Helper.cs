using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRSystem.Models;

namespace HRSystem.Manager
{
    public class Helper
    {
            NubiDBEntities db = new NubiDBEntities();
            public AspNetUser GetName(string id)
            {
                return db.AspNetUsers.Where(a => a.EmpNo == id).FirstOrDefault();
            }

            public string GetUserRole(string id)
            {
                return db.AspNetUsers.Where(a => a.Id == id).FirstOrDefault().AspNetRoles.FirstOrDefault().Id;
            }
    }
}