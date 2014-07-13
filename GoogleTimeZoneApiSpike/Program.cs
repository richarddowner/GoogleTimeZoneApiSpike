using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace GoogleTimeZoneApiSpike
{
    class Program
    {
        private static void Main(string[] args)
        {
            var timeZoneInputData = new TimeZoneInputData
            {
                Longatude = -44.490947,
                Latatude = 171.220966,
                TimeZoneDateTime = new DateTime(2013, 7, 10, 2, 52, 49, DateTimeKind.Utc)
            };

            var uri = string.Format(
                @"https://maps.googleapis.com/maps/api/timezone/json?location={0},{1}&timestamp={2}",
                timeZoneInputData.Longatude, timeZoneInputData.Latatude, timeZoneInputData.TimeStamp
            );

            TimeZoneResponseData timeZoneResponse = GetTimeZoneResponseData(uri);
            string output = CreateOutput(timeZoneInputData, timeZoneResponse);
            Console.WriteLine(output);

            Console.ReadLine();
        }

        public static TimeZoneResponseData GetTimeZoneResponseData(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            request.Accept = "application/json";

            var response = (HttpWebResponse) request.GetResponse();
            var objText = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var serializer = new JavaScriptSerializer();
            var timeZoneResponseData = (TimeZoneResponseData)serializer.Deserialize(objText, typeof(TimeZoneResponseData));
            return timeZoneResponseData;
        }

        public static string CreateOutput(TimeZoneInputData inputTimeZoneData, TimeZoneResponseData responseData)
        {
            var unixDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime convertedDateTime = unixDateTimeUtc.AddSeconds(inputTimeZoneData.TimeStamp + responseData.RawOffset);

            var output = string.Format("{0},{1},{2},{3},{4}",
                inputTimeZoneData.TimeZoneDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                inputTimeZoneData.Longatude, inputTimeZoneData.Latatude, responseData.TimeZoneId,
                convertedDateTime.ToString("yyyy-MM-dd HH:mm:ss")
            );

            return output;
        }
    }
}
