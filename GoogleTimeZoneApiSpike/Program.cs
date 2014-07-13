using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using GoogleTimeZoneApiSpike.Dto;

namespace GoogleTimeZoneApiSpike
{
    class Program
    {
        private static void Main()
        {
            IEnumerable<string[]> inputData = File.ReadAllLines("input.csv").Select(x => x.Split(','));

            var timeZoneInputDataCollection = inputData.Select(
                x => new TimeZoneInputData
                {
                    TimeZoneDateTime = Convert.ToDateTime(x[0]), 
                    Longatude = Convert.ToDouble(x[1]), 
                    Latatude = Convert.ToDouble(x[2])
                }
            ).ToList();
            
            foreach (var timeZoneInputData in timeZoneInputDataCollection)
            {
                var uri = string.Format(
                    @"https://maps.googleapis.com/maps/api/timezone/json?location={0},{1}&timestamp={2}",
                    timeZoneInputData.Longatude, timeZoneInputData.Latatude, timeZoneInputData.TimeStamp
                );

                TimeZoneResponseData timeZoneResponse = GetTimeZoneResponseData(uri);
                Console.WriteLine(CreateOutput(timeZoneInputData, timeZoneResponse));
            }

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
