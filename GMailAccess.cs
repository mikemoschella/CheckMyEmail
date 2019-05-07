using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MimeKit;
using CheckMyEmail.Models;

namespace CheckMyEmail.Objects
{
    // Class to Access Gmail Account

    public class GMailAccess
    {

        private const string INBOXFOLDER = "1";
        private const string HOST = "imap.gmail.com";
        private const int PORT = 993;
        private const bool USESSL = true;

        private static EmailSetting _emailSetting = new EmailSetting();



        public GMailAccess(string Host, int Port, bool UseSSL, string Username, string Password)
        {
            _emailSetting.Host = Host;
            _emailSetting.Port = Port;
            _emailSetting.UseSSL = UseSSL;
            _emailSetting.Username = Username;
            _emailSetting.Password = Password;

        }

        public GMailAccess(string Username, string Password)
        {
            _emailSetting.Host = HOST;
            _emailSetting.Port = PORT;
            _emailSetting.UseSSL = USESSL;
            _emailSetting.Username = Username;
            _emailSetting.Password = Password;


        }

        public EmailSetting getEailMailSettings()
        {
            return _emailSetting;

        }

        public List<EmailMessage> RetrieveFolderMessages(string messageFolder = INBOXFOLDER)
        {
            List<EmailMessage> _emails;

            string _folderName = GetFolder(messageFolder);
        
            EmailAccount _emailAccount = new EmailAccount(_emailSetting);

            _emails = _emailAccount.RetrieveEmails(_folderName);

            return _emails;
        }

        public string GetFolder(string FolderID)
        {
            string Folder = "";

            switch (FolderID)
            {
                case "1":
                    Folder = "INBOX";
                    break;
                case "2":
                    Folder = "[Gmail]/Sent Mail";
                    break;
                case "3":
                    Folder = "[Gmail]/Drafts";
                    break;
                case "4":
                    Folder = "[Gmail]/Spam";
                    break;
                case "5":
                    Folder = "[Gmail]/Trash";
                    break;
                default:
                    Folder = "INBOX";
                    break;
            }
            return Folder;
        }

    }

    public class EmailMessage
    {
        public EmailMessage(List<EmailAddress> ToAddresses, List<EmailAddress> FromAddresses, string Subject, string Body, DateTime DateSent)
        {
            _ToAddresses = ToAddresses;
            _FromAddresses = FromAddresses;
            _Subject = Subject;
            _Body = Body;
            _DateSent = DateSent;
        }
        private readonly List<EmailAddress> _ToAddresses;
        private readonly List<EmailAddress> _FromAddresses;
        private readonly string _Subject;
        private readonly string _Body;
        private readonly DateTime _DateSent;

        public List<EmailAddress> GetToAddresses()
        {
            return _ToAddresses;
        }

        public List<EmailAddress> GetFromAddresses()
        {
            return _FromAddresses;
        }

        public string GetSubject()
        {
            return _Subject;
        }
        public string GetBody()
        {
            return _Body;
        }
        public DateTime GetDateSent()
        {
            return _DateSent;
        }

    }
    public interface IEmailSetting
    {

        string Host { get; }
        int Port { get; }
        bool UseSSL { get; }
        string Username { get; }
        string Password { get; }
    }



    public class EmailSetting : IEmailSetting
    {

        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class EmailAccount
    {
        private IEmailSetting _emailSetting;

        public EmailAccount(IEmailSetting emailSetting)
        {
            _emailSetting = emailSetting;
        }

        public List<EmailMessage> RetrieveEmails(string messageFolder)
        {
            List<EmailMessage> messages = new List<EmailMessage>();

            using (var client = new ImapClient())
            {
                
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(_emailSetting.Host, _emailSetting.Port, _emailSetting.UseSSL);

                    client.Authenticate(_emailSetting.Username, _emailSetting.Password);

                    var emailBox = client.GetFolder(messageFolder);
                    emailBox.Open(FolderAccess.ReadOnly);

                    for (int i = 0; i < emailBox.Count; i++)
                    {
                        var message = emailBox.GetMessage(i);
                        string messageBody;


                        if (!string.IsNullOrEmpty(message.HtmlBody))
                            messageBody = message.HtmlBody;
                        else
                            messageBody = message.HtmlBody;

                        List<EmailAddress> toAddresses = new List<EmailAddress>();

                        for (int t = 0; t < message.To.Count; t++)
                        {
                            string AddressName;
                            if (String.IsNullOrEmpty(message.To[t].Name))
                                AddressName = message.To[t].ToString();
                            else
                                AddressName = message.To[t].Name;

                            toAddresses.Add(new EmailAddress() { Name = AddressName, Address = message.To[t].ToString(), Location = t });
                        }

                        List<EmailAddress> fromAddresses = new List<EmailAddress>();

                        for (int f = 0; f < message.From.Count; f++)
                        {
                            string AddressName;
                            if (String.IsNullOrEmpty(message.From[f].Name))
                                AddressName = message.To[f].ToString();
                            else
                                AddressName = message.To[f].Name;

                            fromAddresses.Add(new EmailAddress() { Name = AddressName, Address = message.To[f].ToString(), Location = f });
                        }

                        EmailMessage emailMessage = new EmailMessage(toAddresses, fromAddresses, message.Subject, message.From[0].Name, message.Date.DateTime);


                        messages.Add(emailMessage);

                    }



                    client.Disconnect(true);

                return messages;

            }
        }


    }
}