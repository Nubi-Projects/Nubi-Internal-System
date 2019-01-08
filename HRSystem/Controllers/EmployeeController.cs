using HRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Web.WebPages.Scope;
using System.IdentityModel;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using System.Net;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Entity;
using HRSystem.Manager;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Windows.Input;
using static HRSystem.Controllers.ManageController;

namespace HRSystem.Controllers
{
    [Authorize(Roles ="Manager")]
    public class EmployeeController : BaseController
    {
        DatabaseManager DBMObj = new DatabaseManager();
       
        NubiDBEntities Db = new NubiDBEntities();
        
        //create objects from all tables
        AspNetUser UserObj = new AspNetUser();
        Employee empObj = new Employee();
        Position positionObj = new Position();
        Salary salaryObj = new Salary();
        Attachment AttachObj = new Attachment();
        BankAccount BankObj = new BankAccount();
        Department DeptObj = new Department();
        AspNetRole RoleObj = new AspNetRole();
        EmergencyContact EmgObj = new EmergencyContact();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public async Task<ActionResult> ChangePassword(AspNetUser usr, string NewPassword)
        {

            string token =  UserManager.GeneratePasswordResetToken(usr.Id);
            var result = await UserManager.ResetPasswordAsync(usr.Id, token, NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(usr.Id);
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                TempData["chec"] = string.Format(Resources.NubiHR.PasswordHasBeenResetSuccessfully, "Index");
                return RedirectToAction("Index", "Employee");
                }
            else
            {
                foreach (var error in result.Errors)
                {
                    TempData["check"] = error;
                    ModelState.AddModelError(string.Empty, TempData["check"].ToString());
                }
                return RedirectToAction("Index");
            }
            
        }
        public ActionResult DeleteEmergencyContact(int id)
        {
            var emegrency = Db.EmergencyContacts.Find(id);

            emegrency.IsDeleted = true;
            Db.Entry(emegrency).State = EntityState.Modified;
            Db.SaveChanges();
            TempData["chec"] = string.Format(Resources.NubiHR.EmergencyContactHasBeenDeleted, "Index");
            return RedirectToAction("Index", "Employee");
        }
        public ActionResult DeleteEmployee(string id)
        {
            var employee = Db.Employees.Find(id);
            var Asp = Db.AspNetUsers.Where(x => x.EmpNo == id).FirstOrDefault();
            
            if(Asp != null)
            {
                Asp.IsDeleted = true;
                Db.Entry(Asp).State = EntityState.Modified;
            }

            employee.IsDeleted = true;
            Db.Entry(employee).State = EntityState.Modified;
            

            Db.SaveChanges();

            TempData["chec"] = string.Format(Resources.NubiHR.EmployeeHasBeenDeleted, "Index");
            return RedirectToAction("Index", "Employee");
        }
        [HttpGet]
        public ActionResult NewDepartment()
        {
            ViewBag.Department = Db.Departments.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult NewDepartment(VMAddEmployee model)
        {
            var DeptEn = Db.Departments.Where(x => x.DepartmentNameEn == model.DepartmentNameEn).Any();
            var DeptAr = Db.Departments.Where(x => x.DepartmentNameAr == model.DepartmentNameAr).Any();
            var PosEn = Db.Positions.Where(x => x.PositionNameEn == model.PositionNameEn).Any();
            var PosAr = Db.Positions.Where(x => x.PositionNameAr == model.PositionNameAr).Any();

            var DeptEn1 = Db.Departments.Where(x => x.DepartmentNameEn == model.PositionNameEn).Any();
            var DeptAr1 = Db.Departments.Where(x => x.DepartmentNameAr == model.PositionNameAr).Any();
            var PosEn1 = Db.Positions.Where(x => x.PositionNameEn == model.DepartmentNameEn).Any();
            var PosAr1 = Db.Positions.Where(x => x.PositionNameAr == model.DepartmentNameAr).Any();

            if (!DeptEn && !DeptAr && !PosEn && !PosAr && !DeptEn1 && !DeptAr1 && !PosEn1 && !PosAr1)
            {
                if (model.DepartmentNameEn != null && model.DepartmentNameAr != null && model.PositionNameEn != null && model.PositionNameAr != null)
                {
                    DeptObj.DepartmentNameEn = model.DepartmentNameEn;
                    DeptObj.DepartmentNameAr = model.DepartmentNameAr;
                    Db.Departments.Add(DeptObj);

                    positionObj.DepartmentNo = DeptObj.Id;
                    positionObj.PositionNameEn = model.PositionNameEn;
                    positionObj.PositionNameAr = model.PositionNameAr;

                    Db.Positions.Add(positionObj);
                    Db.SaveChanges();

                    TempData["chec"] = string.Format(Resources.NubiHR.DepartmentAndPositionHaveBeenAddedSuccessfully, "Create");
                    return RedirectToAction("Create", "Employee");
                }
                else if (Request.Form["Department"] != "")
                {
                    positionObj.DepartmentNo = Convert.ToInt32(Request.Form["Department"]);
                    positionObj.PositionNameEn = model.PositionNameEn;
                    positionObj.PositionNameAr = model.PositionNameAr;

                    Db.Positions.Add(positionObj);
                    Db.SaveChanges();

                    TempData["chec"] = string.Format(Resources.NubiHR.PositionHasBeenAddedSuccessfully, "Create");
                    return RedirectToAction("Create", "Employee");
                }
                else
                {
                    TempData["check"] = string.Format(Resources.NubiHR.Required, "NewDepartment");
                    ViewBag.Department = Db.Departments.ToList();
                    return View();
                }
            }
            else
            {
                TempData["check"] = string.Format(Resources.NubiHR.AlreadyExist, "NewDepartment");
                ViewBag.Department = Db.Departments.ToList();
                return View();
            }

        }

        [HttpPost]
        public async Task<ActionResult> ForgetPassword(string HiddenID)
        {
            if (HiddenID == null)
            {
                return RedirectToAction("Index");
            }
            var NewPassword = Request.Form["Password"];
            var ConfirmPassword = Request.Form["ConfirmPassword"];
            if(NewPassword == "" || ConfirmPassword == "")
            {
                TempData["check"] = Resources.NubiHR.PleaseInsertTheNewPasswordTwice;
                ModelState.AddModelError(string.Empty, TempData["check"].ToString());
                return RedirectToAction("Index");
            }
            var id = Db.AspNetUsers.Where(x => x.EmpNo == HiddenID).Select(x => x.Id).FirstOrDefault();
            var usr = Db.AspNetUsers.Find(id);
            if(usr == null)
            {
                TempData["check"] = Resources.NubiHR.DidntCreateUserYet;
                ModelState.AddModelError(string.Empty, TempData["check"].ToString());
                return RedirectToAction("Index");
            }
            var password = usr.PasswordHash;
            if(Request.Form["Password"] != Request.Form["ConfirmPassword"])
            {
                TempData["check"] = Resources.NubiHR.PasswordAndConfirmationPasswordDoesNotMatch;
                ModelState.AddModelError(string.Empty, TempData["check"].ToString());
                return RedirectToAction("Index");
            }
            else
            {
                await ChangePassword(usr, NewPassword);
                return RedirectToAction("Index");
            }
        }
        public JsonResult GetPosition(int IdDepartment)
        {
            var IsArabic = Request.Cookies["culture"].Value == "ar" ? true : false;
            var DDlPosition = Db.Positions.Where(x => x.DepartmentNo == IdDepartment).ToList();
            List<SelectListItem> ListPosition = new List<SelectListItem>();

            ListPosition.Add(new SelectListItem { Text = Resources.NubiHR.Select, Value = "0" });
            if (DDlPosition != null)
            {
                foreach (var x in DDlPosition)
                {
                    ListPosition.Add(new SelectListItem { Text = (IsArabic == true ? x.PositionNameAr : x.PositionNameEn) , Value = x.Id.ToString() });
                }
            }
            return Json(new SelectList(ListPosition, "Value", "Text", JsonRequestBehavior.AllowGet));


        }



        //Show Employee Form
        [HttpGet]
        public ActionResult Index()
        {
            //var current = System.Globalization.CultureInfo.CurrentCulture;
            //current.DateTimeFormat.Calendar = new GregorianCalendar();
            //current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            ViewBag.Department = Db.Departments.ToList();

            return View(Db.Employees.ToList());

        }
        [HttpGet]
        public ActionResult Report()
        {
            var current = System.Globalization.CultureInfo.CurrentCulture;
            current.DateTimeFormat.Calendar = new GregorianCalendar();
            current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            ViewBag.Departments = Db.Departments.ToList();

            return View(Db.Employees.ToList());
        }


     
        [HttpGet]
        public ActionResult Create()
        {
            //var current = System.Globalization.CultureInfo.CurrentCulture;
            //current.DateTimeFormat.Calendar = new GregorianCalendar();
            //current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            var IsArabic = Request.Cookies["culture"].Value == "ar" ? true : false;
            ViewBag.Department = Db.Departments.ToList();
            ViewBag.AttachmentType = Db.TypesOfAttachments.OrderBy(x => x.Id).ToList();
            ViewBag.RelationshipType = Db.RelationshipTypes/*.OrderBy(x => (IsArabic == true ? x.RelationAr : x.RelationEn))*/.ToList();

            return View();

        }
        [HttpPost]
        public ActionResult Create(VMAddEmployee model)
        {
           // var current = System.Globalization.CultureInfo.CurrentCulture;
          //  current.DateTimeFormat.Calendar = new GregorianCalendar();
           // current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            if (ModelState.IsValid && Request.Form["MySiblingsHidden"] != "")
            {
                try
                {
                    //insert into salary table
                    double BasicSalaryString = Convert.ToDouble(model.BasicSalary);
                    salaryObj.BasicSalary = BasicSalaryString;
                    salaryObj.Date = DateTime.Now.Date;

                    Db.Salaries.Add(salaryObj);
                   // Db.SaveChanges();

                    
                    //if condtion to check if user has account
                    if (model.BankName != null)
                    {
                        //make fileds required in runtime
                        //insert into bank account
                        BankObj.AccountNumber = model.AccountNumber;
                        BankObj.BankName = model.BankName;
                        BankObj.BankBranch = model.BankBranch;
                        BankObj.Date = DateTime.Now.Date;

                        Db.BankAccounts.Add(BankObj);
                        //Db.SaveChanges();
                    }
                    
                    //insert into employee table

                    Guid GuIdObjEmp = Guid.NewGuid();
                    empObj.Id = GuIdObjEmp.ToString();
                    empObj.FirstName = model.FirstName;
                    empObj.LastName = model.LastName;
                    empObj.Mobile1 = model.Mobile1;
                    empObj.Address = model.Address;
                    empObj.DepartmentNo = model.IdDepartment;
                    empObj.PositionNo = model.IdPosition;
                    empObj.FunctionalNumber = Convert.ToInt32(model.FunctionalNumber);
                    
                    if (model.BankName != null)
                    {
                        empObj.BankNo = BankObj.Id;
                    }
                    empObj.SalaryNo = salaryObj.Id;
                    empObj.StartDate = model.StartDate.Date;
                    empObj.Date = DateTime.Now.Date;
                    empObj.IsDeleted = false;
                    empObj.HasAccount = false;

                    if(model.EmailEmployee != null)
                    {
                        empObj.Email = model.EmailEmployee;
                    }
                    
                    Db.Employees.Add(empObj);
                    //Db.SaveChanges();

                    if (Request.Form["MySiblingsHidden"] != "")
                    {
                        //insert into attachment table
                        var counter = Convert.ToInt32(Request.Form["countSib"]);

                        //Attachment tc = new Attachment();
                        if (counter != 1)
                        {
                            for (int i = 1; i < counter; i++)
                            {
                                string NameOfSibling = Request.Form["NameOfSibling" + i];
                                int ValueOfSibling = Convert.ToInt32(Request.Form["ValueOfSibling" + i]);
                                string MobileEmergencyContact = Request.Form["MobileEmergencyContact" + i];
                                string Other = "";

                                if (ValueOfSibling == 7)
                                {
                                    Other = Request.Form["Other" + i];
                                }
                               else
                                {
                                    Other = null;
                                }
                                
                                Db.EmergencyContacts.Add(new EmergencyContact
                                {
                                    EmpNo = empObj.Id,
                                    Name = NameOfSibling,
                                    Mobile = MobileEmergencyContact,
                                    RelationshipTypeNo = ValueOfSibling,
                                    Type = Other,
                                    Date = DateTime.Now.Date,
                                    IsDeleted = false,
                                });
                               // Db.SaveChanges();

                            }
                        }
                    }
                    
                    if (Request.Form["MyAttachmentHidden"] != "")
                    {
                        
                        //insert into attachment table
                        var counter = Convert.ToInt32(Request.Form["countAtt"]);

                        //Attachment tc = new Attachment();
                        if (counter != 1)
                        {
                            for (int i = 1; i < counter; i++)
                            {
                                var Expirationdate = Request.Form["InputText" + i];
                                var x = Convert.ToDateTime(Expirationdate);
                                int ValueOfAttachment = Convert.ToInt32(Request.Form["ValueOfAttachment" + i]) ;
                                HttpPostedFileBase UploadedFile = Request.Files["UploadFile" + i];
                               //string UploadedFileExtension = Path.GetExtension(UploadedFile.FileName);
                                var path = Path.Combine(Server.MapPath("~/Attachments/"), UploadedFile.FileName);

                                UploadedFile.SaveAs(path);
                                // AdditionalFilenameTrainingCertificate = Request.Form["InputText" + i] + AdditonalExtensionTrainingCertificate;

                                if (Expirationdate == null)
                                {
                                    Db.Attachments.Add(new Attachment
                                    {
                                        EmpNo = empObj.Id,
                                        Name = UploadedFile.FileName,
                                        Path = path,
                                        TypeOfAttachmentNo = ValueOfAttachment,
                                        ExpirationDate = null,
                                        IsExpired = null,
                                        IsDeleted = false,
                                        Date = DateTime.Now.Date

                                    });
                                   // Db.SaveChanges();
                                }

                                else
                                {
                                    Db.Attachments.Add(new Attachment
                                    {
                                        EmpNo = empObj.Id,
                                        Name = UploadedFile.FileName,
                                        Path = path,
                                        TypeOfAttachmentNo = ValueOfAttachment,
                                        ExpirationDate = x,
                                        IsExpired = null,
                                        IsDeleted = false,
                                        Date = DateTime.Now.Date

                                    });
                                   // Db.SaveChanges();
                                }
                                

                            }
                        }
                    }

                    Db.SaveChanges();
                    ViewBag.Message = string.Format(Resources.NubiHR.EmployeeHasBeenAddedSuccesfully, "Index", "Employee");

                }
                catch (DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    ViewBag.MessageError = raise.Message;
                    // throw raise;
                    ModelState.Clear();
                    ViewBag.Department = Db.Departments.ToList();
                    ViewBag.AttachmentType = Db.TypesOfAttachments.OrderBy(x => x.Id).ToList();
                    ViewBag.RelationshipType = Db.RelationshipTypes.ToList();

                    return View();
                }

                ModelState.Clear();
                ViewBag.Department = Db.Departments.ToList();
                ViewBag.AttachmentType = Db.TypesOfAttachments.OrderBy(x => x.Id).ToList();
                ViewBag.RelationshipType = Db.RelationshipTypes.OrderBy(x => x.Id).ToList();

                return View();

            }
            else
            {
                
                if(Request.Form["MySiblingsHidden"] == "")
                {
                    TempData["check"] = Resources.NubiHR.MustAddOneSiblingAtLeast;
                    ModelState.AddModelError(string.Empty, TempData["check"].ToString());
                }
                else
                {
                    TempData["check"] = Resources.NubiHR.Error;
                }
                ModelState.Clear();
                ViewBag.Department = Db.Departments.ToList();
                ViewBag.AttachmentType = Db.TypesOfAttachments.OrderBy(x => x.Id).ToList();
                ViewBag.RelationshipType = Db.RelationshipTypes.OrderBy(x => x.Id).ToList();

                return View();
            }
            
        }

        //GET: Edit/
        public ActionResult Edit(string id, string employee, FormCollection Fc)
        {

            ViewBag.id = id;
            if (id == null)
            {
                return RedirectToAction("Index");
            }
           // var current = System.Globalization.CultureInfo.CurrentCulture;
           // current.DateTimeFormat.Calendar = new GregorianCalendar();
           // current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            VMAddEmployee model = new VMAddEmployee();
            ViewBag.AttachmentType = Db.TypesOfAttachments.OrderBy(x => x.Id).ToList();
            ViewBag.RelationshipType = Db.RelationshipTypes/*.OrderBy(x => (IsArabic == true ? x.RelationAr : x.RelationEn))*/.ToList();
            ViewBag.Department = Db.Departments.ToList();
            var depno = Db.Employees.Where(e => e.Id == id).Select(s => s.DepartmentNo).FirstOrDefault();
            ViewBag.Position = Db.Positions.Where(p => p.DepartmentNo == depno);
            ViewBag.Attachment = Db.Attachments.Where(x => x.EmpNo == id).ToList();
            ViewBag.att = Db.Attachments.Where(x => x.EmpNo == id).Count();
            string empname = Request.QueryString["employee"];
            ViewBag.EmployeeName = empname;
            Fc[model.FirstName] = empObj.FirstName;

           
                model = Db.Employees.Where(x => x.Id == id)
               .Select(y => new VMAddEmployee
               {
                   FirstName = y.FirstName,
                   LastName = y.LastName,
                   EmailEmployee = y.Email,
                   Address = y.Address,
                   FunctionalNumber = y.FunctionalNumber,
                   Mobile1 = y.Mobile1,
                   EmergencyContactList = y.EmergencyContacts.ToList(),
                   AttachmentList = y.Attachments.ToList(),
                   StartDate = y.StartDate,
                   IdDepartment = y.Department.Id,
                   IdPosition = y.Position.Id,
                   BasicSalary = y.Salary.BasicSalary,
                   AccountNumber = y.BankAccount.AccountNumber,
                   BankName = y.BankAccount.BankName,
                   BankBranch = y.BankAccount.BankBranch,

               }).FirstOrDefault();


           
            return View(model);
        }


        //POST: /Edit/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //  more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, VMAddEmployee model)
        {
            model.IdEmployee = id;
           // var current = System.Globalization.CultureInfo.CurrentCulture;
          //  current.DateTimeFormat.Calendar = new GregorianCalendar();
          //  current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            if (ModelState.IsValid)
            {
                try
                {

                    
                    ViewBag.Department = Db.Departments.ToList();
                    ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
                    var BankNo = Db.Employees.Where(e => e.Id == id).Select(x => x.BankNo).FirstOrDefault();
                    model.SalaryNoEmployee = Db.Employees.Where(e => e.Id == id).Select(x => x.SalaryNo).FirstOrDefault();
                    var tr = Db.Attachments.Where(e => e.EmpNo == id).ToList();
                    ViewBag.EmployeeName = model.FirstName + " " + model.LastName;
                    var b = Db.BankAccounts.Where(x => x.Id == BankNo);
                    ViewBag.RelationshipType = Db.RelationshipTypes.ToList();
                    model.AttachmentList = Db.Attachments.Where(x => x.EmpNo == id).ToList();
                    model.EmergencyContactList = Db.EmergencyContacts.Where(x => x.EmpNo == id).ToList();

                    int NoOfRowsInBankTable = Db.BankAccounts.Count();

                    Salary salary = new Salary()
                    {
                        Id = (int)model.SalaryNoEmployee,
                        BasicSalary = model.BasicSalary,

                    };
                    if (BankNo != null && model.AccountNumber == null)
                    {
                        ViewBag.MessageException = string.Format("YOU CANNOT DELETE BANK ACCOUNT!", "Index");
                        return View(model);
                    }
                    else if (model.AccountNumber != null)
                    {
                        BankAccount bank = new BankAccount()
                        {
                            //Id = (int)/*model.*/BankNo, NoOfRowsInBankTable++,
                            Id = NoOfRowsInBankTable++,
                            BankBranch = model.BankBranch,
                            BankName = model.BankName,
                            AccountNumber = model.AccountNumber,
                        };
                        Db.Entry(bank).State = EntityState.Modified;

                        Employee employee = new Employee()
                        {
                            Id = model.IdEmployee,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.EmailEmployee,
                            Mobile1 = model.Mobile1,
                            Address = model.Address,
                            DepartmentNo = model.IdDepartment,
                            PositionNo = model.IdPosition,
                            FunctionalNumber = model.FunctionalNumber,
                            
                            BankNo = bank.Id,
                            SalaryNo = salary.Id,
                            StartDate = model.StartDate,

                        };
                        Db.Entry(employee).State = EntityState.Modified;
                    }
                    else
                    {
                        Employee employee = new Employee()
                        {
                            Id = model.IdEmployee,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.EmailEmployee,
                            Mobile1 = model.Mobile1,
                            Address = model.Address,
                            DepartmentNo = model.IdDepartment,
                            PositionNo = model.IdPosition,
                            FunctionalNumber = model.FunctionalNumber,
                            
                            // BankNo = bank.Id,
                            SalaryNo = salary.Id,
                            StartDate = model.StartDate,

                        };
                        Db.Entry(employee).State = EntityState.Modified;
                    }
                   
                    
                    int ii = 1;
                    foreach (var item in model.EmergencyContactList)
                    {
                        
                        var emegrency = Db.EmergencyContacts.Find(item.Id);

                        string MobileEmergencyContact = Request.Form[ii + " " +"MobileEmergencyContact"];
                        
                        emegrency.Mobile = MobileEmergencyContact;
                        Db.Entry(emegrency).State = EntityState.Modified;
                        ii++;
                    }

                    

                    Db.Entry(salary).State = EntityState.Modified;
                   

                    var countAttTable = Convert.ToInt32(Request.Form["countAttTable"]);
                    var no = countAttTable;
                    foreach (var item in model.AttachmentList)
                    {
                        
                        var Expired = Request.Form["IsExpired" + " " + no];

                        var attach = Db.Attachments.Find(item.Id);

                        if (Expired == "on" && item.IsExpired != true)
                        {
                            attach.IsExpired = true;
                            Db.Entry(attach).State = EntityState.Modified;
                            
                        }
                        no++;
                    }
                    

                    if (Request.Form["MySiblingsHidden"] != "")
                    {
                        var counter = Convert.ToInt32(Request.Form["countSib"]);
                        
                        if (counter != 1)
                        {
                            for (int i = 1; i < counter; i++)
                            {
                                string NameOfSibling = Request.Form["NameOfSibling" + i];
                                int ValueOfSibling = Convert.ToInt32(Request.Form["ValueOfSibling" + i]);
                                string MobileEmergencyContact = Request.Form["MobileEmergencyContact" + i];
                                string Other = "";

                                if (ValueOfSibling == 7)
                                {
                                    Other = Request.Form["Other" + i];
                                }
                                else
                                {
                                    Other = null;
                                }

                                Db.EmergencyContacts.Add(new EmergencyContact
                                {
                                    EmpNo = model.IdEmployee,
                                    Name = NameOfSibling,
                                    Mobile = MobileEmergencyContact,
                                    RelationshipTypeNo = ValueOfSibling,
                                    Type = Other,
                                    Date = model.DateEmergencyContact,
                                    IsDeleted = false,
                                });
                                //Db.SaveChanges();

                            }
                        }
                    }

                    
                    if (Request.Form["MyAttachmentHidden"] != "")
                    {
                        
                        var counter = Convert.ToInt32(Request.Form["countAtt"]);
                        
                        if (counter != 1)
                        {
                            for (int i = 1; i < counter; i++)
                            {
                                
                                var Expirationdate = Request.Form["InputText" + i];
                                var x = Convert.ToDateTime(Expirationdate);
                                int ValueOfAttachment = Convert.ToInt32(Request.Form["ValueOfAttachment" + i]);
                                HttpPostedFileBase UploadedFile = Request.Files["UploadFile" + i];
                                //string UploadedFileExtension = Path.GetExtension(UploadedFile.FileName);
                                var path = Path.Combine(Server.MapPath("~/Attachments/"), UploadedFile.FileName);
                                
                                UploadedFile.SaveAs(path);
                                // AdditionalFilenameTrainingCertificate = Request.Form["InputText" + i] + AdditonalExtensionTrainingCertificate;

                                if (Expirationdate == null)
                                {
                                    Db.Attachments.Add(new Attachment
                                    {
                                        EmpNo = model.IdEmployee,
                                        Name = UploadedFile.FileName,
                                        Path = path,
                                        TypeOfAttachmentNo = ValueOfAttachment,
                                        ExpirationDate = null,
                                        IsExpired = null,
                                        IsDeleted = false,
                                        Date = DateTime.Now.Date

                                    });
                                   // Db.SaveChanges();
                                }

                                else
                                {
                                    Db.Attachments.Add(new Attachment
                                    {
                                        EmpNo = model.IdEmployee,
                                        Name = UploadedFile.FileName,
                                        Path = path,
                                        TypeOfAttachmentNo = ValueOfAttachment,
                                        ExpirationDate = x,
                                        IsExpired = null,
                                        IsDeleted = false,
                                        Date = DateTime.Now.Date

                                    });
                                   // Db.SaveChanges();
                                }


                            }
                        }
                    }
                    Db.SaveChanges();
                   
                    ViewBag.Department = Db.Departments.ToList();
                    ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
                    ViewBag.EmployeeName = model.FirstName + " " + model.LastName;
                    //model.EmergencyContactList = Db.EmergencyContacts.Where(x => x.EmpNo == id).ToList();

                    TempData["chec"] = string.Format(Resources.NubiHR.EmployeeHasBeenModifiedSuccesfully, "Index");
                    ModelState.Clear();
                    //return View(model);
                    return RedirectToAction("Index","Employee");
                }

                catch (DbEntityValidationException ee)
                {
                    Exception raise = ee;
                    foreach (var eve in ee.EntityValidationErrors)
                    {

                        string message = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            message += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }

                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);

                    }
                   // ViewBag.MessageException = raise.Message;
                    TempData["check"] = raise.Message;
                    ViewBag.Department = Db.Departments.ToList();
                    ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
                    //var AttachmentNo = Db.Employees.Where(x => x.Id == id).Select(e => e.AttachmentNo).FirstOrDefault();
                    ViewBag.Attachment = Db.Attachments.Where(x => x.EmpNo == id).ToList();
                    //ViewBag.att = Db.Attachments.Where(x => x.Id == AttachmentNo).Count();
                    //return RedirectToAction("Edit");
                    // return View(model);
                    return RedirectToAction("Index", "Employee");
                }


            }
            ViewBag.Department = Db.Departments.ToList();
            ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
            //var AttachmentNo1 = Db.Employees.Where(x => x.Id == id).Select(e => e.AttachmentNo).FirstOrDefault();
            ViewBag.Attachment = Db.Attachments.Where(x => x.EmpNo == id).ToList();
            model.EmergencyContactList = Db.EmergencyContacts.Where(x => x.EmpNo == id).ToList();
            ViewBag.AttachmentType = Db.TypesOfAttachments.OrderBy(x => x.Id).ToList();
            ViewBag.RelationshipType = Db.RelationshipTypes/*.OrderBy(x => (IsArabic == true ? x.RelationAr : x.RelationEn))*/.ToList();
            TempData["check"] = Resources.NubiHR.Error;
            return View(model);
            //return RedirectToAction("Index", "Employee");

        }

        //GET: /Details/
        [HttpGet]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }

            var IsArabic = Request.Cookies["culture"].Value == "ar" ? true : false;
            VMAddEmployee model = new VMAddEmployee();
            var attachments = Db.Attachments.Where(x => x.EmpNo == id).ToList();
           // var current = System.Globalization.CultureInfo.CurrentCulture;
            //current.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
           // current.DateTimeFormat.Calendar = new GregorianCalendar();

            model = Db.Employees.Where(x => x.Id == id)
             .Select(y => new VMAddEmployee
             {
                 IdEmployee = id,
                 FirstName = y.FirstName,
                 LastName = y.LastName,
                 EmailEmployee = y.Email,
                 Address = y.Address,
                 FunctionalNumber = y.FunctionalNumber,
                 Mobile1 = y.Mobile1,
                 AttachmentList = y.Attachments.ToList(),
                 StartDate = y.StartDate,
                 IdDepartment = y.Department.Id,
                 IdPosition = y.Position.Id,
                 BasicSalary = y.Salary.BasicSalary,
                 AccountNumber = y.BankAccount.AccountNumber,
                 BankName = y.BankAccount.BankName,
                 BankBranch = y.BankAccount.BankBranch,
                 EmergencyContactList = y.EmergencyContacts.ToList()

             }).FirstOrDefault();

            ViewBag.dept = Db.Departments.Where(x => x.Id == model.IdDepartment).Select( x=> (IsArabic == true ? x.DepartmentNameAr : x.DepartmentNameEn)).FirstOrDefault();
            ViewBag.pos = Db.Positions.Where(x => x.Id == model.IdPosition).Select(x => (IsArabic == true ? x.PositionNameAr : x.PositionNameEn)).FirstOrDefault();
            ViewBag.AspEmail = Db.AspNetUsers.Where(x => x.EmpNo == model.IdEmployee).Select(x => x.Email).FirstOrDefault();

            return View(model);
        }
       
        
        [HttpPost]
        public JsonResult IsAlreadySigned(RegisterViewModel model)
        {
            string UserName = model.Email;
            return Json(IsUserAvailable(UserName));

        }

        public bool IsUserAvailable(string Email)
        {
            var result = Db.AspNetUsers.Where(x => x.Email == Email || x.UserName == Email).Any();
            
            bool status;
            if (result)
            {
                //Already registered  
                status = false;
            }
            else
            {
                //Available to use  
                status = true;
            }

            return status;
        }
        

    }
}