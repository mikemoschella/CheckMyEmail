using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CheckMyEmail.Models;
using CheckMyEmail.Objects;
using System.Data;
using MySql.Data.MySqlClient;

namespace CheckMyEmail.Controllers
{

    

    public class LoginController : Controller
    {
        private const string EMAILPROFILESELECT = "select * from mikesdata.emailprofile where EmailUserName = '";
        private const string EMAILPROFILEINSERT = @"INSERT INTO mikesdata.emailprofile ( EmailHost, EmailSSL, EmailUserName, EmailPort) VALUES ( ?EmailHost, ?EmailSSL, ?EmailUserName, ?EmailPort)";

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult ResultsPage(string Message = "")
        {
            ViewBag.Message = Message;

            return View();
        }

        public  string CheckMyEmail(string Email, string Password, string Folder)
        {
            string _message;
            string _mySql;
            bool _emailNotFound = false;
            List<EmailMessage> _Messages;
            List<EmailAddress> _fromAddresses;
            List<EmailAddress> _toAddresses;
            GMailAccess gMailAccess;

            _mySql = EMAILPROFILESELECT + Email + "'";

            try
            {


                MySqlAccess _mySqlAccess = new MySqlAccess();

                DataSet ds = _mySqlAccess.MySql_DataAdapter(_mySql);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string EmailHost;
                    int EmailPort;
                    bool EmailSSL;

                    if (ds.Tables[0].Rows.Count > 1)
                    {
                        _message = "Duplicate EmailAddress records found in the database!";
                        return _message;
                    }

                    DataRow dr = ds.Tables[0].Rows[0];


                    EmailHost = Convert.ToString(dr["EmailHost"]);
                    EmailPort = Convert.ToInt32(dr["EmailPort"]);
                    EmailSSL = Convert.ToBoolean(dr["EmailSSL"]);

                    gMailAccess = new GMailAccess(EmailHost, EmailPort, EmailSSL, Email, Password);
                }
                else
                {
                    _emailNotFound = true;
                    gMailAccess = new GMailAccess(Email, Password);
                }

                _Messages = gMailAccess.RetrieveFolderMessages(Folder);

                foreach (EmailMessage message in _Messages)
                {
                    _toAddresses = message.GetToAddresses();
                    _fromAddresses = message.GetFromAddresses();

                    foreach (EmailAddress afromAddresss in _fromAddresses)
                    {
                        
                    }

                    foreach (EmailAddress atoAddresss in _toAddresses)
                    {
                       

                    }

                }

                if(_emailNotFound)
                {
                    EmailSetting _emailSetting = gMailAccess.getEailMailSettings();

                    List<Parameter> _sqlParameters = new List<Parameter>();

                    _mySql = EMAILPROFILEINSERT;

                    _sqlParameters.Add(new Parameter() { DataType = MySqlDbType.VarString, ParameterName = "?EmailHost", Value = _emailSetting.Host });
                    _sqlParameters.Add(new Parameter() { DataType = MySqlDbType.Bit, ParameterName = "?EmailSSL", Value = _emailSetting.UseSSL.ToString() });
                    _sqlParameters.Add(new Parameter() { DataType = MySqlDbType.VarString, ParameterName = "?EmailUserName", Value = _emailSetting.Username });
                    _sqlParameters.Add(new Parameter() { DataType = MySqlDbType.Int32, ParameterName = "?EmailPort", Value = _emailSetting.Port.ToString() });

                    _mySqlAccess.MySql_CommandCall(CommandType.Text, _mySql, _sqlParameters);
                }

                _message = "In folder " + gMailAccess.GetFolder(Folder) + " " + _Messages.Count().ToString() + " messages were in " + Email+"!";

            }
            catch (Exception e)
            {
                _message = e.Message;
            }
            finally
            {

            }
            return _message;

        }

        public string CheckLogin(string Email, string Password, string Folder)
        {
            string _message = CheckMyEmail(Email, Password, Folder);


            return (_message);
        }
    }
}