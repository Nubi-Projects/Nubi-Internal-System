using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRSystem.Models;
using Microsoft.AspNet.Identity;
namespace HRSystem.Controllers
{
    public class PermissionRequestController : Controller
    {
        NubiDBEntities db = new NubiDBEntities();

        // GET: PermissionRequest
        public ActionResult Index()
        {
            Manager.RequestManager mng = new Manager.RequestManager();
            ViewBag.per = mng.GetPermission(id: User.Identity.GetUserId());
            return View(db.PermissionRequests.ToList());
        }
        public ActionResult LeaderRequests()
        {
            return View(db.PermissionRequests.ToList());
        }
        public ActionResult LeaderApprove(int id)
        {
            PermissionRequest per = db.PermissionRequests.Find(id);
            if (ModelState.IsValid)
            {
                per.LeaderApprovement = true;
                db.Entry(per).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LeaderRequests");
            }
            return View(db.PermissionRequests.ToList());
        }
        public ActionResult Reject(int id)
        {
            PermissionRequest per = db.PermissionRequests.Find(id);
            if (ModelState.IsValid)
            {
                per.IsRejected = true;
                db.Entry(per).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LeaderRequests");
            }
            return View(db.PermissionRequests.ToList());
        }
        public ActionResult ManagerRequests()
        {
            return View(db.PermissionRequests.ToList());
        }

        public ActionResult ManagerApprove(int id)
        {
            PermissionRequest per = db.PermissionRequests.Find(id);
            if (ModelState.IsValid)
            {
                per.ManagerApprovement = true;
                db.Entry(per).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManagerRequests");
            }
            return View(db.PermissionRequests.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "ID", "Type");
            return View();
        }

        //
        // POST: /PermissionRequest/Create
        [HttpPost]
        public ActionResult Create(PermissionRequest per)
        {
            var CurrentUser = User.Identity.GetUserId();
            per.EmployeeNo = db.AspNetUsers.Where(a => a.Id == CurrentUser).FirstOrDefault().Id;
            if (ModelState.IsValid)
            {
                per.RequestDate = DateTime.Now;
                db.PermissionRequests.Add(per);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "ID", "Type");
            return View();
        }


        //GET: /PermissionRequest/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                HttpNotFound();
            }
            PermissionRequest per = db.PermissionRequests.Find(id);
            if (per == null)
            {
               return HttpNotFound();
            }

            ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "ID","Type",per.PermissionTypeNo);
            return View(per);
        }

        // POST: /PermissionRequest/Edit
        [HttpPost]
        public ActionResult Edit(int id, PermissionRequest per)
        {
            per.Id = id;
            per.RequestDate = DateTime.Now;
            string CurrentUser = User.Identity.GetUserId();
            per.EmployeeNo = db.AspNetUsers.Where(a => a.EmpNo == CurrentUser).FirstOrDefault().Id;
            if (ModelState.IsValid)
            {
                db.Entry(per).State = System.Data.Entity.EntityState.Modified;
                ViewBag.permission = new SelectList(db.PermissionTypes.ToList(), "ID","Type", per.PermissionTypeNo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.permission = new SelectList(db.PermissionTypes.ToList(), "ID", "Type", per.PermissionTypeNo);
            return View(per);
        }

        public ActionResult Details (int? id)
        {
            return View(db.PermissionRequests.Find(id));
        }
        //
        // GET: /PermissionRequest/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                HttpNotFound();
            }
            PermissionRequest per = db.PermissionRequests.Find(id);
            if (per == null)
            {
                HttpNotFound();
            }
            return View(per);
        }

        // POST: /PermissionRequest/Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            PermissionRequest per = db.PermissionRequests.Find(id);
            if (ModelState.IsValid)
            {
                per.IsDeleted = true;
                db.Entry(per).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index");
        }
    }
}