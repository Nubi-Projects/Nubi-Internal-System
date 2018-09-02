using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRSystem.Models;

namespace HRSystem.Controllers
{
 
    public class VacationTypeController : Controller
    {
        NubiDBEntities db = new NubiDBEntities();
        // GET: VacationType
        public ActionResult Index()
        {
            return View(db.VacationTypes.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /VacationType/Create
        [HttpPost]
        public ActionResult Create(VacationType vacation)
        {
            vacation.Date = DateTime.Now;
            db.VacationTypes.Add(vacation);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        // GET: /VacationType/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                HttpNotFound();
            }
            VacationType vacation = db.VacationTypes.Find(id);
            if (vacation == null)
            {
                HttpNotFound();
            }
            return View(vacation);
        }

        //
        // POST: /VacationType/Edit
        [HttpPost]
        public ActionResult Edit(int id ,VacationType vacation)
        {
            vacation.ID = id;
            if (ModelState.IsValid)
            {
                db.Entry(vacation).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vacation);
        }

        //
        // GET: /VacationType/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                HttpNotFound();
            }
            VacationType vacation = db.VacationTypes.Find(id);
            if (vacation == null)
            {
                HttpNotFound();
            }
            return View(vacation);
        }

        // POST: /VacationType/Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            VacationType vac = db.VacationTypes.Find(id);
            if(ModelState.IsValid)
            {
                vac.IsDeleted = true;
                db.Entry(vac).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index");
        }
    }
}