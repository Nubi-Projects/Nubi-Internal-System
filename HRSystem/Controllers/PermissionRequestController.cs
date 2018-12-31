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
    public class PermissionRequestController : BaseController
    {
        NubiDBEntities db = new NubiDBEntities();

        // GET: PermissionRequest
        public ActionResult Index()
        {
            Manager.RequestManager mng = new Manager.RequestManager();
            ViewBag.per = mng.GetPermission(id: User.Identity.GetUserId());

            var current = System.Globalization.CultureInfo.CurrentCulture;
            current.DateTimeFormat.Calendar = new GregorianCalendar();
            current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            return View(db.PermissionRequests.ToList());
        }
        public ActionResult LeaderRequests()
        {
            var permission = db.PermissionRequests.Where(x => x.LeaderApprovement == null).ToList();
            foreach (PermissionRequest per in permission)

            {

                PermissionRequest Existing_per = db.PermissionRequests.Find(per.Id);

                Existing_per.LeaderHasSeen = true;

            }
            db.SaveChanges();
            var current = System.Globalization.CultureInfo.CurrentCulture;
            current.DateTimeFormat.Calendar = new GregorianCalendar();
            current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            return View(db.PermissionRequests.OrderBy(e => e.RequestDate));
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
            return View(db.PermissionRequests.OrderBy(e => e.RequestDate));
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
            var permission = db.PermissionRequests.Where(x => x.LeaderApprovement == true).ToList();
            foreach (PermissionRequest per in permission)

            {

                PermissionRequest Existing_per = db.PermissionRequests.Find(per.Id);

                Existing_per.ManagerHasSeen = true;

            }
            db.SaveChanges();
            var current = System.Globalization.CultureInfo.CurrentCulture;
            current.DateTimeFormat.Calendar = new GregorianCalendar();
            current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            return View(db.PermissionRequests.OrderBy(e => e.RequestDate));
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
            Manager.RequestManager mng = new Manager.RequestManager();
            ViewBag.per = mng.GetPermission(id: User.Identity.GetUserId());
            ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "Id", "Type");
            return View();
            
        }
        
        // POST: /PermissionRequest/Create
        [HttpPost]
        public ActionResult Create(PermissionRequest per, string PerDateAr)
        {
            ModelState.Remove("PermissionDate");
            Manager.RequestManager mng = new Manager.RequestManager();
            ViewBag.per = mng.GetPermission(id: User.Identity.GetUserId());
            var CurrentUser = User.Identity.GetUserId();
            per.EmployeeNo = db.AspNetUsers.Where(a => a.Id == CurrentUser).FirstOrDefault().EmpNo;
            var IsArabic = Request.Cookies["culture"].Value == "ar" ? true : false;
            if (IsArabic)
            {


                CultureInfo MyCultureInfo = new CultureInfo("en-GB");
                DateTime.Parse(PerDateAr, MyCultureInfo);
                per.PermissionDate = DateTime.Parse(PerDateAr, MyCultureInfo);

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
            if (per.PermissionDate < DateTime.Today)
            {
                ModelState.AddModelError("PermissionDate", "Permission date not valid");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //const int maxTimesAsSeconds = 14400;
                    //int timeto = Convert.ToInt32(per.TimeTo);
                    //int timefrom = Convert.ToInt32(per.TimeFrom);
                    //var difference = Math.Abs(timeto - timefrom);

                    //var hf = 17;
                    //var mf = 20;
                    //per.TimeFrom = new TimeSpan(hours: , minutes: , 0);

                    //var ht = 20;
                    //var mt = 40;
                    //per.TimeTo = new TimeSpan(hours:, minutes:, 0);

                    //var differnceAs = per.TimeTo.Subtract(per.TimeFrom);
                    //var differnceAs_time = TimeSpan.FromMilliseconds(differnceAs_s);

                    //var TimeFromHours = per.TimeFromHours;
                    //var TimeFromMinutes = per.TimeFromMinutes;
                    //var TimeToHours = per.TimeToHours;
                    //var TimeToMinutes = per.TimeToMinutes;
                    ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "Id", "Type");
                    TimeSpan timeFrom = new TimeSpan(0, per.TimeFromHours, per.TimeFromMinutes ?? 0, 0);
                    TimeSpan timeTo = new TimeSpan(0, per.TimeToHours, per.TimeToMinutes ?? 0, 0);

                    //var differnce = timeTo - timeFrom;

                    //if (differnce.TotalHours > 4 )
                    //{
                    //    TempData["checkHours"] = "You Can Not Take More Than 4 Hours!";
                    //    ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "ID", "Type");
                    //}
                    //else
                    //{
                    per.RequestDate = DateTime.Now;
                    per.TimeToHours = timeFrom.Hours + 4;
                    per.LeaderApprovement = true;
                    TempData["chec"] = "Your Request Has Been Sented";
                    db.PermissionRequests.Add(per);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                    //}
                }
            }
            ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "Id", "Type");
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
            ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "Id", "Type");
            //ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "ID","Type",per.PermissionTypeNo);
            return View(per);
        }

        // POST: /PermissionRequest/Edit
        [HttpPost]
        public ActionResult Edit(int id, PermissionRequest per, string PerDateAr)
        {
            per.Id = id;
            string CurrentUser = User.Identity.GetUserId();
            per.EmployeeNo = db.AspNetUsers.Where(e => e.Id == CurrentUser).FirstOrDefault().EmpNo;
            var IsArabic = Request.Cookies["culture"].Value == "ar" ? true : false;
            if (IsArabic)
            {
                CultureInfo MyCultureInfo = new CultureInfo("en-GB");
                DateTime.Parse(PerDateAr, MyCultureInfo);
                per.PermissionDate = DateTime.Parse(PerDateAr, MyCultureInfo);
            }
            if (ModelState.IsValid)
            {
                ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "Id", "Type");
                TimeSpan timeFrom = new TimeSpan(0, per.TimeFromHours, per.TimeFromMinutes ?? 0, 0);
                TimeSpan timeTo = new TimeSpan(0, per.TimeToHours, per.TimeToMinutes ?? 0, 0);
                per.TimeToHours = timeFrom.Hours + 4;
                per.TimeToMinutes = timeFrom.Minutes;
                TempData["Edit"] = "Your Request Has Been Modified";
                db.Entry(per).State = System.Data.Entity.EntityState.Modified;
                ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "Id", "Type");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PermissionTypeNo = new SelectList(db.PermissionTypes.ToList(), "Id", "Type");
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