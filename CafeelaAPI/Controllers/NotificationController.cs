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
                var senderId = "1070252443311";
                string deviceId = "fQ6krLJRTV6e97TmK-7Hks:APA91bHkUj_Vowc-5mr4epwABKf2EDLjrHhIv3wSP-uTbSWLsYy11j2cCgqwfPtQwuFAxopRo5EdqRp8XNHncPL_3g8NMkit9EcLUAbuYro2_yq09aGsAMXhS2knC5OG1O84buTZemSf";
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
