﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRSystem.Models;
using Microsoft.AspNet.Identity;

namespace HRSystem.Controllers
{
    public class VacationRequestController : BaseController
    {
        NubiDBEntities db = new NubiDBEntities();
        // GET: VacationRequest
        public ActionResult Index()
        {
            return View(db.VacationRequests.ToList());
        }
        public ActionResult LeaderRequests()
        {
            //List<VacationRequest> vacation
            var vacation = db.VacationRequests.Where(x => x.LeaderApprovement == null).ToList();
            foreach (VacationRequest vac in vacation)

            {

                VacationRequest Existing_vac = db.VacationRequests.Find(vac.Id);

                Existing_vac.LeaderHasSeen = true;

            }
            db.SaveChanges();
            return View(db.VacationRequests.OrderBy(e => e.RequestDate));
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
            var vacation = db.VacationRequests.Where(x => x.LeaderApprovement == true).ToList();
            foreach (VacationRequest vac in vacation)

            {

                VacationRequest Existing_vac = db.VacationRequests.Find(vac.Id);

                Existing_vac.ManagerHasSeen = true;

            }
            db.SaveChanges();
            return View(db.VacationRequests.OrderBy(e => e.RequestDate));
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
            if (vac== false)
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
            var CompleteYear = mng.CheckVaction(id: User.Identity.GetUserId());
            var HaveTheEmpFatherDeathVacation = mng.EmployeeHaveFatherDeathVacation(id: User.Identity.GetUserId());
            var HaveTheEmpMotherDeathVacation = mng.EmployeeHaveMotherDeathVacation(id: User.Identity.GetUserId());
            var AvailableAccidentalLeave = mng.AvailableAccidentalLeave(id: User.Identity.GetUserId());
            var NoOfAccidentalLeaves = mng.NoOfAccidentalLeave(id: User.Identity.GetUserId());
            string CurrentUser = User.Identity.GetUserId();
            vac.EmployeeNo = db.AspNetUsers.Where(a => a.Id == CurrentUser).FirstOrDefault().EmpNo;
            if((vac.StartDate < DateTime.Today && vac.VacationTypeNo == 1) ||(vac.StartDate < DateTime.Today && vac.VacationTypeNo == 3) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 5) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 6))
            {
                ModelState.AddModelError("StartDate", "Start date not valid");
            }
            else if ((vac.EndDate < DateTime.Today && vac.VacationTypeNo == 1) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 3) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 5) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 6))
            {
                ModelState.AddModelError("EndDate", "End Date not valid");
            }
            else if (vac.VacationTypeNo == 3 && vac.Duration > 1)
            {
                ModelState.AddModelError("Duration", "Just one day allowed for accidental leave");
            }
            else
            { 
            if (ModelState.IsValid)
            {
                if ( vac.Duration > 5 && CompleteYear == false )
                {
                    TempData["checkk"] = "You Can Not Take More Than 5 Days Until Complete one Year!";

             
                    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);

                    ViewBag.emp = db.AspNetUsers.ToList();

                 
                }
                else if (vac.VacationTypeNo == 5 && HaveTheEmpFatherDeathVacation == true)
                {
                    TempData["CheckFatherDeathVacation"] = "You Can Not Take This Vacation You Have Already Taken Father's Death Vacation";
                    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);

                    ViewBag.emp = db.AspNetUsers.ToList();
                }
                else if (vac.VacationTypeNo == 6 && HaveTheEmpMotherDeathVacation == true)
                {
                    TempData["CheckMotherDeathVacation"] = "You Can Not Take This Vacation You Have Already Taken Mother's Death Vacation";
                    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);

                    ViewBag.emp = db.AspNetUsers.ToList();
                }
                else if (vac.VacationTypeNo == 3 && NoOfAccidentalLeaves == true && CompleteYear == false)
                    {
                        TempData["AccidentalLeave"] = "You Took 3 Accidental Leaves In This Year You Can't Take Accidental Leave";
                        ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);
                        ViewBag.emp = db.AspNetUsers.ToList();
                    }
                else if ((vac.VacationTypeNo == 3 && AvailableAccidentalLeave == false && NoOfAccidentalLeaves == false && CompleteYear == false) || (vac.VacationTypeNo == 3 && AvailableAccidentalLeave == false && NoOfAccidentalLeaves == false && CompleteYear == true))
                    {
                        TempData["AccLeave"]= "You can't take accidental leave";
                    }
                else
                {
                        vac.EndDate = vac.StartDate.AddDays(vac.Duration);
                        vac.ResumeDate = vac.EndDate.AddDays(1);
                        vac.RequestDate = DateTime.Now;
                        TempData["chec"] = "Your Request Has Been Sented";
                        db.VacationRequests.Add(vac);
                    db.SaveChanges();
                    return RedirectToAction("index");
                }
                //ModelState.AddModelError("MedicalReport","ple;lsjkjdfhshdkn ksjdf sffkh jkwhe fshdf owihuhdf owhjf ihf wieofh woeif")



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
            vac.EmployeeNo = db.AspNetUsers.Where(a => a.Id == CurrentUser).FirstOrDefault().EmpNo;
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