using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRSystem.Models;
using Microsoft.AspNet.Identity;
using System.Globalization;

namespace HRSystem.Controllers
{
    public class VacationRequestController : BaseController
    {
        NubiDBEntities db = new NubiDBEntities();
        // GET: VacationRequest
        public ActionResult Index()
        {
           // var current = System.Globalization.CultureInfo.CurrentCulture;
           // current.DateTimeFormat.Calendar = new GregorianCalendar();
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
           // var current = System.Globalization.CultureInfo.CurrentCulture;
           // current.DateTimeFormat.Calendar = new GregorianCalendar();
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
           // var current = System.Globalization.CultureInfo.CurrentCulture;
           // current.DateTimeFormat.Calendar = new GregorianCalendar();
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
        [HttpGet]
        public ActionResult Create()
        {
            Manager.RequestManager mng = new Manager.RequestManager();
            var chk = mng.CheckFivedaysVaction(id: User.Identity.GetUserId());
            if (chk == false)
            {
                TempData["check"] = Resources.NubiHR.YouDidNotCompleteMonthYet;
                return RedirectToAction("Index");
            }
            var vac = mng.AvailableVacation(id: User.Identity.GetUserId());
            if (vac== false)
            {
                TempData["vac"] = Resources.NubiHR.YouDidNotComplete14DaysFromTheLastVacationYouTook;
                return RedirectToAction("Index");
            }
            //ViewBag.EmpNo = new SelectList(db.Employees.ToList(), "Id", "FirstName");
            ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type");
            ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "Id", "FirstName");
            //ViewBag.emp = db.AspNetUsers.ToList();
            return View();
        }

        //
        // POST: /VacationRequest/Create
        [HttpPost]
        public ActionResult Create(VacationRequest vac , string StartDateAr, string AlternativeEmp = "")
        {

            ModelState.Remove("StartDate");
            Manager.RequestManager mng = new Manager.RequestManager();
            var NoOfYear = mng.NoOfEmployeeYears(id: User.Identity.GetUserId());
            var total = mng.totalVacationDuration(id: User.Identity.GetUserId());
            var EmpDays = mng.NoOfEmployeeDays(id: User.Identity.GetUserId());
            var HaveTheEmpFatherDeathVacation = mng.EmployeeHaveFatherDeathVacation(id: User.Identity.GetUserId());
            var HaveTheEmpMotherDeathVacation = mng.EmployeeHaveMotherDeathVacation(id: User.Identity.GetUserId());
            var AvailableAccidentalLeave = mng.AvailableAccidentalLeave(id: User.Identity.GetUserId());
            var NoOfAccidentalLeaves = mng.NoOfAccidentalLeave(id: User.Identity.GetUserId());
            string CurrentUser = User.Identity.GetUserId();
            vac.EmployeeNo = db.AspNetUsers.Where(a => a.Id == CurrentUser).FirstOrDefault().EmpNo;
            var IsArabic = Request.Cookies["culture"].Value == "ar-SA" ? true : false;
            if(IsArabic)
            {


                CultureInfo MyCultureInfo = new CultureInfo("en-GB");
                DateTime.Parse(StartDateAr, MyCultureInfo);
                vac.StartDate = DateTime.Parse(StartDateAr, MyCultureInfo);

                //DateTime.Now.ToString("dd dddd , MMMM, yyyy", new CultureInfo("ar-AE"));
                /*vac.StartDate =new DateTime(2018,12,12);*//* DateTime.Parse(StartDateAr, MyCultureInfo);*/
                //vac.StartDate = Convert.ToDateTime(StartDateAr,);
                //vac.StartDate.ToString("dd dddd , MMMM, yyyy", new CultureInfo("en-US"));

                //CultureInfo MyCultureInfo = new CultureInfo("en-US");
                //MyCultureInfo.DateTimeFormat.Calendar = new HijriCalendar();
                //vac.StartDate = DateTime.Parse(StartDateAr, MyCultureInfo);

                //CultureInfo arSA = new CultureInfo("ar-SA");
                //arSA.DateTimeFormat.Calendar = new HijriCalendar();
                //vac.StartDate = DateTime.ParseExact(StartDateAr, "dd/MM/yyyy", arSA);

            }

            //CultureInfo arSA = new CultureInfo("ar-SA");
            //arSA.DateTimeFormat.Calendar = new HijriCalendar();
            //var dateValue = DateTime.ParseExact("29/08/1434", "dd/MM/yyyy", arSA);

            if ((vac.StartDate < DateTime.Today && vac.VacationTypeNo == 1) ||(vac.StartDate < DateTime.Today && vac.VacationTypeNo == 3) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 5) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 6))
            {
                ModelState.AddModelError("StartDate", "Start date not valid");
            }
            //else if ((vac.EndDate < DateTime.Today && vac.VacationTypeNo == 1) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 3) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 5) || (vac.StartDate < DateTime.Today && vac.VacationTypeNo == 6))
            //{
            //    ModelState.AddModelError("EndDate", "End Date not valid");
            //}
            else if (vac.VacationTypeNo == 3 && vac.Duration > 1)
            {
                ModelState.AddModelError("Duration", Resources.NubiHR.JustOneDayAllowedForAccidentalLeave);
            }
            else
            { 
            if (ModelState.IsValid)
            {
                if (vac.VacationTypeNo == 1 && vac.Duration > 5 && (total <= NoOfYear))
                {
                    TempData["checkk"] = Resources.NubiHR.YouCantTakeMoreThan5DaysUntilCompleteOneYear;

             
                    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);
                    ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "Id", "FirstName", vac.AlternativeEmp);
                        //db.Employees.ToList();

                 
                }
                else if (vac.VacationTypeNo == 1 && (total >= NoOfYear) || vac.VacationTypeNo == 1 && EmpDays < 92)
                {
                        TempData["normal"] = Resources.NubiHR.YouCantTakeNormalLeave;


                        ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);
                        ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "Id", "FirstName", vac.AlternativeEmp);
                        //ViewBag.emp = db.AspNetUsers.ToList();
                    }
                else if (vac.VacationTypeNo == 5 && HaveTheEmpFatherDeathVacation == true)
                {
                    TempData["CheckFatherDeathVacation"] = Resources.NubiHR.YouCantTakeThisVacationYouAlreadyTakeFatherDeathVacation;
                    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);
                    ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "Id", "FirstName", vac.AlternativeEmp);
                        //ViewBag.emp = db.AspNetUsers.ToList();
                    }
                else if (vac.VacationTypeNo == 6 && HaveTheEmpMotherDeathVacation == true)
                {
                    TempData["CheckMotherDeathVacation"] = Resources.NubiHR.YouCantTakeThisVacationYouAlreadyTakeFatherDeathVacation;
                    ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);
                    ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "Id", "FirstName", vac.AlternativeEmp);
                        //ViewBag.emp = db.AspNetUsers.ToList();
                    }
                else if (vac.VacationTypeNo == 3 && (NoOfAccidentalLeaves == NoOfYear))
                    {
                        TempData["AccidentalLeave"] = Resources.NubiHR.YouTook3AccidentalLeavesInYearYouCantTakeAccidentalLeave;
                        ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);
                        ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "Id", "FirstName", vac.AlternativeEmp);
                    }
                else if ((vac.VacationTypeNo == 3 && AvailableAccidentalLeave == false && (NoOfAccidentalLeaves < NoOfYear)))
                    {
                        TempData["AccLeave"]= Resources.NubiHR.YouAreNotAvailableToTakeAccidentalLeave;
                        ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);
                        ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "Id", "FirstName", vac.AlternativeEmp);
                    }
                else
                {
                        List<DateTime> VacationsDate = new List<DateTime>();
                        var date = vac.StartDate;
                        var duration = vac.Duration;

                        while (vac.VacationTypeNo == 1 && VacationsDate.Count < vac.Duration )
                            {
                            //var day = date.DayOfWeek;
                            //if (day == DayOfWeek.Friday)
                            //{
                            //    VacationsDate.Add(date);
                            //    vac.Duration = vac.Duration + 1;
                            //    date = date.AddDays(1);


                            //}
                            //else if (day == DayOfWeek.Saturday)
                            //{
                            //    VacationsDate.Add(date);
                            //    vac.Duration = vac.Duration + 1;
                            //    date = date.AddDays(1);


                            //}
                            //else
                            //{
                            //    VacationsDate.Add(date);
                            //    date = date.AddDays(1);

                            //}
                            VacationsDate.Add(date);
                            date = date.AddDays(1);
                        }
                            
                       
                        vac.EndDate = VacationsDate.LastOrDefault();
                        //if(vac.EndDate.DayOfWeek == DayOfWeek.Friday)
                        //{
                        //    vac.EndDate = vac.EndDate.AddDays(1);
                        //}
                       
                       
                        //vac.EndDate = vac.StartDate.AddDays((vac.Duration) - 1);
                        //vac.EndDate = end_date.Subtract(end_date.Day);
                        var NextDay = vac.EndDate.AddDays(1);
                        if (NextDay.DayOfWeek == DayOfWeek.Friday)
                        {
                            vac.ResumeDate = NextDay.AddDays(2);
                        }
                        else if(NextDay.DayOfWeek == DayOfWeek.Saturday)
                        {
                            vac.ResumeDate = NextDay.AddDays(1);
                        }
                        else
                        {
                            vac.ResumeDate = NextDay;
                        }
                      
                        var emp = db.Employees.FirstOrDefault(p => p.Id == AlternativeEmp);
                        vac.AlternativeEmp = emp.FirstName;
                        vac.RequestDate = DateTime.Now;
                        vac.LeaderApprovement = true;
                        TempData["chec"] = Resources.NubiHR.YourRequestHasBeenSented;
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
            //ViewBag.EmpNo = new SelectList(db.Employees.ToList(), "Id", "FirstName");
            ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID", "Type", vac.VacationTypeNo);
            ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(),"Id", "FirstName", vac.AlternativeEmp);
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
            
                ViewBag.VacationTypeNo = new SelectList(db.VacationTypes, "ID", "Type");
            ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "FirstName", vac.AlternativeEmp);

            //ViewBag.VacationTypeNo = new SelectList(db.VacationTypes.ToList(), "ID" , "Type", vac.VacationTypeNo);
            //ViewBag.emp = db.AspNetUsers.ToList();
            return View(vac);
        }

        // POST: /VacationRequest/Edit
        [HttpPost]
        public ActionResult Edit(int id, VacationRequest vac, string StartDateAr)
        {
            vac.Id = id;
            string CurrentUser = User.Identity.GetUserId();
            vac.EmployeeNo = db.AspNetUsers.Where(a => a.Id == CurrentUser).FirstOrDefault().EmpNo;
            var IsArabic = Request.Cookies["culture"].Value == "ar" ? true : false;
            if (IsArabic)
            {
                CultureInfo MyCultureInfo = new CultureInfo("en-GB");
                DateTime.Parse(StartDateAr, MyCultureInfo);
                vac.StartDate = DateTime.Parse(StartDateAr, MyCultureInfo);
            }
            if (ModelState.IsValid)
            {
                TempData["Edit"] = Resources.NubiHR.YourRequestHasBeenModified;
                db.Entry(vac).State = System.Data.Entity.EntityState.Modified;
                ViewBag.VacationTypeNo = new SelectList(db.VacationTypes, "ID", "Type");
                ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "Id", "FirstName", vac.AlternativeEmp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VacationTypeNo = new SelectList(db.VacationTypes, "ID", "Type");
            ViewBag.AlternativeEmp = new SelectList(db.Employees.ToList(), "Id", "FirstName", vac.AlternativeEmp);
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