using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using GoogleTimeZoneApiSpike.Dto;
using GoogleTimeZoneApiSpike.Settings;
using Newtonsoft.Json;

namespace GoogleTimeZoneApiSpike
{
    static class Program
    {
        private static void Main()
        {
            IEnumerable<string[]> inputData = File.ReadAllLines(Constants.InputFile).Select(x => x.Split(','));

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

        private static TimeZoneResponseData GetTimeZoneResponseData(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            request.Accept = "application/json";

            var httpClient = new HttpClient();
            var response = httpClient.GetAsync(uri).Result;
            var objectData = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<TimeZoneResponseData>(objectData);
        }

        private static string CreateOutput(TimeZoneInputData inputTimeZoneData, TimeZoneResponseData responseData)
        {
            var unixDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime convertedDateTime = unixDateTimeUtc.AddSeconds(inputTimeZoneData.TimeStamp + responseData.RawOffset);

            var output = string.Format("{0},{1},{2},{3},{4}",
                inputTimeZoneData.TimeZoneDateTime.ToString(Constants.DateFormat),
                inputTimeZoneData.Longatude, inputTimeZoneData.Latatude, responseData.TimeZoneId,
                convertedDateTime.ToString(Constants.DateFormat)
            );

            return output;
        }
    }
}
