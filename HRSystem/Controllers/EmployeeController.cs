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

namespace HRSystem.Controllers
{
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
        


       

        public JsonResult GetPosition(int IdDepartment)
        {
            var IsArabic = Request.Cookies["culture"].Value == "ar" ? true : false;
            var DDlPosition = Db.Positions.Where(x => x.DepartmentNo == IdDepartment).ToList();
            List<SelectListItem> ListPositib = new List<SelectListItem>();

            ListPositib.Add(new SelectListItem { Text = Resources.NubiHR.Select, Value = "0" });
            if (DDlPosition != null)
            {
                foreach (var x in DDlPosition)
                {
                    ListPositib.Add(new SelectListItem { Text = (IsArabic == true ? x.PositionNameAr : x.PositionNameEn) , Value = x.Id.ToString() });
                }
            }
            return Json(new SelectList(ListPositib, "Value", "Text", JsonRequestBehavior.AllowGet));


        }



        //Show Employee Form
        [HttpGet]
        public ActionResult Index()
        {
            var current = System.Globalization.CultureInfo.CurrentCulture;
            current.DateTimeFormat.Calendar = new GregorianCalendar();
            ViewBag.Department = Db.Departments.ToList();

            return View(Db.Employees.ToList());

        }
        [HttpGet]
        public ActionResult Report()
        {
            ViewBag.Departments = Db.Departments.ToList();

            return View(Db.Employees.ToList());
        }


     
        [HttpGet]
        public ActionResult Create()
        {
            var IsArabic = Request.Cookies["culture"].Value == "ar" ? true : false;
            ViewBag.Department = Db.Departments.ToList();
            ViewBag.AttachmentType = Db.TypesOfAttachments.OrderBy(x => x.Id).ToList();
            ViewBag.RelationshipType = Db.RelationshipTypes/*.OrderBy(x => (IsArabic == true ? x.RelationAr : x.RelationEn))*/.ToList();

            return View();

        }
        [HttpPost]
        public ActionResult Create(VMAddEmployee model)
        {
            //model.NationalId = model.EmpAttachment.NationalId;
            if (ModelState.IsValid)
            {
                try
                {

                    //insert into salary table
                    double BasicSalaryString = Convert.ToDouble(model.BasicSalary);
                    salaryObj.BasicSalary = BasicSalaryString;
                    salaryObj.Date = DateTime.Now.Date;

                    Db.Salaries.Add(salaryObj);
                    Db.SaveChanges();

                    
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
                        Db.SaveChanges();
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
                    empObj.Email = model.EmailEmployee;

                    Db.Employees.Add(empObj);
                    Db.SaveChanges();

                    if (Request.Form["MySiblingsHidden"] != "")
                    {

                        //insert into attachment table
                        var counter = Convert.ToInt32(Request.Form["count"]);

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
                            });
                                Db.SaveChanges();

                            }
                        }
                    }



                    if (Request.Form["MyAttachmentHidden"] != "")
                    {
                        
                        //insert into attachment table
                        var counter = Convert.ToInt32(Request.Form["count"]);

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
                                    Db.SaveChanges();
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
                                    Db.SaveChanges();
                                }
                                

                            }
                        }
                    }

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
                    ViewBag.RelationshipType = Db.RelationshipTypes/*.OrderBy(x => (IsArabic == true ? x.RelationAr : x.RelationEn))*/.ToList();

                    return View();
                }

            }

            ModelState.Clear();
            ViewBag.Department = Db.Departments.ToList();
            ViewBag.AttachmentType = Db.TypesOfAttachments.OrderBy(x => x.Id).ToList();
            ViewBag.RelationshipType = Db.RelationshipTypes.OrderBy(x => x.Id).ToList();

            return View();


        }

        //GET: Edit/
        public ActionResult Edit(string id, string employee, FormCollection Fc)
        {

            ViewBag.id = id;
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            VMAddEmployee model = new VMAddEmployee();
            ViewBag.AttachmentType = Db.TypesOfAttachments.OrderBy(x => x.Id).ToList();
            ViewBag.RelationshipType = Db.RelationshipTypes/*.OrderBy(x => (IsArabic == true ? x.RelationAr : x.RelationEn))*/.ToList();
            ViewBag.Department = Db.Departments.ToList();
            var depno = Db.Employees.Where(e => e.Id == id).Select(s => s.DepartmentNo).FirstOrDefault();
            ViewBag.Position = Db.Positions.Where(p => p.DepartmentNo == depno);
           // ViewBag.TrainingCertificate = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).ToList();
           // var AttachmentNo = Db.Employees.Where(x => x.Id == id).Select(e => e.AttachmentNo).FirstOrDefault();
            ViewBag.Attachment = Db.Attachments.Where(x => x.EmpNo == id).ToList();
            ViewBag.att = Db.Attachments.Where(x => x.EmpNo == id).Count();
          //  ViewBag.TrainingCertificateList = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).ToList();
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

            if (ModelState.IsValid)
            {
                try
                {

                    
                    ViewBag.Department = Db.Departments.ToList();
                    ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
                    var BankNo = Db.Employees.Where(e => e.Id == id).Select(x => x.BankNo).FirstOrDefault();
                    model.SalaryNoEmployee = Db.Employees.Where(e => e.Id == id).Select(x => x.SalaryNo).FirstOrDefault();
                    // model.PhoneNo = Db.Employees.Where(e => e.Id == id).Select(x => x.PhoneNo).FirstOrDefault();
                    //model.AttachmentNo = Db.Employees.Where(e => e.Id == id).Select(x => (int)x.AttachmentNo).FirstOrDefault();
                    //ViewBag.TrainingCertificateList = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).ToList();
                    var tr = Db.Attachments.Where(e => e.EmpNo == id).ToList();
                    //model.TrainingCertificateList = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).ToList();
                    // var HasBankAccount = Db.Employees.Where(b => b.BankNo == BankNo).ToList();
                    ViewBag.EmployeeName = model.FirstName + " " + model.LastName;
                    var b = Db.BankAccounts.Where(x => x.Id == BankNo);
                    ViewBag.RelationshipType = Db.RelationshipTypes.ToList();

                    int NoOfRowsInBankTable = Db.BankAccounts.Count();

                    Salary salary = new Salary()
                    {
                        Id = (int)model.SalaryNoEmployee,
                        BasicSalary = model.BasicSalary,

                    };
                    if (BankNo != null && model.AccountNumber == null)
                    {
                        /*
                        BankAccount bank = new BankAccount()
                        {

                           // Id = (int)BankNo,
                           // BankBranch = b.BankBranch,
                           // BankName = model.BankName,
                           // AccountNumber = model.AccountNumber,
                           IsDeleted = true
                        };
                        Db.Entry(bank).State = EntityState.Modified;b*/
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
                    
                    List<EmergencyContact> EmergencyContactList = new List<EmergencyContact>();
                    EmergencyContact EC = new EmergencyContact()
                    {
                        Name = model.NameOfSibling,
                        Mobile = model.MobileEmergencyContact,
                        RelationshipTypeNo = Convert.ToInt32(model.IdRelationshipType),
                        Type = model.Other,
                        Date = model.DateEmergencyContact,
                        

                    };
                    EmergencyContactList.Add(EC);
                    foreach (var item in EmergencyContactList)
                    {
                        if (model.IdRelationshipType == 7)
                        {
                            model.IdEmergencyContact = Db.EmergencyContacts.Where(o => o.EmpNo == id && o.Name == model.NameOfSibling).Select(p => p.Id).FirstOrDefault();
                            EmergencyContact emegrency = new EmergencyContact()
                            {
                                Id = model.IdEmergencyContact,
                                EmpNo = model.IdEmployee,
                                Name = model.NameOfSibling,
                                Mobile = model.MobileEmergencyContact,
                                RelationshipTypeNo = Convert.ToInt32(model.IdRelationshipType),
                                Type = model.Other,
                                Date = model.DateEmergencyContact,
                            };
                            Db.Entry(emegrency).State = EntityState.Modified;
                        }
                        else
                        {
                            model.IdEmergencyContact = Db.EmergencyContacts.Where(o => o.EmpNo == id && o.Name == model.NameOfSibling).Select(p => p.Id).FirstOrDefault();
                            EmergencyContact emegrency = new EmergencyContact()
                            {
                                Id = model.IdEmergencyContact,
                                EmpNo = model.IdEmployee,
                                Name = model.NameOfSibling,
                                Mobile = model.MobileEmergencyContact,
                                RelationshipTypeNo = Convert.ToInt32(model.IdRelationshipType),
                                Type = null,
                            };
                            Db.Entry(emegrency).State = EntityState.Modified;
                        }

                    }

                    Db.Entry(salary).State = EntityState.Modified;
                    //Db.Entry(attach).State = EntityState.Modified;
                    
                    
                    
                    if (Request.Form["MySiblingsHidden"] != "")
                    {

                        //insert into attachment table
                        var counter = Convert.ToInt32(Request.Form["count"]);

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
                                    EmpNo = model.IdEmployee,
                                    Name = NameOfSibling,
                                    Mobile = MobileEmergencyContact,
                                    RelationshipTypeNo = ValueOfSibling,
                                    Type = Other,
                                });
                                Db.SaveChanges();

                            }
                        }
                    }



                    if (Request.Form["MyAttachmentHidden"] != "")
                    {

                        //insert into attachment table
                        var counter = Convert.ToInt32(Request.Form["count"]);

                        //Attachment tc = new Attachment();
                        if (counter != 1)
                        {
                            for (int i = 1; i < counter; i++)
                            {
                                var Expired = Request.Form["IsExpired" +" "+ i];
                                var Expirationdate = Request.Form["InputText" + i];
                                var x = Convert.ToDateTime(Expirationdate);
                                int ValueOfAttachment = Convert.ToInt32(Request.Form["ValueOfAttachment" + i]);
                                HttpPostedFileBase UploadedFile = Request.Files["UploadFile" + i];
                                //string UploadedFileExtension = Path.GetExtension(UploadedFile.FileName);
                                var path = Path.Combine(Server.MapPath("~/Attachments/"), UploadedFile.FileName);
                                bool bb = false;
                                if(Expired == "on")
                                {
                                    bb = true;
                                }
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
                                    Db.SaveChanges();
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
                                    Db.SaveChanges();
                                }


                            }
                        }
                    }
                    Db.SaveChanges();
                    // var counter = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).Count();

                    /*Attachment tc = new Attachment();
                    int i = 1;
                    foreach (var item in tr)
                    {

                        string filenameTrainingCertificate = Request.Form["TrainingCertificateName" + i];
                        if (!Request.Files["TrainingCertificateFile" + i].FileName.Equals(""))
                        {
                            HttpPostedFileBase AdditionalUploadedFileTrainingCertificate = Request.Files["TrainingCertificateFile" + i];
                            string AdditonalExtensionTrainingCertificate = Path.GetExtension(AdditionalUploadedFileTrainingCertificate.FileName);
                            filenameTrainingCertificate = Path.Combine(Server.MapPath("~/Attachments/"), filenameTrainingCertificate + AdditonalExtensionTrainingCertificate);
                            AdditionalUploadedFileTrainingCertificate.SaveAs(filenameTrainingCertificate);
                            filenameTrainingCertificate = Request.Form["TrainingCertificateName" + i] + AdditonalExtensionTrainingCertificate;
                            model.TrainingCertificateName = filenameTrainingCertificate;


                            tc = Db.TrainingCertificates.Find(item.Id);
                            tc.TrainingCertificateName = model.TrainingCertificateName;
                            tc.TrainingCertificateUrl = model.TrainingCertificateName;

                            Db.Entry(tc).State = EntityState.Modified;
                            Db.SaveChanges();

                            i++;

                        };
                    } */
                    //Db.SaveChanges();

                    ViewBag.Department = Db.Departments.ToList();
                    ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
                    ViewBag.EmployeeName = model.FirstName + " " + model.LastName;
                    model.EmergencyContactList = EmergencyContactList;
                    
                    ViewBag.Message = string.Format(Resources.NubiHR.EmployeeHasBeenModifiedSuccesfully, "Index");
                    ModelState.Clear();
                    return View(model);
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
                    ViewBag.MessageError = raise.Message;
                    ViewBag.Department = Db.Departments.ToList();
                    ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
                    //var AttachmentNo = Db.Employees.Where(x => x.Id == id).Select(e => e.AttachmentNo).FirstOrDefault();
                    ViewBag.Attachment = Db.Attachments.Where(x => x.EmpNo == id).ToList();
                    //ViewBag.att = Db.Attachments.Where(x => x.Id == AttachmentNo).Count();
                    //return RedirectToAction("Edit");
                    return View(model);
                }


            }
            ViewBag.Department = Db.Departments.ToList();
            ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
            //var AttachmentNo1 = Db.Employees.Where(x => x.Id == id).Select(e => e.AttachmentNo).FirstOrDefault();
            ViewBag.Attachment = Db.Attachments.Where(x => x.EmpNo == id).ToList();
            //ViewBag.att = Db.Attachments.Where(x => x.Id == AttachmentNo1).Count();
            //return RedirectToAction("Edit");
            return View(model);

        }

        //GET: /Details/
        [HttpGet]
        public ActionResult Details(string id)
        {
            VMAddEmployee model = new VMAddEmployee();
            var attachments = Db.Attachments.Where(x => x.EmpNo == id).ToList();

            model = Db.Employees.Where(x => x.Id == id)
             .Select(y => new VMAddEmployee
             {
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

            if (id == null)
            {
               // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }

            return View(model);
        }
       
        
        [HttpPost]
        public JsonResult IsAlreadySigned(string UserName)
        {

            return Json(IsUserAvailable(UserName));

        }

        public bool IsUserAvailable(string uname)
        {
            AspNetUser model = new AspNetUser();
            model = Db.AspNetUsers
              .Select(y => new AspNetUser
              {
                 
                  UserName = y.UserName//.Position.AspNetUsers.Select(u => u.UserName).FirstOrDefault(),
                  
              }).FirstOrDefault();



            var result = (from u in Db.AspNetUsers
                              where u.UserName == uname
                              select new { uname }).FirstOrDefault();

            bool status;
            if (result != null)
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

        /*
        [HttpPost]
        public ActionResult DownloadFile(int? id)
        {

          
            string filenamePassport = model.PassportTitle;
            string extensionPassport = Path.GetExtension(model.PassportNumber.FileName);
            filenamePassport = Path.Combine(Server.MapPath("~/Attachments/"), filenamePassport + extensionPassport);
            model.PassportNumber.SaveAs(filenamePassport);
            
            List<Attachment> AttachmentList = new List<Attachment>;


            //identify the virtual path
            string filePath = "~/Attachments/";

            //map the virtual path to a "local" path since GetFiles() can't use URI paths
            DirectoryInfo dir = new DirectoryInfo(Server.MapPath(filePath+ fkfdkhfdkg));

            //Get all files (but not any subdirectories) in the folder specified above
            FileInfo[] files = dir.GetFiles();

//iterate through each file, get its name and set its path, and add to my VM
            foreach (FileInfo file in files)
            {
                AttachmentList newFile = new AttachmentList();
                newFile.FileName = Path.GetFileNameWithoutExtension(file.FullName);     //remove the file extension for the name
                newFile.Path = filePath + file.Name;                        //set path to virtual directory + file name
                vm.FileList.Add(newFile);                                       //add each file to the right list in the Viewmodel
            }

            return (vm);
           
        }
        */
        

    }
}