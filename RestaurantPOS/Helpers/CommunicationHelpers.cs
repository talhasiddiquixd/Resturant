using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Nancy.Json;
using Newtonsoft.Json;
using RestaurantPOS.Helpers.RequestDTO;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace RestaurantPOS.Helpers
{
    public class CommunicationHelpers
    {
        /// <summary>
        /// Function for Sending Email generic emails
        /// </summary>
        /// <param name="emailRequestModel"></param>
        /// <param name="configuration"></param>
        /// <returns>True if emails send successfully</returns>
        public static bool SendEmail(EmailRequestModel emailRequestModel, IConfiguration configuration)
        {
            try
            {
                string projectName = configuration["Utility:ProjectName"];
                string emailFromName = configuration["EmailConfig:EmailFromName"];
                string emailFromEmail = configuration["EmailConfig:EmailFromEmail"];
                string emailFromPassword = configuration["EmailConfig:EmailFromPassword"];
                int port = Convert.ToInt32(configuration["EmailConfig:EmailPort"]);
                bool EmailEnableSSL = Convert.ToBoolean(configuration["EmailConfig:EmailEnableSSL"]);
                string SMTP = configuration["EmailConfig:EmailSMTP"];
                string emailText = emailRequestModel.EmailTemplate;
                string emailSubject = emailRequestModel.EmailSubject;
                string emailToName = emailRequestModel.EmailToEmail;
                string emailToEmail = emailRequestModel.EmailToEmail;       // email;
                MimeMessage message = new MimeMessage();
                var bodyBuilder = new BodyBuilder();
                MailboxAddress from = new MailboxAddress(emailFromEmail, emailFromEmail);
                message.From.Add(from);
                MailboxAddress to = null;
                to = new MailboxAddress(emailToName, emailToEmail);
                message.To.Add(to);
                message.Subject = emailSubject;
                bodyBuilder.HtmlBody = emailText;
                message.Body = bodyBuilder.ToMessageBody();
                SmtpClient client = new SmtpClient();
                client.Connect(SMTP, port, EmailEnableSSL);
                client.Authenticate(emailFromEmail, emailFromPassword);
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception )
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generic function for sending Push Notifications
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="_title"></param>
        /// <param name="_data"></param>
        /// <param name="messageText"></param>
        /// <param name="os"></param>
        /// <param name="type"></param>
        /// <returns>Push Notifications Response string</returns>


        //public static async Task<object> SendNotificationToDevice(SendNotificationRequestDTO notification)
        //{
        //    string webAPIKey = "AIzaSyANWZCRQ5dCPQ8YXSz3C0u5IWSZVK5E7pY";

        //    //string serverKey = "AAAAvRCFj1M:APA91bFawWn6bgmPhAgB8bW7UNuOM5-wv12s6HSWT-5ncSL4HyrjeeEOL9x-AMdQes9BAW3tCtXZ1Xh4u7RoWVlWRiyoNLZuW1sekiGb40jMFnue6I_2iraN2huUJt08Za5rY_Hi63vh";

        //    string serverKey = "AAAAgnLutRU:APA91bGBFDeu0mkeJLmOLsVZmELZNFebL5GOYrbCjylCwOIWRivv3qSnOSMwovXkX8SPbFxnBOJRCzmlhCftAnKiDvfjfmcF_2tfTpu1ApckL0EOsaAHuAvuhheHaq-Q727zM6ySbpgw";
        //    string senderId = "560273995029";

        //    string webAddress = "https://fcm.googleapis.com/fcm/send";
        //    try
        //    {
        //        // These registration tokens come from the client FCM SDKs.
        //        #region Proviouse Code for Sending Notification to Devices
        //        var result = "-1";
        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
        //        httpWebRequest.ContentType = "application/json";
        //        httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
        //        httpWebRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
        //        httpWebRequest.Method = "POST";

        //        var payload = new
        //        {
        //            //to = deviceToken,
        //            registration_ids = notification.DeviceToken,
        //            notification = new
        //            {
        //                title = notification.Title,
        //                body = notification.MessageText,
        //                sound = "default",
        //                badge = 0,
        //                data = notification.Data
        //            },
        //            data = new
        //            {
        //                body = notification.Data,
        //                title = notification.Title,
        //                type = notification.OsType,
        //            }
        //        };

        //        var serializer = new JavaScriptSerializer();
        //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //        {
        //            string json = JsonConvert.SerializeObject(payload);
        //            streamWriter.Write(json);
        //            streamWriter.Flush();
        //        }

        //        var httpResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            result = streamReader.ReadToEnd();
        //        }

        //        return result;
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        var mes = ex;
        //    }

        //    return false;
        //}

        //public static async Task<object> SendNotificationToDevice(SendNotificationRequestDTO notification)
        public async Task<object> SendNotificationToDevice(string[] deviceToken, string _title, object _data, string messageText, string os, string type = "")
        {
            //string webAPIKey = "AIzaSyBuYLcWNIAjvDYKNaZxOncQs5TapB-OjUA";
            string serverKey = "AAAAgnLutRU:APA91bGBFDeu0mkeJLmOLsVZmELZNFebL5GOYrbCjylCwOIWRivv3qSnOSMwovXkX8SPbFxnBOJRCzmlhCftAnKiDvfjfmcF_2tfTpu1ApckL0EOsaAHuAvuhheHaq-Q727zM6ySbpgw";
            string senderId = "560273995029";
            string webAddress = "https://fcm.googleapis.com/fcm/send";
            try
            {
                // These registration tokens come from the client FCM SDKs.
                #region Proviouse Code for Sending Notification to Devices
                var result = "-1";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                httpWebRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                httpWebRequest.Method = "POST";

                var payload = new
                {
                    //to = deviceToken,
                    registration_ids = deviceToken,
                    notification = new
                    {
                        title = _title,
                        body = messageText,
                        sound = "default",
                        badge = 0,
                        data = _data
                    },
                    data = new
                    {
                        body = _data,
                        type = type,
                        title = _title,
                    }
                };

                var serializer = new JavaScriptSerializer();
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(payload);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                return result;
                #endregion
            }
            catch (Exception ex)
            {
                var mes = ex;
            }
            return false;
        }


        /// <summary>
        /// Function will use to send sms
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns>Return sms sent response True/False</returns>
        //public static bool SendSMS(string phoneNumber, string message)
        //{
        //    const string accountSid = "AC8818ac37c117c86d6bd687a9123e772c";
        //    const string authToken = "aa20193ee3ec938b3e9eaff965eb5572";

        //    TwilioClient.Init(accountSid, authToken);

        //    var responseMessage = MessageResource.Create(
        //        body: message,
        //        from: new Twilio.Types.PhoneNumber("+14049984471"),
        //        to: new Twilio.Types.PhoneNumber(phoneNumber)
        //    );
        //    return true;
        //}
    }
}
