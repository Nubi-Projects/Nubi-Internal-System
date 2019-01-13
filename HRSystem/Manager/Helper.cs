using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRSystem.Models;
using OfficeOpenXml;
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;

namespace HRSystem.Manager
{
    public class Helper
    {
        NubiDBEntities db = new NubiDBEntities();
        public static bool SendEmail(int id)
        {
            var err = "";


            return SendEmail(id, out err);
        }

        // send Email
        public static bool SendEmail(int id, out string err)
        {
            err = null;
            if (ConfigurationManager.AppSettings["EnableNotifications"] == null || ConfigurationManager.AppSettings["EnableNotifications"].ToLower().Trim() != "true") return true;
            using (NubiDBEntities db = new NubiDBEntities())
            {
                Alert alert = db.Alerts.Find(id);
                if (alert == null)
                {
                    return false;
                }
                /*
                var IsSendEmail = FunSendEmail(alert.MsgEmailText, alert.Email, alert.EmailSubject, true, alert.AttachmentPath, out err);
                if (IsSendEmail == true)
                {
                    alert.IsSendEmail = true;
                    alert.DateSendEmail = DateTime.Now;
                    db.Entry(alert).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                } */
                return true;
            }
            //  return 0;
        }
        public static bool FunSendEmail(string Body, string To, string Subject, bool isBodyHtml = true, string AttachmentPath = null)
        {
            var err = "";

            return FunSendEmail(Body, To, Subject, isBodyHtml, AttachmentPath, out err);

        }

        public static bool FunSendEmail(string Body, string To, string Subject, bool isBodyHtml, string AttachmentPath, out string err)
        {
            err = null;
            if (ConfigurationManager.AppSettings["EnableNotifications"] == null || ConfigurationManager.AppSettings["EnableNotifications"].ToLower().Trim() != "true") return true;

            var toArr = new List<string>();
            var DesignText = "<Div style=\"unicode-bidi:plaintext\">";
            DesignText += Body;
            DesignText += "</div>";

            try
            {
                var MailMessage = new System.Net.Mail.MailMessage();
                MailMessage.To.Add(To);
                if (IsLocalhost)
                {
                    MailMessage.To.Clear();
                    MailMessage.CC.Clear();
                    MailMessage.Bcc.Clear();
                    MailMessage.Subject = "FAKE EMAIL -  LOCALHOST -" + Subject;
                    MailMessage.Bcc.Add(new MailAddress("razaz@nubi-ict.com", "Razaz Ayoub"));
                    MailMessage.Bcc.Add(new MailAddress("eithar@nubi-ict.com", "Eithar Abdullah"));
                }
                else
                {
                    MailMessage.Subject = Subject;
                }
                MailMessage.From = new System.Net.Mail.MailAddress("test@nubi-ict.com", "NUBI HR-System");
                
                MailMessage.Body = DesignText;
                MailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                MailMessage.IsBodyHtml = isBodyHtml;
                if (AttachmentPath != null)
                {
                    MailMessage.Attachments.Add(new System.Net.Mail.Attachment(AttachmentPath));
                }

                var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                var useSSL = false;
                var port = 587;

                if (ConfigurationManager.AppSettings["smtp_port"] != null)
                {
                    int.TryParse(ConfigurationManager.AppSettings["smtp_port"], out port);
                }

                if (ConfigurationManager.AppSettings["smtp_ssl"] != null && ConfigurationManager.AppSettings["smtp_ssl"].ToLower().Trim() == "false")
                {
                    useSSL = false;
                }

                var x = new SmtpClient
                {
                    Host = "mail.nubi-ict.com",
                    Port = port,
                    EnableSsl = useSSL,
                    //Timeout = 1000000000000,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential("test@nubi-ict.com", "1213Nubi")
                };
                x.Send(MailMessage);
                return true;

            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }
        public static bool IsLocalhost
        {
            get
            {
                if (ConfigurationManager.AppSettings["IsLocalhost"] == null || ConfigurationManager.AppSettings["IsLocalhost"].ToLower().Trim() != "true")
                    return false;
                else
                    return true;
            }
        }
        public AspNetUser GetName(string id)
        {
            return db.AspNetUsers.Where(a => a.Id == id).FirstOrDefault();
        }

        public string GetUserRole(string userId)
        {
            var user = db.AspNetUsers.FirstOrDefault(a => a.Id == userId);
            if (user != null)
            {
                var emp = user.Employee;
                if (emp != null)
                {
                    var position = emp.Position;
                    if (position != null)
                    {
                        var positionName = position.PositionNameAr;
                        return positionName;
                    }
                }
            }

            return "This Employee Does Not Have A Position";
        }

        public static List<ExcelWorksheet> GetWorksheets(ExcelPackage package)
        {
            var attempts = 0;
            while (attempts < 3)
            {
                try
                {
                    return package.Workbook.Worksheets.Select(s => s).ToList();
                }
                catch (Exception ex)
                {
                    
                }

                attempts++;
            }

            return new List<ExcelWorksheet>();
        }
    }
}