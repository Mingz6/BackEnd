using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace searchDBandCreateTXT
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {

            logger.Info("job start.");
            string dbconnectionstring = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dbFile = filePath + @"\Your_Files"+ DateTime.Now.ToString("yyyyMMdd") + ".txt";

            writeFileFromDB(dbconnectionstring, dbFile);

            sendEmail(dbFile);

            logger.Info("job completed.");
        }

        public static void writeFileFromDB(string dbConn, string dbFile)
        {
            SqlCommand comm = new SqlCommand();
            comm.Connection = new SqlConnection(dbConn);
            string sqlQuery = ConfigurationManager.AppSettings["sqlQuery"];
            comm.CommandText = sqlQuery;

            try
            {
                comm.Connection.Open();

                SqlDataReader sqlReader = comm.ExecuteReader();

                using (StreamWriter file = new StreamWriter(dbFile, false))
                {
                    while (sqlReader.Read())
                    {
                        file.WriteLine(sqlReader["columName"]);
                    }
                }
                sqlReader.Close();
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            
            comm.Connection.Close();
        }

        private static void sendEmail(string dbFile)
        {
            try
            {
                Outlook.Application oApp = new Outlook.Application();
                Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
                
                oMsg.HTMLBody = ConfigurationManager.AppSettings["Body"]; ;
                String sDisplayName = "MyAttachment";
                int iPosition = (int)oMsg.Body.Length + 1;
                int iAttachType = (int)Outlook.OlAttachmentType.olByValue;

                Outlook.Attachment oAttach = oMsg.Attachments.Add
                    (@dbFile, iAttachType, iPosition, sDisplayName);
                //Subject line
                string subjectLine = ConfigurationManager.AppSettings["Subject"] + DateTime.Now.ToString("yyyyMMdd");
                oMsg.Subject = subjectLine;

                // Add a recipient.
                Outlook.Recipients oRecips = (Outlook.Recipients)oMsg.Recipients;
                
                List<string> sTORecipsList = new List<string>();

                sTORecipsList.Add(ConfigurationManager.AppSettings["recipient1"]);
                sTORecipsList.Add(ConfigurationManager.AppSettings["recipient2"]);
                sTORecipsList.Add(ConfigurationManager.AppSettings["recipient3"]);
                sTORecipsList.Add(ConfigurationManager.AppSettings["recipient4"]);
                sTORecipsList.Add(ConfigurationManager.AppSettings["recipient5"]);
                sTORecipsList.Add(ConfigurationManager.AppSettings["recipient6"]);

                foreach (string t in sTORecipsList)
                {
                    Outlook.Recipient oTORecip = oRecips.Add(t);
                    oTORecip.Type = (int)Outlook.OlMailRecipientType.olTo;
                    oTORecip.Resolve();
                }
                
                oMsg.Send();
                oRecips = null;
                oMsg = null;
                oApp = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
