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
        TrainingCertificate TCertificateObj = new TrainingCertificate();
        Department DeptObj = new Department();
        AspNetRole RoleObj = new AspNetRole();


       

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
            
            ViewBag.Department = Db.Departments.ToList();

            return View(Db.Employees.ToList());

        }
        [HttpGet]
        public ActionResult Report()
        {
            ViewBag.Position = Db.Positions.ToList();

            return View(Db.Employees.ToList());
        }

      
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Department = Db.Departments.ToList();
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
                    if(model.BankName !=  null)
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



                    //insert into attachment table


                    string filenameNId = model.NIdTitle;
                    string extensionNId = Path.GetExtension(model.NationalId.FileName);                  
                    filenameNId = Path.Combine(Server.MapPath("~/Attachments/"), filenameNId + extensionNId);
                    model.NationalId.SaveAs(filenameNId);
                    filenameNId = model.NIdTitle + extensionNId;

                    if(model.PassportNumber != null)
                    {
                        string filenamePassport = model.PassportTitle;
                        string extensionPassport = Path.GetExtension(model.PassportNumber.FileName);
                        filenamePassport = Path.Combine(Server.MapPath("~/Attachments/"), filenamePassport + extensionPassport);
                        model.PassportNumber.SaveAs(filenamePassport);
                        filenamePassport = model.PassportTitle + extensionPassport;
                        AttachObj.PassportNumber = filenamePassport;
                    }

                    if(model.LastCertificate != null)
                    {
                        string filenameLastCertificate = model.LastCertTitle;
                        string extensionLastCertificate = Path.GetExtension(model.LastCertificate.FileName);
                        filenameLastCertificate = Path.Combine(Server.MapPath("~/Attachments/"), filenameLastCertificate + extensionLastCertificate);
                        model.LastCertificate.SaveAs(filenameLastCertificate);
                        filenameLastCertificate = model.LastCertTitle + extensionLastCertificate;
                        AttachObj.LastCertificate = filenameLastCertificate;

                    }


                    string filenameImage = model.ImageTitle;
                    string extensionImage = Path.GetExtension(model.ImageUrl.FileName);
                    filenameImage = Path.Combine(Server.MapPath("~/Attachments/"), filenameImage + extensionImage);
                    model.ImageUrl.SaveAs(filenameImage);
                    filenameImage = model.ImageTitle + extensionImage;


                    AttachObj.NationalId = filenameNId;
                    
                    AttachObj.ImageUrl = filenameImage;
                    AttachObj.Date = DateTime.Now.Date;

                    Db.Attachments.Add(AttachObj);
                    Db.SaveChanges();


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
                    empObj.AttachmentNo = AttachObj.Id;
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

                    /*
                    //insert into users
                    UserObj.Email = model.EmailAspNetUser;
                    UserObj.UserName = null; // "null";
                     serObj.PasswordHash = Encrypt(model.PasswordHash);
                    Guid GuIdObj = Guid.NewGuid();
                    UserObj.Id = GuIdObj.ToString();
                    UserObj.IsDeleted = false;
                    UserObj.EmpNo = empObj.Id;

                    Db.AspNetUsers.Add(UserObj);
                    Db.SaveChanges();
                    */

                    if (model.TrainingCertificateUrl != null)
                    {


                        //insert into training certificate table

                        string filenameTrainingCertificate = model.TrainingCertificateName;
                        string extensionTrainingCertificate = Path.GetExtension(model.TrainingCertificateUrl.FileName);
                        filenameTrainingCertificate = Path.Combine(Server.MapPath("~/Attachments/"), filenameTrainingCertificate + extensionTrainingCertificate);
                        model.TrainingCertificateUrl.SaveAs(filenameTrainingCertificate);
                        filenameTrainingCertificate = model.TrainingCertificateName + extensionTrainingCertificate;


                        TCertificateObj.EmployeeNo = empObj.Id;
                        TCertificateObj.TrainingCertificateName = model.TrainingCertificateName;
                        TCertificateObj.TrainingCertificateUrl = filenameTrainingCertificate;
                        TCertificateObj.Date = DateTime.Now.Date;

                        Db.TrainingCertificates.Add(TCertificateObj);
                        Db.SaveChanges();

                        //upload multiple training certificate
                        var counter = Convert.ToInt32(Request.Form["count"]);

                        TrainingCertificate tc = new TrainingCertificate();
                        if (counter != 1)
                        {
                            for (int i = 1; i < counter; i++)
                            {
                                string AdditionalFilenameTrainingCertificate = Request.Form["InputText" + i];
                                HttpPostedFileBase AdditionalUploadedFileTrainingCertificate = Request.Files["UploadFile" + i];
                                string AdditonalExtensionTrainingCertificate = Path.GetExtension(AdditionalUploadedFileTrainingCertificate.FileName);
                                AdditionalFilenameTrainingCertificate = Path.Combine(Server.MapPath("~/Attachments/"), AdditionalFilenameTrainingCertificate + AdditonalExtensionTrainingCertificate);

                                AdditionalUploadedFileTrainingCertificate.SaveAs(AdditionalFilenameTrainingCertificate);
                                AdditionalFilenameTrainingCertificate = Request.Form["InputText" + i] + AdditonalExtensionTrainingCertificate;



                                Db.TrainingCertificates.Add(new TrainingCertificate
                                {
                                    EmployeeNo = empObj.Id,
                                    TrainingCertificateName = Request.Form["InputText" + i],
                                    TrainingCertificateUrl = AdditionalFilenameTrainingCertificate,

                                });
                                Db.SaveChanges();

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
                    throw raise;
                }
                ModelState.Clear();
                ViewBag.Department = Db.Departments.ToList();
                return View();
                

            }
           // ViewBag.MessageError = string.Format("Something Went Wrong");
            ViewBag.Department = Db.Departments.ToList();
            return View();


        }

        // GET: Edit/
        public ActionResult Edit(string id, string employee, FormCollection Fc)
        {

            ViewBag.id = id;
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            VMAddEmployee model = new VMAddEmployee();
           
            ViewBag.Department = Db.Departments.ToList();
            var depno = Db.Employees.Where(e => e.Id == id).Select(s => s.DepartmentNo).FirstOrDefault();
            ViewBag.Position = Db.Positions.Where(p => p.DepartmentNo == depno);
            ViewBag.TrainingCertificate = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).ToList();
            ViewBag.AttachmentNo = Db.Employees.Select(e => e.AttachmentNo).FirstOrDefault();
            ViewBag.TrainingCertificateList = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).ToList();
            
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
                   EmpAttachment=y.Attachment,
                   TrainingCertificateList = y.TrainingCertificates.ToList(),
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


        // POST: /Edit/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, VMAddEmployee model)
        {
            model.IdEmployee = id;
            
            if (ModelState.IsValid || model.NationalId == null || model.ImageUrl == null)
            {
                ViewBag.Department = Db.Departments.ToList();
                ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
                var BankNo = Db.Employees.Where(e => e.Id == id).Select(x => x.BankNo).FirstOrDefault();
                model.SalaryNoEmployee = Db.Employees.Where(e => e.Id == id).Select(x => x.SalaryNo).FirstOrDefault();
               // model.PhoneNo = Db.Employees.Where(e => e.Id == id).Select(x => x.PhoneNo).FirstOrDefault();
                model.AttachmentNo = Db.Employees.Where(e => e.Id == id).Select(x => x.AttachmentNo).FirstOrDefault();
                ViewBag.TrainingCertificateList = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).ToList();
                var tr = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).ToList();
                model.TrainingCertificateList = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).ToList();
                var HasBankAccount = Db.Employees.Where(b => b.Id == id).Select(n => n.BankNo).Any(); 
               
                Salary salary = new Salary()
                {
                    Id = (int)model.SalaryNoEmployee,
                    BasicSalary = model.BasicSalary,

                };
                

                string filenameNId = Request.Form["NIdTitle"];
               
                if (!Request.Files["NIDFile"].FileName.Equals(""))
                {
                    HttpPostedFileBase NationalIdFile = Request.Files["NIDFile"];
                    string extensionNId = Path.GetExtension(NationalIdFile.FileName);
                    filenameNId = Path.Combine(Server.MapPath("~/Attachments/"), filenameNId + extensionNId);
                    NationalIdFile.SaveAs(filenameNId);
                    filenameNId = Request.Form["NIdTitle"] + extensionNId;
                    model.EmpAttachment.NationalId = Request.Form["NIdTitle"] + extensionNId;
                }

                string filenamePassport = Request.Form["PassportTitle"];

                if (!Request.Files["PassportFile"].FileName.Equals(""))
                {
                    HttpPostedFileBase PassportNumberFile = Request.Files["PassportFile"];
                    string extensionPassport = Path.GetExtension(PassportNumberFile.FileName);
                    filenamePassport = Path.Combine(Server.MapPath("~/Attachments/"), filenamePassport + extensionPassport);
                    PassportNumberFile.SaveAs(filenamePassport);
                    filenamePassport = Request.Form["PassportTitle"] + extensionPassport;
                    model.EmpAttachment.PassportNumber = Request.Form["PassportTitle"] + extensionPassport;
                }
                string filenameLastCertificate = Request.Form["LastCertificateTitle"];

                if (!Request.Files["LastCertificateFile"].FileName.Equals(""))
                {
                    HttpPostedFileBase LastCertificateFile = Request.Files["LastCertificateFile"];
                    string extensionLastCertificate = Path.GetExtension(LastCertificateFile.FileName);
                    filenameLastCertificate = Path.Combine(Server.MapPath("~/Attachments/"), filenameLastCertificate + extensionLastCertificate);
                    LastCertificateFile.SaveAs(filenameLastCertificate);
                    filenameLastCertificate = Request.Form["LastCertificateTitle"] + extensionLastCertificate;
                    model.EmpAttachment.LastCertificate = Request.Form["LastCertificateTitle"] + extensionLastCertificate;
                }
                string filenameImageUrl = Request.Form["ImageTitle"];

                if (!Request.Files["ImageFile"].FileName.Equals(""))
                {
                    HttpPostedFileBase ImageFile = Request.Files["ImageFile"];
                    string extensionImageUrl = Path.GetExtension(ImageFile.FileName);
                    filenameImageUrl = Path.Combine(Server.MapPath("~/Attachments/"), filenameImageUrl + extensionImageUrl);
                    ImageFile.SaveAs(filenameImageUrl);
                    filenameImageUrl = Request.Form["ImageTitle"] + extensionImageUrl;
                    model.EmpAttachment.ImageUrl = Request.Form["ImageTitle"] + extensionImageUrl;
                }
                Attachment attach = new Attachment()
                {
                    Id = (int)model.AttachmentNo,
                    NationalId = model.EmpAttachment.NationalId,
                    PassportNumber = model.EmpAttachment.PassportNumber,
                    LastCertificate = model.EmpAttachment.LastCertificate,
                    ImageUrl = model.EmpAttachment.ImageUrl,
                };



                if (!HasBankAccount)
                {
                    if (model.AccountNumber != null)
                    {
                        BankAccount bank = new BankAccount()
                        {
                            Id = (int)/*model.*/BankNo,
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
                            AttachmentNo = model.AttachmentNo,
                           // BankNo = bank.Id,
                            SalaryNo = salary.Id,
                            StartDate = model.StartDate,

                        };
                        Db.Entry(employee).State = EntityState.Modified;
                    }
                    else
                    {
                        //insert into bank account
                        BankObj.AccountNumber = model.AccountNumber;
                        BankObj.BankName = model.BankName;
                        BankObj.BankBranch = model.BankBranch;
                        BankObj.Date = DateTime.Now.Date;

                        Db.BankAccounts.Add(BankObj);


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
                            AttachmentNo = model.AttachmentNo,
                            BankNo = BankObj.Id,
                            SalaryNo = salary.Id,
                            StartDate = model.StartDate,

                        };
                        Db.Entry(employee).State = EntityState.Modified;
                    }

                }
                else
                {
                    if (model.AccountNumber != null)
                    {
                        BankAccount bank = new BankAccount()
                        {
                            Id = (int)/*model.*/BankNo,
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
                            AttachmentNo = model.AttachmentNo,
                            BankNo = bank.Id,
                            SalaryNo = salary.Id,
                            StartDate = model.StartDate,

                        };
                        Db.Entry(employee).State = EntityState.Modified;
                    }
                    else
                    {
                        ViewBag.MessageException = string.Format("YOU CANNOT DELETE!", "Index");
                        return View(model);
                    }
                    
                }


               

                ViewBag.EmployeeName = model.FirstName + " " + model.LastName;
                try
                {
                    //if (model.BankNo != null)
                    //{
                    //    Db.Entry(bank).State = EntityState.Modified;
                    //}
                    Db.Entry(salary).State = EntityState.Modified;
                    Db.Entry(attach).State = EntityState.Modified;
                    


                    var counter = Db.TrainingCertificates.Where(e => e.EmployeeNo == id).Count();
                    
                    TrainingCertificate tc = new TrainingCertificate();
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
                    }
                    Db.SaveChanges();

                    ViewBag.Department = Db.Departments.ToList();
                    ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());
                    ViewBag.EmployeeName = model.FirstName + " " + model.LastName;

                    ViewBag.Message = string.Format(Resources.NubiHR.EmployeeHasBeenModifiedSuccesfully, "Index");
                    ModelState.Clear();
                    return View(model);
                }
                catch (DbEntityValidationException ee)
                {
                    foreach (var eve in ee.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
               

            }
            ViewBag.Department = Db.Departments.ToList();
            ViewBag.Position = Db.Positions.Where(pos => pos.DepartmentNo == pos.Employees.Select(emp => emp.DepartmentNo).FirstOrDefault());

            //return RedirectToAction("Edit");
            return View(model);

        }

        // GET: /Details/
        [HttpGet]
        public ActionResult Details(string id)
        {

            ViewBag.TC = Db.TrainingCertificates.Where(x => x.EmployeeNo == id).ToList();

            if (id == null)
            {
               // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }

            return View(Db.Employees.Where(x => x.Id == id));
        }
       
        [HttpPost]
        static public string Encrypt(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }
        //[HttpPost]
        public string Decrypt(string id)
        {

            string cipher = "";

            AspNetUser model = new AspNetUser();

            model = Db.AspNetUsers.Where(x => x.Id == id)
              .Select(y => new AspNetUser
              {
                  PasswordHash = y.PasswordHash,
                  
              }).FirstOrDefault();

            cipher = model.PasswordHash;
            string decryptpwd;
            if (cipher == null)
            {
                decryptpwd = "";
                return decryptpwd;
            }

            decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(cipher);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            ViewBag.ForgetPassword = "Password is: " + decryptpwd;

            return decryptpwd;

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