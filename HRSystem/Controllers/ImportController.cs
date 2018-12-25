using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Xml.Linq;
using OfficeOpenXml.Style;
using HRSystem.Models;
using HRSystem.Manager;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using Excel = Microsoft.Office.Interop.Excel;
using COLOR = System.Drawing;
using System.Web.UI.WebControls;
using System.Text;
using System.Globalization;

namespace HRSystem.Controllers
{
    public class ImportController : BaseController
    {
        DatabaseManager DBMObj = new DatabaseManager();

        NubiDBEntities Db = new NubiDBEntities();

        AttendanceSheet AttendanceObj = new AttendanceSheet();
        ImportLog ImportLogObj = new ImportLog();
        // GET: Import
        [HttpGet]
        public ActionResult Index()
        {
            var current = System.Globalization.CultureInfo.CurrentCulture;
            current.DateTimeFormat.Calendar = new GregorianCalendar();

            return View();
        }
        [HttpPost]
        public ActionResult Index(AttendanceViewModel model)
        {
            
            if (ModelState.IsValid)
            {

                HttpPostedFileBase UploadedFile = Request.Files["ExcelFile"];
                var FN = UploadedFile.FileName;
                var path = Path.Combine(Server.MapPath("~/Attendance/"), FN);
                if (System.IO.File.Exists(path))
                {
                    Random generator = new Random();
                    String r = generator.Next(0, 9999).ToString("D4");
                    path = Path.Combine(Server.MapPath("~/Attendance/"), r + "-" + FN);
                }

                UploadedFile.SaveAs(path);
                List<AttendanceViewModel> RowsWithAllData = new List<AttendanceViewModel>();
                
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(path);
                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                Excel.Range xlRange = xlWorksheet.UsedRange;
                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;
                
                for (int i = 2; i < rowCount + 1; i++)
                {
                    AttendanceViewModel all = new AttendanceViewModel()
                    {
                        Number = xlWorksheet.Cells[i, 1].Value,
                        Name = xlWorksheet.Cells[i, 2].Value,
                        PunchTime = xlWorksheet.Cells[i, 3].Value,
                        WorkState = xlWorksheet.Cells[i, 4].Value,
                        Terminal = null,
                        PunchType = xlWorksheet.Cells[i, 6].Value,

                    };
                    RowsWithAllData.Add(all);

                }


                var fileName = "";
                var exclude = "";
                if (model.Exclude)
                {
                    exclude = "_Excluded";
                }
                if (!model.Automatic)
                {
                    fileName = Path.GetFileNameWithoutExtension(path) + exclude + "_Filtered.xlsx"/* + Path.GetExtension(path)*/;
                }
                else
                {
                    fileName = Path.GetFileNameWithoutExtension(path) + exclude + "_Filtered_Automatic.xlsx"/* + Path.GetExtension(path)*/;
                }

                var outputDir = Path.GetDirectoryName(path) + "\\";
                string curFile = outputDir + fileName;

                if (System.IO.File.Exists(curFile))
                {
                    ViewBag.MessageError = string.Format(Resources.NubiHR.FileExists, "Index", "Import");

                }
                else if (colCount != 6)
                {
                    ViewBag.MessageError = string.Format(Resources.NubiHR.WrongExcelSheet, "Index", "Import");
                }
                else
                {
                    try
                    {

                        using (ExcelPackage package = new ExcelPackage(new FileInfo(outputDir + fileName)))
                        {
                            var list = new List<AttendanceViewModel>();
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Attendance - " + DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString());
                            int NoOfRow = 2;
                           
                            foreach (var ws in Helper.GetWorksheets(package))
                            {

                                string[] columns = new string[] { "Number", "Name", "Punch Time", "Work State", "Terminal", "Punch Type", "No of Hours", "Remaining Hours", "Notes" };
                                int[] columnIndices = new int[columns.Length];
                                var headerRow = new List<string[]>()
                                {
                                      new string[] { "Number", "Name", "Punch Time", "Work State", "Terminal", "Punch Type", "No of Hours", "Remaining Hours", "Notes" }
                                };
                                // Determine the header range (e.g. A1:D1)
                                string headerRange = "A1:" + Char.ConvertFromUtf32(columns[0].Length + 64) + "1";

                                // Popular header row data
                                worksheet.Cells[headerRange].LoadFromArrays(headerRow);

                                //set style to excel sheet

                                worksheet.Cells[headerRange].Style.Font.Name = "Verdana";
                                worksheet.Cells[headerRange].Style.Font.Size = 10;
                                worksheet.Cells[headerRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[headerRange].Style.Fill.BackgroundColor.SetColor(COLOR.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                                worksheet.Cells[headerRange].Style.Font.Color.SetColor(COLOR.Color.White);

                                worksheet.Cells[1, 1, rowCount + 10, colCount + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                               
                                worksheet.Cells["G1:I1"].Style.Font.Name = "Verdana";
                                worksheet.Cells["G1:I1"].Style.Font.Size = 10;
                                worksheet.Cells["G1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells["G1:I1"].Style.Fill.BackgroundColor.SetColor(COLOR.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                                worksheet.Cells["G1:I1"].Style.Font.Color.SetColor(COLOR.Color.White);
                                
                                List<AttendanceViewModel> RowsWithData = new List<AttendanceViewModel>();
                                AttendanceViewModel obj = null;
                                if (!model.Automatic)
                                {

                                    for (var i = 2; i <= rowCount + 1; i++)
                                    {

                                        AttendanceViewModel AddRowsOneByOne = new AttendanceViewModel()
                                        {
                                            Number = xlWorksheet.Cells[i, 1].Value,
                                            Name = xlWorksheet.Cells[i, 2].Value,
                                            PunchTime = xlWorksheet.Cells[i, 3].Value,
                                            WorkState = xlWorksheet.Cells[i, 4].Value,
                                            Terminal = xlWorksheet.Cells[i, 5].Value,
                                            PunchType = xlWorksheet.Cells[i, 6].Value,

                                        };
                                        RowsWithData.Add(AddRowsOneByOne);

                                        var NumberCell = xlWorksheet.Cells[i, 1].Value;
                                        var NameCell = xlWorksheet.Cells[i, 2].Value;
                                        string PunchTimeCell = xlWorksheet.Cells[i, 3].Value;
                                        var WorkStateCell = xlWorksheet.Cells[i, 4].Value;
                                        var TerminalCell = xlWorksheet.Cells[i, 5].Value;
                                        var PunchTypeCell = xlWorksheet.Cells[i, 6].Value;

                                        DayOfWeek day = Convert.ToDateTime(Convert.ToDateTime(PunchTimeCell)).DayOfWeek;

                                        if (model.Exclude && (day.Equals(DayOfWeek.Friday) || day.Equals(DayOfWeek.Saturday)))
                                        {
                                            continue;
                                        }

                                        else if (i == 2)
                                        {
                                            if (WorkStateCell.Equals("Checkin"))
                                            {
                                                worksheet.Cells[NoOfRow, 1].Value = NumberCell;
                                                worksheet.Cells[NoOfRow, 2].Value = NameCell;
                                                worksheet.Cells[NoOfRow, 3].Value = PunchTimeCell;
                                                worksheet.Cells[NoOfRow, 4].Value = WorkStateCell;
                                                worksheet.Cells[NoOfRow, 5].Value = TerminalCell;
                                                worksheet.Cells[NoOfRow, 6].Value = PunchTypeCell;
                                                package.Save();
                                                NoOfRow++;

                                                obj = new AttendanceViewModel();
                                                obj.Number = NumberCell;
                                                obj.Name = NameCell;
                                                obj.PunchTime = PunchTimeCell;
                                                obj.WorkState = WorkStateCell;

                                                list.Add(obj);

                                                 
                                                if(!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                {
                                                    Guid GuIdObjEmp = Guid.NewGuid();

                                                    ImportLogObj.Id = GuIdObjEmp.ToString();
                                                    ImportLogObj.Path = curFile;
                                                    ImportLogObj.FileName = fileName;
                                                    ImportLogObj.Date = DateTime.Now.Date;
                                                    Db.ImportLogs.Add(ImportLogObj);
                                                    Db.SaveChanges();
                                                }
                                               

                                                AttendanceObj.Number = Convert.ToInt32( NumberCell);
                                                AttendanceObj.Name = NameCell.ToString();
                                                AttendanceObj.PunchTime = Convert.ToDateTime(PunchTimeCell);
                                                AttendanceObj.WorkState = WorkStateCell;
                                                AttendanceObj.Terminal = TerminalCell;
                                                AttendanceObj.PunchType = PunchTypeCell;
                                                AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                AttendanceObj.Date = DateTime.Now.Date;
                                                AttendanceObj.NoOfHours = null;
                                                AttendanceObj.RemainingHours = null;

                                                Db.AttendanceSheets.Add(AttendanceObj);
                                                Db.SaveChanges();
                                            }


                                        }
                                        else
                                        {
                                            var CurrentDate = Convert.ToDateTime(PunchTimeCell).ToShortDateString();
                                            var CurrentNumber = NumberCell;
                                            var CurrentStatus = RowsWithData.Where(x => x.PunchTime == PunchTimeCell).Select(x => x.WorkState).FirstOrDefault();
                                            TimeSpan CurrentTime = Convert.ToDateTime(Convert.ToDateTime(PunchTimeCell).ToShortTimeString()).TimeOfDay;

                                            var PreviousDate = Convert.ToDateTime(xlWorksheet.Cells[i - 1, 3].Value).ToShortDateString();
                                            var PreviousNumber = xlWorksheet.Cells[i - 1, 1].Value;
                                            var PreviousStatus = xlWorksheet.Cells[i - 1, 4].Value;

                                            var NextDate = Convert.ToDateTime(xlWorksheet.Cells[i + 1, 3].Value).ToShortDateString();
                                            var NextNumber = xlWorksheet.Cells[i + 1, 1].Value;
                                            var NextStatus = xlWorksheet.Cells[i + 1, 4].Value;



                                            var ListOfCheckOuts = RowsWithAllData.Where(x => (Convert.ToDateTime(x.PunchTime).ToShortDateString() == CurrentDate) && (x.Number == CurrentNumber) && (x.WorkState == "Checkout")).OrderByDescending(x => x.PunchTime)/*.Select(x => Convert.ToDateTime(x.PunchTime).ToShortTimeString())*/.ToList();
                                            var ListOfCheckins = RowsWithAllData.Where(x => (Convert.ToDateTime(x.PunchTime).ToShortDateString() == CurrentDate) && (x.Number == CurrentNumber) && (x.WorkState == "Checkin"))/*.OrderByDescending(x => x.PunchTime).Select(x => Convert.ToDateTime(x.PunchTime).ToShortTimeString())*/.ToList();

                                            // var FirstCheckOutOfTheCurrentDate = RowsWithAllData.Where(x => (Convert.ToDateTime(x.PunchTime).ToShortDateString() == CurrentDate) && (x.Number == CurrentNumber) && (x.WorkState == "Checkout")).FirstOrDefault();


                                            if (CurrentStatus == "Checkin")
                                            {
                                                DateTime d = Convert.ToDateTime(Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).ToShortDateString());
                                                var PreviousDateOfTheFilteredExcel = Convert.ToDateTime(d).ToShortDateString();

                                                if (PreviousDate != CurrentDate)
                                                {
                                                    foreach (var item in ListOfCheckins)
                                                    {

                                                        TimeSpan start = new TimeSpan(06, 0, 0); //6 o'clock
                                                        TimeSpan end = new TimeSpan(14, 0, 0); //14 o'clock
                                                        TimeSpan now = Convert.ToDateTime(item.PunchTime).TimeOfDay;

                                                        if ((now.Hours >= start.Hours) && (now.Hours <= end.Hours))
                                                        {
                                                            worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                            worksheet.Cells[NoOfRow, 2].Value = item.Name;
                                                            worksheet.Cells[NoOfRow, 3].Value = item.PunchTime;
                                                            worksheet.Cells[NoOfRow, 4].Value = item.WorkState;
                                                            worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                            worksheet.Cells[NoOfRow, 6].Value = item.PunchType;
                                                            package.Save();
                                                            NoOfRow++;

                                                            obj = new AttendanceViewModel();
                                                            obj.Number = NumberCell;
                                                            obj.Name = NameCell;
                                                            obj.PunchTime = item.PunchTime;
                                                            obj.WorkState = item.WorkState;

                                                            list.Add(obj);

                                                             
                                                            if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                            {
                                                                Guid GuIdObjEmp = Guid.NewGuid();

                                                                ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                ImportLogObj.Path = curFile;
                                                                ImportLogObj.FileName = fileName;
                                                                ImportLogObj.Date = DateTime.Now.Date;
                                                                Db.ImportLogs.Add(ImportLogObj);
                                                                Db.SaveChanges();
                                                            }

                                                            AttendanceObj.Number = Convert.ToInt32(item.Number);
                                                            AttendanceObj.Name = item.Name.ToString();
                                                            AttendanceObj.PunchTime = Convert.ToDateTime(item.PunchTime);
                                                            AttendanceObj.WorkState = item.WorkState;
                                                            AttendanceObj.Terminal = item.Terminal;
                                                            AttendanceObj.PunchType = item.PunchType;
                                                            AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                            AttendanceObj.Date = DateTime.Now.Date;
                                                            AttendanceObj.NoOfHours = null;
                                                            AttendanceObj.RemainingHours = null;

                                                            Db.AttendanceSheets.Add(AttendanceObj);
                                                            Db.SaveChanges();

                                                            break;
                                                        }

                                                    }
                                                }
                                                else if (CurrentDate != PreviousDateOfTheFilteredExcel)
                                                {
                                                    TimeSpan t = Convert.ToDateTime(PunchTimeCell).TimeOfDay;
                                                    foreach (var item in ListOfCheckins.Where(x => (t.Hours >= 6) && t.Hours <= 14))
                                                    {

                                                        TimeSpan start = new TimeSpan(06, 0, 0); //6 o'clock
                                                        TimeSpan end = new TimeSpan(14, 0, 0); //13 o'clock
                                                        TimeSpan now = Convert.ToDateTime(item.PunchTime).TimeOfDay;

                                                        if ((now.Hours >= start.Hours) && (now.Hours <= end.Hours))
                                                        {
                                                            worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                            worksheet.Cells[NoOfRow, 2].Value = item.Name;
                                                            worksheet.Cells[NoOfRow, 3].Value = item.PunchTime;
                                                            worksheet.Cells[NoOfRow, 4].Value = item.WorkState;
                                                            worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                            worksheet.Cells[NoOfRow, 6].Value = item.PunchType;
                                                            package.Save();
                                                            NoOfRow++;

                                                            obj = new AttendanceViewModel();
                                                            obj.Number = NumberCell;
                                                            obj.Name = NameCell;
                                                            obj.PunchTime = item.PunchTime;
                                                            obj.WorkState = item.WorkState;

                                                            list.Add(obj);

                                                             
                                                            if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                            {
                                                                Guid GuIdObjEmp = Guid.NewGuid();

                                                                ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                ImportLogObj.Path = curFile;
                                                                ImportLogObj.FileName = fileName;
                                                                ImportLogObj.Date = DateTime.Now.Date;
                                                                Db.ImportLogs.Add(ImportLogObj);
                                                                Db.SaveChanges();
                                                            }

                                                            AttendanceObj.Number = Convert.ToInt32(item.Number);
                                                            AttendanceObj.Name = item.Name.ToString();
                                                            AttendanceObj.PunchTime = Convert.ToDateTime(item.PunchTime);
                                                            AttendanceObj.WorkState = item.WorkState;
                                                            AttendanceObj.Terminal = item.Terminal;
                                                            AttendanceObj.PunchType = item.PunchType;
                                                            AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                            AttendanceObj.Date = DateTime.Now.Date;
                                                            AttendanceObj.NoOfHours = null;
                                                            AttendanceObj.RemainingHours = null;

                                                            Db.AttendanceSheets.Add(AttendanceObj);
                                                            Db.SaveChanges();

                                                            break;
                                                        }

                                                    }
                                                }


                                            }

                                            else if (CurrentStatus == "Checkout")
                                            {
                                                DateTime x = Convert.ToDateTime(Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).ToShortDateString());
                                                var PreviousDateOfTheFilteredExcel = Convert.ToDateTime(x).ToShortDateString();

                                                if (CurrentTime.Hours <= 11 && CurrentTime.Hours >= 6 && NextDate == CurrentDate && CurrentDate != PreviousDateOfTheFilteredExcel)
                                                {

                                                    TimeSpan now = Convert.ToDateTime(PunchTimeCell).TimeOfDay;
                                                    worksheet.Cells[NoOfRow, 1].Value = NumberCell;
                                                    worksheet.Cells[NoOfRow, 2].Value = NameCell;
                                                    worksheet.Cells[NoOfRow, 3].Value = PunchTimeCell;
                                                    worksheet.Cells[NoOfRow, 4].Value = "Checkin";
                                                    worksheet.Cells[NoOfRow, 5].Value = TerminalCell;
                                                    worksheet.Cells[NoOfRow, 6].Value = PunchTypeCell;

                                                    package.Save();
                                                    NoOfRow++;

                                                    obj = new AttendanceViewModel();
                                                    obj.Number = NumberCell;
                                                    obj.Name = NameCell;
                                                    obj.PunchTime = PunchTimeCell;
                                                    obj.WorkState = "Checkin";

                                                    list.Add(obj);

                                                     
                                                    if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                    {
                                                        Guid GuIdObjEmp = Guid.NewGuid();

                                                        ImportLogObj.Id = GuIdObjEmp.ToString();
                                                        ImportLogObj.Path = curFile;
                                                        ImportLogObj.FileName = fileName;
                                                        ImportLogObj.Date = DateTime.Now.Date;
                                                        Db.ImportLogs.Add(ImportLogObj);
                                                        Db.SaveChanges();
                                                    }

                                                    AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                    AttendanceObj.Name = NameCell.ToString();
                                                    AttendanceObj.PunchTime = Convert.ToDateTime(PunchTimeCell);
                                                    AttendanceObj.WorkState = "Checkin";
                                                    AttendanceObj.Terminal = TerminalCell;
                                                    AttendanceObj.PunchType = PunchTypeCell;
                                                    AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                    AttendanceObj.Date = DateTime.Now.Date;
                                                    AttendanceObj.NoOfHours = null;
                                                    AttendanceObj.RemainingHours = null;

                                                    Db.AttendanceSheets.Add(AttendanceObj);
                                                    Db.SaveChanges();
                                                }
                                                else if (NextDate != CurrentDate)
                                                {
                                                    int Count = 0;
                                                    foreach (var item in ListOfCheckOuts)
                                                    {
                                                        TimeSpan start = new TimeSpan(11, 0, 0); //11 o'clock
                                                        TimeSpan end = new TimeSpan(18, 0, 0); //18 o'clock
                                                        TimeSpan now = Convert.ToDateTime(item.PunchTime).TimeOfDay;

                                                        TimeSpan Eight = new TimeSpan(8, 0, 0);
                                                        TimeSpan CheckInTime;
                                                        //DateTime x = Convert.ToDateTime(Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).ToShortDateString());
                                                        //var PreviousDateOfTheFilteredExcel = Convert.ToDateTime(x).ToShortDateString();
                                                        TimeSpan NoCheckInFound = new TimeSpan(0, 0, 0);

                                                        if (CurrentDate == PreviousDateOfTheFilteredExcel)
                                                        {
                                                            CheckInTime = Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).TimeOfDay;

                                                        }
                                                        else
                                                        {
                                                            CheckInTime = new TimeSpan(0, 0, 0);

                                                        }

                                                        if ((now.Hours >= start.Hours) && (now.Hours < end.Hours))
                                                        {
                                                            worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                            worksheet.Cells[NoOfRow, 2].Value = item.Name;
                                                            worksheet.Cells[NoOfRow, 3].Value = item.PunchTime;
                                                            worksheet.Cells[NoOfRow, 4].Value = item.WorkState;
                                                            worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                            worksheet.Cells[NoOfRow, 6].Value = item.PunchType;


                                                            obj = new AttendanceViewModel();
                                                            obj.Number = NumberCell;
                                                            obj.Name = NameCell;
                                                            obj.PunchTime = item.PunchTime;
                                                            obj.WorkState = item.WorkState;

                                                            


                                                             
                                                            if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                            {
                                                                Guid GuIdObjEmp = Guid.NewGuid();

                                                                ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                ImportLogObj.Path = curFile;
                                                                ImportLogObj.FileName = fileName;
                                                                ImportLogObj.Date = DateTime.Now.Date;
                                                                Db.ImportLogs.Add(ImportLogObj);
                                                                Db.SaveChanges();
                                                            }

                                                            AttendanceObj.Number = Convert.ToInt32(item.Number);
                                                            AttendanceObj.Name = item.Name.ToString();
                                                            AttendanceObj.PunchTime = Convert.ToDateTime(item.PunchTime);
                                                            AttendanceObj.WorkState = item.WorkState;
                                                            AttendanceObj.Terminal = item.Terminal;
                                                            AttendanceObj.PunchType = item.PunchType;
                                                            AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                            AttendanceObj.Date = DateTime.Now.Date;

                                                            

                                                            if (CheckInTime.Equals(NoCheckInFound))
                                                            {
                                                                worksheet.Cells[NoOfRow, 7].Value = "00:00:00";
                                                                worksheet.Cells[NoOfRow, 8].Value = "00:00:00";
                                                                worksheet.Cells[NoOfRow, 9].Value = "No Check In Found";

                                                                obj.NoOfHours = "00:00:00";
                                                                obj.RemainingHours = "00:00:00";
                                                                list.Add(obj);

                                                                AttendanceObj.NoOfHours = "00:00:00";
                                                                AttendanceObj.RemainingHours = "00:00:00";

                                                                Db.AttendanceSheets.Add(AttendanceObj);
                                                                Db.SaveChanges();
                                                            }
                                                            else
                                                            {
                                                                worksheet.Cells[NoOfRow, 7].Value = (now - CheckInTime).ToString();
                                                                worksheet.Cells[NoOfRow, 8].Value = ((now - CheckInTime) - Eight).ToString();

                                                                obj.NoOfHours = (now - CheckInTime).ToString();
                                                                obj.RemainingHours = ((now - CheckInTime) - Eight).ToString();
                                                                list.Add(obj);

                                                                AttendanceObj.NoOfHours = (now - CheckInTime).ToString();
                                                                AttendanceObj.RemainingHours = ((now - CheckInTime) - Eight).ToString();

                                                                Db.AttendanceSheets.Add(AttendanceObj);
                                                                Db.SaveChanges();
                                                            }
                                                            package.Save();
                                                            NoOfRow++;



                                                            break;
                                                        }
                                                        else
                                                        {
                                                            Count++;
                                                            if (Count == ListOfCheckOuts.Count())
                                                            {
                                                                worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                                worksheet.Cells[NoOfRow, 2].Value = item.Name;
                                                                worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + "6:00:00 PM";
                                                                worksheet.Cells[NoOfRow, 4].Value = item.WorkState;
                                                                worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                                worksheet.Cells[NoOfRow, 6].Value = item.PunchType;
                                                                TimeSpan Six = new TimeSpan(18, 0, 0);

                                                                obj = new AttendanceViewModel();
                                                                obj.Number = NumberCell;
                                                                obj.Name = NameCell;
                                                                obj.PunchTime = CurrentDate + " " + "6:00:00 PM";
                                                                obj.WorkState = item.WorkState;


                                                                 
                                                                if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                                {
                                                                    Guid GuIdObjEmp = Guid.NewGuid();

                                                                    ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                    ImportLogObj.Path = curFile;
                                                                    ImportLogObj.FileName = fileName;
                                                                    ImportLogObj.Date = DateTime.Now.Date;
                                                                    Db.ImportLogs.Add(ImportLogObj);
                                                                    Db.SaveChanges();
                                                                }

                                                                AttendanceObj.Number = Convert.ToInt32(item.Number);
                                                                AttendanceObj.Name = item.Name.ToString();
                                                                AttendanceObj.PunchTime = Convert.ToDateTime(CurrentDate + " " + "6:00:00 PM");
                                                                AttendanceObj.WorkState = item.WorkState;
                                                                AttendanceObj.Terminal = item.Terminal;
                                                                AttendanceObj.PunchType = item.PunchType;
                                                                AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                                AttendanceObj.Date = DateTime.Now.Date;

                                                                
                                                                if (CheckInTime.Equals(NoCheckInFound))
                                                                {
                                                                    worksheet.Cells[NoOfRow, 7].Value = "00:00:00";
                                                                    worksheet.Cells[NoOfRow, 8].Value = "00:00:00";
                                                                    worksheet.Cells[NoOfRow, 9].Value = "No Check In Found";

                                                                    obj.NoOfHours = "00:00:00";
                                                                    obj.RemainingHours = "00:00:00";
                                                                    list.Add(obj);

                                                                    AttendanceObj.NoOfHours = "00:00:00";
                                                                    AttendanceObj.RemainingHours = "00:00:00";

                                                                    Db.AttendanceSheets.Add(AttendanceObj);
                                                                    Db.SaveChanges();

                                                                }
                                                                else
                                                                {
                                                                    worksheet.Cells[NoOfRow, 7].Value = (Six - CheckInTime).ToString();
                                                                    worksheet.Cells[NoOfRow, 8].Value = ((Six - CheckInTime) - Eight).ToString();

                                                                    obj.NoOfHours = (Six - CheckInTime).ToString();
                                                                    obj.RemainingHours = ((Six - CheckInTime) - Eight).ToString();
                                                                    list.Add(obj);

                                                                    AttendanceObj.NoOfHours = (Six - CheckInTime).ToString();
                                                                    AttendanceObj.RemainingHours = ((Six - CheckInTime) - Eight).ToString();

                                                                    Db.AttendanceSheets.Add(AttendanceObj);
                                                                    Db.SaveChanges();
                                                                }
                                                                package.Save();
                                                                NoOfRow++;

                                                                
                                                                
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }

                                            }

                                            else
                                            {

                                            }


                                        }

                                    }
                                }
                                else //if AUTOMATIC is checked
                                {
                                    
                                    for (var i = 2; i <= rowCount + 1; i++)
                                    {
                                        
                                        AttendanceViewModel AddRowsOneByOne = new AttendanceViewModel()
                                        {
                                            Number = xlWorksheet.Cells[i, 1].Value,
                                            Name = xlWorksheet.Cells[i, 2].Value,
                                            PunchTime = xlWorksheet.Cells[i, 3].Value,
                                            WorkState = xlWorksheet.Cells[i, 4].Value,
                                            Terminal = xlWorksheet.Cells[i, 5].Value,
                                            PunchType = xlWorksheet.Cells[i, 6].Value,

                                        };
                                        RowsWithData.Add(AddRowsOneByOne); 


                                        var NumberCell = xlWorksheet.Cells[i, 1].Value;
                                        var NameCell = xlWorksheet.Cells[i, 2].Value;
                                        var PunchTimeCell = xlWorksheet.Cells[i, 3].Value;
                                        var WorkStateCell = xlWorksheet.Cells[i, 4].Value;
                                        var TerminalCell = xlWorksheet.Cells[i, 5].Value;
                                        var PunchTypeCell = xlWorksheet.Cells[i, 6].Value;

                                        DayOfWeek day = Convert.ToDateTime(Convert.ToDateTime(PunchTimeCell)).DayOfWeek;

                                        if (model.Exclude && (day.Equals(DayOfWeek.Friday) || day.Equals(DayOfWeek.Saturday)))
                                        {
                                            continue;
                                        }
                                        else if (i == 2)
                                        {

                                            var CurrentDate = Convert.ToDateTime(PunchTimeCell).ToShortDateString();
                                            var CurrentNumber = NumberCell;
                                            var CurrentStatus = RowsWithData.Where(x => x.PunchTime == PunchTimeCell).Select(x => x.WorkState).FirstOrDefault();
                                            TimeSpan CurrentTime = Convert.ToDateTime(Convert.ToDateTime(PunchTimeCell).ToShortTimeString()).TimeOfDay;


                                            var NextDate = Convert.ToDateTime(xlWorksheet.Cells[i + 1, 3].Value).ToShortDateString();
                                            var NextNumber = xlWorksheet.Cells[i + 1, 1].Value;
                                            var NextStatus = xlWorksheet.Cells[i + 1, 4].Value;


                                            TimeSpan Eight = new TimeSpan(8, 0, 0);
                                            TimeSpan Twelve = new TimeSpan(12, 0, 0);
                                            TimeSpan Six = new TimeSpan(6, 0, 0);
                                            var ListOfCheckOuts = RowsWithAllData.Where(x => (Convert.ToDateTime(x.PunchTime).ToShortDateString() == CurrentDate) && (x.Number == CurrentNumber) && (x.WorkState == "Checkout")).OrderByDescending(x => x.PunchTime)/*.Select(x => Convert.ToDateTime(x.PunchTime).ToShortTimeString())*/.ToList();

                                            if (WorkStateCell.Equals("Checkin"))
                                            {
                                                worksheet.Cells[NoOfRow, 1].Value = NumberCell;
                                                worksheet.Cells[NoOfRow, 2].Value = NameCell;
                                                worksheet.Cells[NoOfRow, 3].Value = PunchTimeCell;
                                                worksheet.Cells[NoOfRow, 4].Value = WorkStateCell;
                                                worksheet.Cells[NoOfRow, 5].Value = TerminalCell;
                                                worksheet.Cells[NoOfRow, 6].Value = PunchTypeCell;
                                                package.Save();
                                                NoOfRow++;

                                                
                                                obj = new AttendanceViewModel();
                                                obj.Number = NumberCell;
                                                obj.Name = NameCell;
                                                obj.PunchTime = PunchTimeCell;
                                                obj.WorkState = WorkStateCell;
                                                
                                                list.Add(obj);


                                                 
                                                if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                {
                                                    Guid GuIdObjEmp = Guid.NewGuid();

                                                    ImportLogObj.Id = GuIdObjEmp.ToString();
                                                    ImportLogObj.Path = curFile;
                                                    ImportLogObj.FileName = fileName;
                                                    ImportLogObj.Date = DateTime.Now.Date;
                                                    Db.ImportLogs.Add(ImportLogObj);
                                                    Db.SaveChanges();
                                                }


                                                AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                AttendanceObj.Name = NameCell.ToString();
                                                AttendanceObj.PunchTime = Convert.ToDateTime(PunchTimeCell);
                                                AttendanceObj.WorkState = WorkStateCell;
                                                AttendanceObj.Terminal = TerminalCell;
                                                AttendanceObj.PunchType = PunchTypeCell;
                                                AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                AttendanceObj.Date = DateTime.Now.Date;
                                                AttendanceObj.NoOfHours = null;
                                                AttendanceObj.RemainingHours = null;

                                                Db.AttendanceSheets.Add(AttendanceObj);
                                                Db.SaveChanges();

                                                if (ListOfCheckOuts.Count == 0)
                                                {
                                                    TimeSpan CheckInTime = Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).TimeOfDay;

                                                    worksheet.Cells[NoOfRow, 1].Value = NumberCell;
                                                    worksheet.Cells[NoOfRow, 2].Value = NameCell;
                                                    worksheet.Cells[NoOfRow, 4].Value = "Checkout";
                                                    worksheet.Cells[NoOfRow, 5].Value = TerminalCell;
                                                    worksheet.Cells[NoOfRow, 6].Value = PunchTypeCell;

                                                    if (CheckInTime.Hours > 6)
                                                    {
                                                        TimeSpan now = Convert.ToDateTime("18:00:00").TimeOfDay;

                                                        worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + "6:00:00" + " " + "PM";
                                                        worksheet.Cells[NoOfRow, 7].Value = (now - CheckInTime).ToString();
                                                        worksheet.Cells[NoOfRow, 8].Value = ((now - CheckInTime) - Eight).ToString();
                                                        worksheet.Cells[NoOfRow, 9].Value = "No Checkout Found (generated and became after 6)";

                                                        package.Save();
                                                        NoOfRow++;

                                                        obj = new AttendanceViewModel();
                                                        obj.Number = NumberCell;
                                                        obj.Name = NameCell;
                                                        obj.PunchTime = CurrentDate + " " + "6:00:00" + " " + "PM"; ;
                                                        obj.WorkState = "Checkout";
                                                        obj.NoOfHours = (now - CheckInTime).ToString();
                                                        obj.RemainingHours = ((now - CheckInTime) - Eight).ToString();
                                                        list.Add(obj);


                                                        if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                        {
                                                            Guid GuIdObjEmp = Guid.NewGuid();

                                                            ImportLogObj.Id = GuIdObjEmp.ToString();
                                                            ImportLogObj.Path = curFile;
                                                            ImportLogObj.FileName = fileName;
                                                            ImportLogObj.Date = DateTime.Now.Date;
                                                            Db.ImportLogs.Add(ImportLogObj);
                                                            Db.SaveChanges();
                                                        }


                                                        AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                        AttendanceObj.Name = NameCell.ToString();
                                                        AttendanceObj.PunchTime = Convert.ToDateTime(PunchTimeCell);
                                                        AttendanceObj.WorkState = WorkStateCell;
                                                        AttendanceObj.Terminal = TerminalCell;
                                                        AttendanceObj.PunchType = PunchTypeCell;
                                                        AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                        AttendanceObj.Date = DateTime.Now.Date;
                                                        AttendanceObj.NoOfHours = (now - CheckInTime).ToString();
                                                        AttendanceObj.RemainingHours = ((now - CheckInTime) - Eight).ToString();

                                                        Db.AttendanceSheets.Add(AttendanceObj);
                                                        Db.SaveChanges();
                                                    }
                                                    else
                                                    {

                                                        worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + ((CheckInTime + Eight) - Twelve) + " " + "PM";
                                                        worksheet.Cells[NoOfRow, 7].Value = "8:00:00";
                                                        worksheet.Cells[NoOfRow, 8].Value = "00:00:00";
                                                        worksheet.Cells[NoOfRow, 9].Value = "No Checkout Found (generated 2)";

                                                        package.Save();
                                                        NoOfRow++;

                                                        obj = new AttendanceViewModel();
                                                        obj.Number = NumberCell;
                                                        obj.Name = NameCell;
                                                        obj.PunchTime = CurrentDate + " " + ((CheckInTime + Eight) - Twelve) + " " + "PM"; ;
                                                        obj.WorkState = "Checkout";
                                                        obj.NoOfHours = "8:00:00";
                                                        obj.RemainingHours = "00:00:00";
                                                        list.Add(obj);

                                                        AttendanceObj.NoOfHours = "8:00:00";
                                                        AttendanceObj.RemainingHours = "00:00:00";

                                                        if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                        {
                                                            Guid GuIdObjEmp = Guid.NewGuid();

                                                            ImportLogObj.Id = GuIdObjEmp.ToString();
                                                            ImportLogObj.Path = curFile;
                                                            ImportLogObj.FileName = fileName;
                                                            ImportLogObj.Date = DateTime.Now.Date;
                                                            Db.ImportLogs.Add(ImportLogObj);
                                                            Db.SaveChanges();
                                                        }


                                                        AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                        AttendanceObj.Name = NameCell.ToString();
                                                        AttendanceObj.PunchTime = Convert.ToDateTime(PunchTimeCell);
                                                        AttendanceObj.WorkState = WorkStateCell;
                                                        AttendanceObj.Terminal = TerminalCell;
                                                        AttendanceObj.PunchType = PunchTypeCell;
                                                        AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                        AttendanceObj.Date = DateTime.Now.Date;
                                                        
                                                        Db.AttendanceSheets.Add(AttendanceObj);
                                                        Db.SaveChanges();
                                                    }

                                                }
                                            }

                                        }
                                        else
                                        {
                                            var CurrentDate = Convert.ToDateTime(PunchTimeCell).ToShortDateString();
                                            var CurrentNumber = NumberCell;
                                            var CurrentStatus = RowsWithData.Where(x => x.PunchTime == PunchTimeCell).Select(x => x.WorkState).FirstOrDefault();
                                            TimeSpan CurrentTime = Convert.ToDateTime(Convert.ToDateTime(PunchTimeCell).ToShortTimeString()).TimeOfDay;

                                            var PreviousDate = Convert.ToDateTime(xlWorksheet.Cells[i - 1, 3].Value).ToShortDateString();
                                            var PreviousNumber = xlWorksheet.Cells[i - 1, 1].Value;
                                            var PreviousStatus = xlWorksheet.Cells[i - 1, 4].Value;

                                            var NextDate = Convert.ToDateTime(xlWorksheet.Cells[i + 1, 3].Value).ToShortDateString();
                                            var NextNumber = xlWorksheet.Cells[i + 1, 1].Value;
                                            var NextStatus = xlWorksheet.Cells[i + 1, 4].Value;



                                            var ListOfCheckOuts = RowsWithAllData.Where(x => (Convert.ToDateTime(x.PunchTime).ToShortDateString() == CurrentDate) && (x.Number == CurrentNumber) && (x.WorkState == "Checkout")).OrderByDescending(x => x.PunchTime)/*.Select(x => Convert.ToDateTime(x.PunchTime).ToShortTimeString())*/.ToList();
                                            var ListOfCheckins = RowsWithAllData.Where(x => (Convert.ToDateTime(x.PunchTime).ToShortDateString() == CurrentDate) && (x.Number == CurrentNumber) && (x.WorkState == "Checkin"))/*.OrderByDescending(x => x.PunchTime).Select(x => Convert.ToDateTime(x.PunchTime).ToShortTimeString())*/.ToList();



                                            if (CurrentStatus == "Checkin")
                                            {
                                                DateTime d = Convert.ToDateTime(Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).ToShortDateString());
                                                var PreviousDateOfTheFilteredExcel = Convert.ToDateTime(d).ToShortDateString();

                                                if (PreviousDate != CurrentDate)
                                                {
                                                    foreach (var item in ListOfCheckins)
                                                    {

                                                        TimeSpan start = new TimeSpan(06, 0, 0); //6 o'clock
                                                        TimeSpan end = new TimeSpan(14, 0, 0); //13 o'clock
                                                        TimeSpan now = Convert.ToDateTime(item.PunchTime).TimeOfDay;
                                                        TimeSpan Eight = new TimeSpan(8, 0, 0);
                                                        TimeSpan Twelve = new TimeSpan(12, 0, 0);


                                                        if ((now.Hours >= start.Hours) && (now.Hours <= end.Hours))
                                                        {
                                                            worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                            worksheet.Cells[NoOfRow, 2].Value = item.Name;
                                                            worksheet.Cells[NoOfRow, 3].Value = item.PunchTime;
                                                            worksheet.Cells[NoOfRow, 4].Value = item.WorkState;
                                                            worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                            worksheet.Cells[NoOfRow, 6].Value = item.PunchType;
                                                            package.Save();
                                                            NoOfRow++;

                                                            obj = new AttendanceViewModel();
                                                            obj.Number = NumberCell;
                                                            obj.Name = NameCell;
                                                            obj.PunchTime = item.PunchTime;
                                                            obj.WorkState = item.WorkState;
                                                            list.Add(obj);

                                                             
                                                            if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                            {
                                                                Guid GuIdObjEmp = Guid.NewGuid();

                                                                ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                ImportLogObj.Path = curFile;
                                                                ImportLogObj.FileName = fileName;
                                                                ImportLogObj.Date = DateTime.Now.Date;
                                                                Db.ImportLogs.Add(ImportLogObj);
                                                                Db.SaveChanges();
                                                            }

                                                            AttendanceObj.Number = Convert.ToInt32(item.Number);
                                                            AttendanceObj.Name = item.Name.ToString();
                                                            AttendanceObj.PunchTime = Convert.ToDateTime(item.PunchTime);
                                                            AttendanceObj.WorkState = item.WorkState;
                                                            AttendanceObj.Terminal = item.Terminal;
                                                            AttendanceObj.PunchType = item.PunchType;
                                                            AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                            AttendanceObj.Date = DateTime.Now.Date;
                                                            AttendanceObj.NoOfHours = null;
                                                            AttendanceObj.RemainingHours = null;

                                                            Db.AttendanceSheets.Add(AttendanceObj);
                                                            Db.SaveChanges();

                                                            if (ListOfCheckOuts.Count == 0)
                                                            {
                                                                TimeSpan CheckInTime = Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).TimeOfDay;
                                                                TimeSpan CO = ((CheckInTime + Eight) - Twelve);
                                                                TimeSpan Six = new TimeSpan(6, 0, 0);

                                                                worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                                worksheet.Cells[NoOfRow, 2].Value = item.Name;


                                                                obj = new AttendanceViewModel();
                                                                obj.Number = NumberCell;
                                                                obj.Name = NameCell;
                                                                obj.WorkState = "Checkout";
                                                                

                                                                if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                                {
                                                                    Guid GuIdObjEmp = Guid.NewGuid();

                                                                    ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                    ImportLogObj.Path = curFile;
                                                                    ImportLogObj.FileName = fileName;
                                                                    ImportLogObj.Date = DateTime.Now.Date;
                                                                    Db.ImportLogs.Add(ImportLogObj);
                                                                    Db.SaveChanges();
                                                                }

                                                                AttendanceObj.Number = Convert.ToInt32(item.Number);
                                                                AttendanceObj.Name = item.Name.ToString();
                                                                AttendanceObj.WorkState = "Checkout";
                                                                AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                                AttendanceObj.Date = DateTime.Now.Date;
                                                                AttendanceObj.Terminal = item.Terminal;
                                                                AttendanceObj.PunchType = item.PunchType;



                                                                if (CO.Hours >= 6)
                                                                {
                                                                    TimeSpan noww = Convert.ToDateTime("18:00:00").TimeOfDay;

                                                                    worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + "6:00:00" + " " + "PM";
                                                                    worksheet.Cells[NoOfRow, 7].Value = (noww - CheckInTime).ToString();
                                                                    worksheet.Cells[NoOfRow, 8].Value = ((noww - CheckInTime) - Eight).ToString();
                                                                    worksheet.Cells[NoOfRow, 9].Value = "No Checkout Found (generated and became after 6)";

                                                                    obj.PunchTime = CurrentDate + " " + "6:00:00" + " " + "PM";
                                                                    obj.NoOfHours = (noww - CheckInTime).ToString();
                                                                    obj.RemainingHours = ((noww - CheckInTime) - Eight).ToString();
                                                                    list.Add(obj);

                                                                    AttendanceObj.PunchTime = Convert.ToDateTime(CurrentDate + " " + "6:00:00" + " " + "PM");
                                                                    AttendanceObj.NoOfHours = (noww - CheckInTime).ToString();
                                                                    AttendanceObj.RemainingHours = ((noww - CheckInTime) - Eight).ToString();

                                                                    Db.AttendanceSheets.Add(AttendanceObj);
                                                                    Db.SaveChanges();
                                                                }
                                                                else
                                                                {
                                                                    worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + CO + " " + "PM";
                                                                    worksheet.Cells[NoOfRow, 7].Value = "8:00:00";
                                                                    worksheet.Cells[NoOfRow, 8].Value = "00:00:00";
                                                                    worksheet.Cells[NoOfRow, 9].Value = "No Checkout Found (generated)";

                                                                    obj.PunchTime = CurrentDate + " " + CO + " " + "PM";
                                                                    obj.NoOfHours = "8:00:00";
                                                                    obj.RemainingHours = "00:00:00";
                                                                    list.Add(obj);

                                                                    AttendanceObj.PunchTime = Convert.ToDateTime(CurrentDate + " " + CO + " " + "PM");
                                                                    AttendanceObj.NoOfHours = "8:00:00";
                                                                    AttendanceObj.RemainingHours = "00:00:00";

                                                                    Db.AttendanceSheets.Add(AttendanceObj);
                                                                    Db.SaveChanges();



                                                                }

                                                                
                                                                
                                                                
                                                                worksheet.Cells[NoOfRow, 4].Value = "Checkout";
                                                                worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                                worksheet.Cells[NoOfRow, 6].Value = item.PunchType;

                                                                package.Save();
                                                                NoOfRow++;

                                                            }



                                                            break;
                                                        }

                                                    }
                                                }
                                                else if (CurrentDate != PreviousDateOfTheFilteredExcel)
                                                {
                                                    TimeSpan t = Convert.ToDateTime(PunchTimeCell).TimeOfDay;
                                                    foreach (var item in ListOfCheckins.Where(x => (t.Hours >= 6) && t.Hours <= 14))
                                                    {

                                                        TimeSpan start = new TimeSpan(06, 0, 0); //6 o'clock
                                                        TimeSpan end = new TimeSpan(14, 0, 0); //13 o'clock
                                                        TimeSpan now = Convert.ToDateTime(item.PunchTime).TimeOfDay;
                                                        TimeSpan Eight = new TimeSpan(8, 0, 0);
                                                        TimeSpan Twelve = new TimeSpan(12, 0, 0);

                                                        if ((now.Hours >= start.Hours) && (now.Hours <= end.Hours))
                                                        {
                                                            worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                            worksheet.Cells[NoOfRow, 2].Value = item.Name;
                                                            worksheet.Cells[NoOfRow, 3].Value = item.PunchTime;
                                                            worksheet.Cells[NoOfRow, 4].Value = item.WorkState;
                                                            worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                            worksheet.Cells[NoOfRow, 6].Value = item.PunchType;
                                                            package.Save();
                                                            NoOfRow++;

                                                            obj = new AttendanceViewModel();
                                                            obj.Number = NumberCell;
                                                            obj.Name = NameCell;
                                                            obj.PunchTime = item.PunchTime;
                                                            obj.WorkState = item.WorkState;
                                                            list.Add(obj);

                                                             
                                                            if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                            {
                                                                Guid GuIdObjEmp = Guid.NewGuid();

                                                                ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                ImportLogObj.Path = curFile;
                                                                ImportLogObj.FileName = fileName;
                                                                ImportLogObj.Date = DateTime.Now.Date;
                                                                Db.ImportLogs.Add(ImportLogObj);
                                                                Db.SaveChanges();
                                                            }


                                                            AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                            AttendanceObj.Name = NameCell.ToString();
                                                            AttendanceObj.PunchTime = Convert.ToDateTime(item.PunchTime);
                                                            AttendanceObj.WorkState = item.WorkState;
                                                            AttendanceObj.Terminal = item.Terminal;
                                                            AttendanceObj.PunchType = item.PunchType;
                                                            AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                            AttendanceObj.Date = DateTime.Now.Date;
                                                            AttendanceObj.NoOfHours = null;
                                                            AttendanceObj.RemainingHours = null;

                                                            Db.AttendanceSheets.Add(AttendanceObj);
                                                            Db.SaveChanges();

                                                            if (ListOfCheckOuts.Count == 0)
                                                            {
                                                                TimeSpan CheckInTime = Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).TimeOfDay;


                                                                worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                                worksheet.Cells[NoOfRow, 2].Value = item.Name;
                                                                worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + ((CheckInTime + Eight) - Twelve) + " " + "PM";
                                                                worksheet.Cells[NoOfRow, 4].Value = "Checkout";
                                                                worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                                worksheet.Cells[NoOfRow, 6].Value = item.PunchType;
                                                                worksheet.Cells[NoOfRow, 7].Value = "8:00:00";
                                                                worksheet.Cells[NoOfRow, 8].Value = "00:00:00";
                                                                package.Save();
                                                                NoOfRow++;

                                                                obj = new AttendanceViewModel();
                                                                obj.Number = NumberCell;
                                                                obj.Name = NameCell;
                                                                obj.PunchTime = CurrentDate + " " + ((CheckInTime + Eight) - Twelve) + " " + "PM";
                                                                obj.WorkState = "Checkout";
                                                                obj.NoOfHours = "8:00:00";
                                                                obj.RemainingHours = "00:00:00";
                                                                list.Add(obj);

                                                                if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                                {
                                                                    Guid GuIdObjEmp = Guid.NewGuid();

                                                                    ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                    ImportLogObj.Path = curFile;
                                                                    ImportLogObj.FileName = fileName;
                                                                    ImportLogObj.Date = DateTime.Now.Date;
                                                                    Db.ImportLogs.Add(ImportLogObj);
                                                                    Db.SaveChanges();
                                                                }


                                                                AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                                AttendanceObj.Name = NameCell.ToString();
                                                                AttendanceObj.PunchTime = Convert.ToDateTime(CurrentDate + " " + ((CheckInTime + Eight) - Twelve) + " " + "PM");
                                                                AttendanceObj.WorkState = "Checkout";
                                                                AttendanceObj.Terminal = item.Terminal;
                                                                AttendanceObj.PunchType = item.PunchType;
                                                                AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                                AttendanceObj.Date = DateTime.Now.Date;
                                                                AttendanceObj.NoOfHours = "8:00:00";
                                                                AttendanceObj.RemainingHours = "00:00:00";

                                                                Db.AttendanceSheets.Add(AttendanceObj);
                                                                Db.SaveChanges();

                                                            }

                                                            break;
                                                        }

                                                    }
                                                }


                                            }

                                            else if (CurrentStatus == "Checkout")
                                            {
                                                var PreviousDateOfTheFilteredExcel = Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).ToShortDateString();

                                                if (CurrentTime.Hours <= 11 && CurrentTime.Hours >= 6 && NextDate == CurrentDate && CurrentDate != PreviousDateOfTheFilteredExcel)
                                                {

                                                    TimeSpan now = Convert.ToDateTime(PunchTimeCell).TimeOfDay;
                                                    TimeSpan Eight = new TimeSpan(8, 0, 0);
                                                    TimeSpan Twelve = new TimeSpan(12, 0, 0);

                                                    worksheet.Cells[NoOfRow, 1].Value = NumberCell;
                                                    worksheet.Cells[NoOfRow, 2].Value = NameCell;
                                                    worksheet.Cells[NoOfRow, 3].Value = PunchTimeCell;
                                                    worksheet.Cells[NoOfRow, 4].Value = "Checkinnnnnn";
                                                    worksheet.Cells[NoOfRow, 5].Value = TerminalCell;
                                                    worksheet.Cells[NoOfRow, 6].Value = PunchTypeCell;

                                                    package.Save();
                                                    NoOfRow++;

                                                    obj = new AttendanceViewModel();
                                                    obj.Number = NumberCell;
                                                    obj.Name = NameCell;
                                                    obj.PunchTime = PunchTimeCell;
                                                    obj.WorkState = "Checkinnnnnn";
                                                    list.Add(obj);


                                                     
                                                    if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                    {
                                                        Guid GuIdObjEmp = Guid.NewGuid();

                                                        ImportLogObj.Id = GuIdObjEmp.ToString();
                                                        ImportLogObj.Path = curFile;
                                                        ImportLogObj.FileName = fileName;
                                                        ImportLogObj.Date = DateTime.Now.Date;
                                                        Db.ImportLogs.Add(ImportLogObj);
                                                        Db.SaveChanges();
                                                    }


                                                    AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                    AttendanceObj.Name = NameCell.ToString();
                                                    AttendanceObj.PunchTime = Convert.ToDateTime(PunchTimeCell);
                                                    AttendanceObj.WorkState = "Checkinnnnnn";
                                                    AttendanceObj.Terminal = TerminalCell;
                                                    AttendanceObj.PunchType = PunchTypeCell;
                                                    AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                    AttendanceObj.Date = DateTime.Now.Date;
                                                    AttendanceObj.NoOfHours = null;
                                                    AttendanceObj.RemainingHours = null;

                                                    Db.AttendanceSheets.Add(AttendanceObj);
                                                    Db.SaveChanges();

                                                    var o = ListOfCheckOuts.Where(xx => (Convert.ToDateTime(xx.PunchTime)).TimeOfDay.Hours <= 11).ToList();
                                                    var oo = ListOfCheckOuts.Where(xx => (Convert.ToDateTime(xx.PunchTime)).TimeOfDay.Hours > 11).ToList();

                                                    if (o.Count != 0 && oo.Count == 0)
                                                    {
                                                        TimeSpan CheckInTime = Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).TimeOfDay;
                                                        TimeSpan CO = ((CheckInTime + Eight) - Twelve);
                                                        TimeSpan Six = new TimeSpan(6, 0, 0);
                                                        TimeSpan Eighteen = new TimeSpan(18, 0, 0);

                                                        worksheet.Cells[NoOfRow, 1].Value = NumberCell;
                                                        worksheet.Cells[NoOfRow, 2].Value = NameCell;

                                                        obj = new AttendanceViewModel();
                                                        obj.Number = NumberCell;
                                                        obj.Name = NameCell;

                                                        obj.WorkState = "Checkout";
                                                        

                                                        if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                        {
                                                            Guid GuIdObjEmp = Guid.NewGuid();

                                                            ImportLogObj.Id = GuIdObjEmp.ToString();
                                                            ImportLogObj.Path = curFile;
                                                            ImportLogObj.FileName = fileName;
                                                            ImportLogObj.Date = DateTime.Now.Date;
                                                            Db.ImportLogs.Add(ImportLogObj);
                                                            Db.SaveChanges();
                                                        }


                                                        AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                        AttendanceObj.Name = NameCell.ToString();
                                                        AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                        AttendanceObj.Date = DateTime.Now.Date;

                                                       

                                                        if (CO.Hours >= 6)
                                                        {
                                                            worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + "6:00:00" + " " + "PM";
                                                            worksheet.Cells[NoOfRow, 7].Value = (Eighteen - CheckInTime).ToString();
                                                            worksheet.Cells[NoOfRow, 8].Value = ((Eighteen - CheckInTime) - Eight).ToString();
                                                            worksheet.Cells[NoOfRow, 9].Value = "Checkout was too early and new checkout was after 6";

                                                            obj.PunchTime = CurrentDate + " " + "6:00:00" + " " + "PM";
                                                            obj.NoOfHours = (Eighteen - CheckInTime).ToString();
                                                            obj.RemainingHours = ((Eighteen - CheckInTime) - Eight).ToString();

                                                            AttendanceObj.PunchTime = Convert.ToDateTime(CurrentDate + " " + "6:00:00" + " " + "PM");
                                                            AttendanceObj.NoOfHours = (Eighteen - CheckInTime).ToString();
                                                            AttendanceObj.RemainingHours = ((Eighteen - CheckInTime) - Eight).ToString();
                                                        }
                                                        else
                                                        {
                                                            worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + CO + " " + "PM";
                                                            worksheet.Cells[NoOfRow, 7].Value = "8:00:00";
                                                            worksheet.Cells[NoOfRow, 8].Value = "00:00:00";
                                                            worksheet.Cells[NoOfRow, 9].Value = "Checkout was too early and checkinnnnnnn";

                                                            obj.PunchTime = CurrentDate + " " + CO + " " + "PM";
                                                            obj.NoOfHours = "8:00:00";
                                                            obj.RemainingHours = "00:00:00";

                                                            AttendanceObj.PunchTime = Convert.ToDateTime(CurrentDate + " " + CO + " " + "PM");
                                                            AttendanceObj.NoOfHours = "8:00:00";
                                                            AttendanceObj.RemainingHours = "00:00:00";


                                                        }

                                                        worksheet.Cells[NoOfRow, 4].Value = "Checkout";
                                                        worksheet.Cells[NoOfRow, 5].Value = TerminalCell;
                                                        worksheet.Cells[NoOfRow, 6].Value = PunchTypeCell;

                                                        package.Save();
                                                        NoOfRow++;

                                                        list.Add(obj);

                                                        AttendanceObj.WorkState = "Checkout";
                                                        AttendanceObj.Terminal = TerminalCell;
                                                        AttendanceObj.PunchType = PunchTypeCell;

                                                        Db.AttendanceSheets.Add(AttendanceObj);
                                                        Db.SaveChanges();

                                                    }

                                                }
                                                else if (NextDate != CurrentDate)
                                                {
                                                    int Count = 0;
                                                    foreach (var item in ListOfCheckOuts)
                                                    {
                                                        TimeSpan start = new TimeSpan(10, 0, 0); //10 o'clock
                                                        TimeSpan end = new TimeSpan(18, 0, 0); //18 o'clock
                                                        TimeSpan now = Convert.ToDateTime(item.PunchTime).TimeOfDay;

                                                        TimeSpan Eight = new TimeSpan(8, 0, 0);
                                                        TimeSpan Twelve = new TimeSpan(12, 0, 0);
                                                        TimeSpan Six = new TimeSpan(18, 0, 0);
                                                        TimeSpan CheckInTime;
                                                        TimeSpan NoCheckInFound = new TimeSpan(0, 0, 0);

                                                        if (CurrentDate == PreviousDateOfTheFilteredExcel)
                                                        {
                                                            CheckInTime = Convert.ToDateTime(worksheet.Cells[NoOfRow - 1, 3].Value).TimeOfDay;

                                                        }
                                                        else
                                                        {
                                                            CheckInTime = new TimeSpan(0, 0, 0);

                                                        }

                                                        if ((now.Hours >= start.Hours) && (now.Hours < end.Hours))
                                                        {
                                                            if (CheckInTime.Equals(NoCheckInFound))
                                                            {

                                                                worksheet.Cells[NoOfRow + 1, 1].Value = item.Number;
                                                                worksheet.Cells[NoOfRow + 1, 2].Value = item.Name;
                                                                worksheet.Cells[NoOfRow + 1, 3].Value = item.PunchTime;
                                                                worksheet.Cells[NoOfRow + 1, 4].Value = item.WorkState;
                                                                worksheet.Cells[NoOfRow + 1, 5].Value = item.Terminal;
                                                                worksheet.Cells[NoOfRow + 1, 6].Value = item.PunchType;
                                                                worksheet.Cells[NoOfRow + 1, 7].Value = "8:00:00";
                                                                worksheet.Cells[NoOfRow + 1, 8].Value = "00:00:00";
                                                                worksheet.Cells[NoOfRow + 1, 9].Value = "No Checkin Found (generated)";
                                                                package.Save();


                                                                TimeSpan CheckoutTime = Convert.ToDateTime(worksheet.Cells[NoOfRow + 1, 3].Value).TimeOfDay;

                                                                
                                                                worksheet.Cells[NoOfRow, 1].Value = NumberCell;
                                                                worksheet.Cells[NoOfRow, 2].Value = NameCell;
                                                                worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + (CheckoutTime - Eight) + " " + "AM";
                                                                worksheet.Cells[NoOfRow, 4].Value = "Checkin";
                                                                worksheet.Cells[NoOfRow, 5].Value = TerminalCell;
                                                                worksheet.Cells[NoOfRow, 6].Value = PunchTypeCell;


                                                                package.Save();
                                                                NoOfRow = NoOfRow + 2;


                                                                obj = new AttendanceViewModel();
                                                                obj.Number = NumberCell;
                                                                obj.Name = NameCell;
                                                                obj.PunchTime = CurrentDate + " " + (CheckoutTime - Eight) + " " + "AM";
                                                                obj.WorkState = "Checkin";
                                                                list.Add(obj);

                                                                 
                                                                if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                                {
                                                                    Guid GuIdObjEmp = Guid.NewGuid();

                                                                    ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                    ImportLogObj.Path = curFile;
                                                                    ImportLogObj.FileName = fileName;
                                                                    ImportLogObj.Date = DateTime.Now.Date;
                                                                    Db.ImportLogs.Add(ImportLogObj);
                                                                    Db.SaveChanges();
                                                                }


                                                                AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                                AttendanceObj.Name = NameCell.ToString();
                                                                AttendanceObj.PunchTime = Convert.ToDateTime(CurrentDate + " " + (CheckoutTime - Eight) + " " + "AM");
                                                                AttendanceObj.WorkState = "Checkin";
                                                                AttendanceObj.Terminal = TerminalCell;
                                                                AttendanceObj.PunchType = PunchTypeCell;
                                                                AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                                AttendanceObj.Date = DateTime.Now.Date;
                                                                AttendanceObj.NoOfHours = null;
                                                                AttendanceObj.RemainingHours = null;

                                                                Db.AttendanceSheets.Add(AttendanceObj);
                                                                Db.SaveChanges();

                                                                obj = new AttendanceViewModel();
                                                                obj.Number = NumberCell;
                                                                obj.Name = NameCell;
                                                                obj.PunchTime = item.PunchTime;
                                                                obj.WorkState = item.WorkState;
                                                                obj.NoOfHours = "8:00:00";
                                                                obj.RemainingHours = "00:00:00";
                                                                list.Add(obj);


                                                               if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                                {
                                                                    Guid GuIdObjEmp = Guid.NewGuid();

                                                                    ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                    ImportLogObj.Path = curFile;
                                                                    ImportLogObj.FileName = fileName;
                                                                    ImportLogObj.Date = DateTime.Now.Date;
                                                                    Db.ImportLogs.Add(ImportLogObj);
                                                                    Db.SaveChanges();
                                                                }


                                                                AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                                AttendanceObj.Name = NameCell.ToString();
                                                                AttendanceObj.PunchTime = Convert.ToDateTime(item.PunchTime);
                                                                AttendanceObj.WorkState = item.WorkState;
                                                                AttendanceObj.Terminal = item.Terminal;
                                                                AttendanceObj.PunchType = item.PunchType;
                                                                AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                                AttendanceObj.Date = DateTime.Now.Date;
                                                                AttendanceObj.NoOfHours = "8:00:00";
                                                                AttendanceObj.RemainingHours = "00:00:00";

                                                                Db.AttendanceSheets.Add(AttendanceObj);
                                                                Db.SaveChanges();

                                                                break;

                                                            }
                                                            else
                                                            {
                                                                worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                                worksheet.Cells[NoOfRow, 2].Value = item.Name;
                                                                worksheet.Cells[NoOfRow, 3].Value = item.PunchTime;
                                                                worksheet.Cells[NoOfRow, 4].Value = item.WorkState;
                                                                worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                                worksheet.Cells[NoOfRow, 6].Value = item.PunchType;
                                                                worksheet.Cells[NoOfRow, 7].Value = (now - CheckInTime).ToString();
                                                                worksheet.Cells[NoOfRow, 8].Value = ((now - CheckInTime) - Eight).ToString();

                                                                package.Save();
                                                                NoOfRow++;

                                                                obj = new AttendanceViewModel();
                                                                obj.Number = NumberCell;
                                                                obj.Name = NameCell;
                                                                obj.PunchTime = item.PunchTime;
                                                                obj.WorkState = item.WorkState;
                                                                obj.NoOfHours = (now - CheckInTime).ToString();
                                                                obj.RemainingHours = ((now - CheckInTime) - Eight).ToString();
                                                                list.Add(obj);

                                                                 
                                                                if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                                {
                                                                    Guid GuIdObjEmp = Guid.NewGuid();

                                                                    ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                    ImportLogObj.Path = curFile;
                                                                    ImportLogObj.FileName = fileName;
                                                                    ImportLogObj.Date = DateTime.Now.Date;
                                                                    Db.ImportLogs.Add(ImportLogObj);
                                                                    Db.SaveChanges();
                                                                }


                                                                AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                                AttendanceObj.Name = NameCell.ToString();
                                                                AttendanceObj.PunchTime = Convert.ToDateTime(item.PunchTime);
                                                                AttendanceObj.WorkState = item.WorkState;
                                                                AttendanceObj.Terminal = item.Terminal;
                                                                AttendanceObj.PunchType = item.PunchType;
                                                                AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                                AttendanceObj.Date = DateTime.Now.Date;
                                                                AttendanceObj.NoOfHours = (now - CheckInTime).ToString();
                                                                AttendanceObj.RemainingHours = ((now - CheckInTime) - Eight).ToString();

                                                                Db.AttendanceSheets.Add(AttendanceObj);
                                                                Db.SaveChanges();

                                                                break;
                                                            }



                                                        }
                                                        else
                                                        {
                                                            Count++;
                                                            var o = ListOfCheckOuts.Where(xx => (Convert.ToDateTime(xx.PunchTime)).TimeOfDay.Hours <= 11).ToList();

                                                            if (Count == ListOfCheckOuts.Count())
                                                            {

                                                                if (ListOfCheckins.Count == 0)
                                                                {
                                                                    worksheet.Cells[NoOfRow + 1, 1].Value = item.Number;
                                                                    worksheet.Cells[NoOfRow + 1, 2].Value = item.Name;
                                                                    worksheet.Cells[NoOfRow + 1, 3].Value = CurrentDate + " " + "6:00:00 PM";
                                                                    worksheet.Cells[NoOfRow + 1, 4].Value = item.WorkState;
                                                                    worksheet.Cells[NoOfRow + 1, 5].Value = item.Terminal;
                                                                    worksheet.Cells[NoOfRow + 1, 6].Value = item.PunchType;
                                                                    worksheet.Cells[NoOfRow + 1, 7].Value = "8:00:00";
                                                                    worksheet.Cells[NoOfRow + 1, 8].Value = "00:00:00";
                                                                    worksheet.Cells[NoOfRow + 1, 9].Value = "Checkout was after 6 and no checkin found";
                                                                    package.Save();

                                                                   


                                                                    TimeSpan CheckoutTime = new TimeSpan(18, 0, 0);


                                                                    worksheet.Cells[NoOfRow, 1].Value = NumberCell;
                                                                    worksheet.Cells[NoOfRow, 2].Value = NameCell;
                                                                    worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + (CheckoutTime - Eight) + " " + "AM";
                                                                    worksheet.Cells[NoOfRow, 4].Value = "Checkin";
                                                                    worksheet.Cells[NoOfRow, 5].Value = TerminalCell;
                                                                    worksheet.Cells[NoOfRow, 6].Value = PunchTypeCell;

                                                                    package.Save();
                                                                    NoOfRow = NoOfRow + 2;

                                                                    obj = new AttendanceViewModel();
                                                                    obj.Number = NumberCell;
                                                                    obj.Name = NameCell;
                                                                    obj.PunchTime = CurrentDate + " " + (CheckoutTime - Eight) + " " + "AM";
                                                                    obj.WorkState = "Checkin";
                                                                    list.Add(obj);
                                                                     
                                                                    if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                                    {
                                                                        Guid GuIdObjEmp = Guid.NewGuid();

                                                                        ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                        ImportLogObj.Path = curFile;
                                                                        ImportLogObj.FileName = fileName;
                                                                        ImportLogObj.Date = DateTime.Now.Date;
                                                                        Db.ImportLogs.Add(ImportLogObj);
                                                                        Db.SaveChanges();
                                                                    }


                                                                    AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                                    AttendanceObj.Name = NameCell.ToString();
                                                                    AttendanceObj.PunchTime = Convert.ToDateTime( CurrentDate + " " + (CheckoutTime - Eight) + " " + "AM");
                                                                    AttendanceObj.WorkState = "Checkin";
                                                                    AttendanceObj.Terminal = item.Terminal;
                                                                    AttendanceObj.PunchType = item.PunchType;
                                                                    AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                                    AttendanceObj.Date = DateTime.Now.Date;
                                                                    AttendanceObj.NoOfHours = null;
                                                                    AttendanceObj.RemainingHours = null;

                                                                    Db.AttendanceSheets.Add(AttendanceObj);
                                                                    Db.SaveChanges();

                                                                    obj = new AttendanceViewModel();
                                                                    obj.Number = NumberCell;
                                                                    obj.Name = NameCell;
                                                                    obj.PunchTime = CurrentDate + " " + "6:00:00 PM";
                                                                    obj.WorkState = item.WorkState;
                                                                    obj.NoOfHours = "8:00:00";
                                                                    obj.RemainingHours = "00:00:00";
                                                                    list.Add(obj);

                                                                    if (!(Db.ImportLogs.Where(r => r.Id == AttendanceObj.ImportLogNo).Any()))
                                                                    {
                                                                        Guid GuIdObjEmp = Guid.NewGuid();

                                                                        ImportLogObj.Id = GuIdObjEmp.ToString();
                                                                        ImportLogObj.Path = curFile;
                                                                        ImportLogObj.FileName = fileName;
                                                                        ImportLogObj.Date = DateTime.Now.Date;
                                                                        Db.ImportLogs.Add(ImportLogObj);
                                                                        Db.SaveChanges();
                                                                    }


                                                                    AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                                    AttendanceObj.Name = NameCell.ToString();
                                                                    AttendanceObj.PunchTime = Convert.ToDateTime(CurrentDate + " " + "6:00:00 PM");
                                                                    AttendanceObj.WorkState = item.WorkState;
                                                                    AttendanceObj.Terminal = item.Terminal;
                                                                    AttendanceObj.PunchType = item.PunchType;
                                                                    AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                                    AttendanceObj.Date = DateTime.Now.Date;
                                                                    AttendanceObj.NoOfHours = "8:00:00";
                                                                    AttendanceObj.RemainingHours = "00:00:00";

                                                                    Db.AttendanceSheets.Add(AttendanceObj);
                                                                    Db.SaveChanges();

                                                                    break;

                                                                }
                                                            
                                                                else
                                                                {
                                                                    worksheet.Cells[NoOfRow, 1].Value = item.Number;
                                                                    worksheet.Cells[NoOfRow, 2].Value = item.Name;
                                                                    worksheet.Cells[NoOfRow, 3].Value = CurrentDate + " " + "6:00:00 PM";
                                                                    worksheet.Cells[NoOfRow, 4].Value = item.WorkState;
                                                                    worksheet.Cells[NoOfRow, 5].Value = item.Terminal;
                                                                    worksheet.Cells[NoOfRow, 6].Value = item.PunchType;
                                                                    worksheet.Cells[NoOfRow, 7].Value = (Six - CheckInTime).ToString();
                                                                    worksheet.Cells[NoOfRow, 8].Value = ((Six - CheckInTime) - Eight).ToString();
                                                                    worksheet.Cells[NoOfRow, 9].Value = "Checkout was after 6";
                                                                    package.Save();
                                                                    NoOfRow++;

                                                                    obj = new AttendanceViewModel();
                                                                    obj.Number = NumberCell;
                                                                    obj.Name = NameCell;
                                                                    obj.PunchTime = CurrentDate + " " + "6:00:00 PM";
                                                                    obj.WorkState = item.WorkState;
                                                                    obj.NoOfHours = (Six - CheckInTime).ToString();
                                                                    obj.RemainingHours = ((Six - CheckInTime) - Eight).ToString();
                                                                    list.Add(obj);


                                                                    AttendanceObj.Number = Convert.ToInt32(NumberCell);
                                                                    AttendanceObj.Name = NameCell.ToString();
                                                                    AttendanceObj.PunchTime = Convert.ToDateTime(CurrentDate + " " + "6:00:00 PM");
                                                                    AttendanceObj.WorkState = item.WorkState;
                                                                    AttendanceObj.Terminal = item.Terminal;
                                                                    AttendanceObj.PunchType = item.PunchType;
                                                                    AttendanceObj.ImportLogNo = ImportLogObj.Id;
                                                                    AttendanceObj.Date = DateTime.Now.Date;
                                                                    AttendanceObj.NoOfHours = (Six - CheckInTime).ToString();
                                                                    AttendanceObj.RemainingHours = ((Six - CheckInTime) - Eight).ToString();

                                                                    Db.AttendanceSheets.Add(AttendanceObj);
                                                                    Db.SaveChanges();

                                                                    break;
                                                                }


                                                            }
                                                        }
                                                    }
                                                }

                                            }

                                            else
                                            {

                                            }


                                        }

                                    }
                                }
                                
                                xlWorkbook.Close();
                                package.Dispose();
                               
                                ViewBag.Message = string.Format(Resources.NubiHR.DoneFiltering, "Index", "Import");
                                
                                return View(list);


                            }
                        }


                    }

                    catch (Exception ex)
                    {
                        xlWorkbook.Close();
                        ViewBag.MessageError = string.Format(ex.Message, "Index", "Import");
                        return View();
                    }
                }
            }
            return View();

        }

        //GET: /Details/
        [HttpGet]
        public ActionResult Details()
        {
            var current = System.Globalization.CultureInfo.CurrentCulture;
            current.DateTimeFormat.Calendar = new GregorianCalendar();
            return View(Db.AttendanceSheets.ToList());
         
        }


    }
}