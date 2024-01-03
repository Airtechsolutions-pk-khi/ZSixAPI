using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace ZSixRestaurantAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>  
        [HttpGet]
        [Route("api/push/android")]
        public void PushNotification()
        {
            try
            {
                //var applicationID = "AAAArE69nn0:APA91bFqI80DpPrdb1s0lT8tf4xUfoigYvGVXQOlBAIq7tCB3224CjqOTyvxiP7L_eKN4uoRWsTVw0661yX4CooMBPgIsddZSMxpypJCKg6l5Q7xgHTGYDlqeVV-HTCn9ud94a5X5a2e";
                //var senderId = "740055424637"; 
                var applicationID = "AAAAbkNg6FE:APA91bGh1BZdl9AnsDfJfmCgbSEaKpEDzIEzw_jwN8zaeXIpOWGBJc77sXnYcNCoCA15zWkXgyX42gkzOw0sCh0wVDC5NYhwZPM9ZzFEd7Y5y12lzjMs2n15uBbmLzH39TocNXv9MqRQ";
                var senderId = "473576826961";
                string deviceId = "fh5W_w3JSViCXD0rGuUmzl:APA91bGJF1AUoUjQ2Ezk7FkJwfV63CgNoYKXVt8zyHFb__1sdmJBcMxeWI6j1nLTOTT66uDtccC0My8XlH1B48DZIRxbOakW-oWYhO4XCxSzi77M_Zt_5MMSKYUajdE2RaeLDKxIhNEh";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = "test",
                        title = "teest",
                        icon = "myicon",
                        sound = "default"

                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }
    }
}
