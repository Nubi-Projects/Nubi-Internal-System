using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRSystem.Models;
using Microsoft.AspNet.Identity;

namespace HRSystem.Controllers
{
    public class VacationRequestController : Controller
    {
        NubiDBEntities db = new NubiDBEntities();
        // GET: VacationRequest
        public ActionResult Index()
        {
            return View(db.VacationRequests.ToList());
        }
        public ActionResult LeaderRequests()
        {
            return View(db.VacationRequests.ToList());
        }
        public ActionResult LeaderApprove(int id)
        {
            VacationRequest vac = db.VacationRequests.Find(id);
            if (ModelState.IsValid)
            {
                vac.LeaderApprovement = true;
                db.Entry(vac).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LeaderRequests");
            }
            return View(db.VacationRequests.ToList());
        }
        public ActionResult Reject(int id)
        {
            VacationRequest vac = db.VacationRequests.Find(id);
            if (ModelState.IsValid)
            {
                vac.IsRejected = true;
                db.Entry(vac).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LeaderRequests");
            }
            return View(db.VacationRequests.ToList());
        }
        public ActionResult ManagerRequests()
        {
            return View(db.VacationRequests.ToList());
        }
        public ActionResult ManagerApprove(int id)
        {
            VacationRequest vac = db.VacationRequests.Find(id);
            if (ModelState.IsValid)
            {
                vac.ManagerApprovement = true;
                db.Entry(vac).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManagerRequests");
            }
            return View(db.VacationRequests.ToList());
        }
        public ActionResult Create()
        {
            Manager.RequestManager mng = new Manager.RequestManager();
            var chk = mng.CheckFivedaysVaction(id: User.Identity.GetUserId());
            if (chk == false)
            {
                TempData["check"] = "You did not complete Month Yet!";
                return RedirectToAction("Index");
            }
            var vac = mng.AvailableVacation(id: User.Identity.GetUserId());
            if (vac== true)
            {
                TempData["vac"] = "You Did Not complete 14 Days From The Last Vacation You Took!";
                return RedirectToAction("Index");
            }
            ViewBag.EmpNo = new SelectList(db.Employees.ToList(), "Id", "FirstName");
            ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type");
            ViewBag.emp = db.AspNetUsers.ToList();
            return View();
        }

        //
        // POST: /VacationRequest/Create
        [HttpPost]
        public ActionResult Create(VacationRequest vac)
        {
            Manager.RequestManager mng = new Manager.RequestManager();
            var v = mng.CheckVaction(id: User.Identity.GetUserId());
            var HaveTheEmpFatherDeathVacation = mng.EmployeeHaveFatherDeathVacation(id: User.Identity.GetUserId());
            string CurrentUser = User.Identity.GetUserId();
            vac.EmployeeNo = db.AspNetUsers.Where(a => a.Id == CurrentUser).FirstOrDefault().EmpNo;
            if (ModelState.IsValid)
            {
                if ( vac.Duration > 5 && v == false )
                {
                    TempData["chec"] = "You Can Not Take More Than 5 Days Until Complete one Year!";

             
                    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);

                    ViewBag.emp = db.AspNetUsers.ToList();

                 
                }
                else if (vac.VacationTypeNo == 5 && HaveTheEmpFatherDeathVacation == true)
                {
                    TempData["CheckFatherDeathVacation"] = "You Can Not Take This Vacation You Have Already Taken Father's Death Vacation";
                    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);

                    ViewBag.emp = db.AspNetUsers.ToList();
                }
                else
                {
                    TempData["chec"] = "Your Request Has Been Sented";
                    vac.RequestDate = DateTime.Now;
                    db.VacationRequests.Add(vac);
                    db.SaveChanges();
                    return RedirectToAction("index");
                }



                //if(vac.VacationTypeNo == 5 && HaveTheEmpFatherDeathVacation == true)
                //{
                //    TempData["CheckFatherDeathVacation"] = "You Can Not Take This Vacation You Have Already Taken Father's Death Vacation";
                //    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);

                //    ViewBag.emp = db.AspNetUsers.ToList();

                 
                //}
                //else
                //{
                //    TempData["CheckFatherDeathVacation"] = "Your Request Has Been Sented";
                //    vac.RequestDate = DateTime.Now;
                //    db.VacationRequests.Add(vac);
                //    db.SaveChanges();
                //    return RedirectToAction("index");
                //}
            }
            ViewBag.EmpNo = new SelectList(db.Employees.ToList(), "Id", "FirstName");
            ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);
            ViewBag.emp = db.AspNetUsers.ToList();       
            return View();       
          
          
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                HttpNotFound();
            }
            VacationRequest vac = db.VacationRequests.Find(id);
            if (vac == null)
            {
                HttpNotFound();
            }
            
                ViewBag.VacationTypeNo = new SelectList(db.VacationTypes, "ID", "Type", vac.VacationTypeNo);
           

            //ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID" , "Type", vac.VacationTypeNo);
            ViewBag.emp = db.AspNetUsers.ToList();
            return View(vac);
        }

        // POST: /VacationRequest/Edit
        [HttpPost]
        public ActionResult Edit(int id, VacationRequest vac)
        {
            vac.Id = id;
            vac.RequestDate = DateTime.Now;
            string CurrentUser = User.Identity.GetUserId();
            vac.EmployeeNo = db.AspNetUsers.Where(a => a.EmpNo == CurrentUser).FirstOrDefault().Id;
            if (ModelState.IsValid)
            {
                db.Entry(vac).State = System.Data.Entity.EntityState.Modified;
               
                    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes, "ID", "Type", vac.VacationTypeNo);
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VacationTypeNo = new SelectList(db.VacationTypes, "ID", "Type", vac.VacationTypeNo);
            return View(vac);
        }

        public ActionResult Details(int? id)
        {
            return View(db.VacationRequests.Find(id));
        }

        // GET: /VacationRequest/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                HttpNotFound();
            }
            VacationRequest per = db.VacationRequests.Find(id);
            if (per == null)
            {
                HttpNotFound();
            }
            return View(per);
        }

        // POST: /VacationRequest/Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            VacationRequest vac = db.VacationRequests.Find(id);
            if (ModelState.IsValid)
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