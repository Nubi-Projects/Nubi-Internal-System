using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRSystem.Models;

namespace HRSystem.Controllers
{
    public class PermissionTypeController : Controller
    {
        NubiDBEntities db = new NubiDBEntities();
        // GET: PermissionType
        public ActionResult Index()
        {
            return View(db.PermissionTypes.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /PermissionType/Create
        [HttpPost]
        public ActionResult Create(PermissionType permission)
        {
            permission.Date = DateTime.Now;
            db.PermissionTypes.Add(permission);
            db.SaveChanges();
            return RedirectToAction("Create");
        }


        //GET: /PermissionType/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                HttpNotFound();
            }
            PermissionType permission = db.PermissionTypes.Find(id);
            if (permission == null)
            {
                HttpNotFound();
            }
            return View(permission);
        }

        //
        // POST: /PermissionType/Edit
        [HttpPost]
        public ActionResult Edit(int id,PermissionType permission)
        {
            permission.Id = id;
            if (ModelState.IsValid)
            {
                db.Entry(permission).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permission);
        }

        //
        // GET: /PermissionType/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                HttpNotFound();
            }
            PermissionType permission = db.PermissionTypes.Find(id);
            if (permission == null)
            {
                HttpNotFound();
            }
            return View(permission);
        }

        // POST: /PermissionType/Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            PermissionType per = db.PermissionTypes.Find(id);
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