using System;

namespace GoogleTimeZoneApiSpike.Dto
{
    public class TimeZoneInputData
    {
        public double Longatude { get; set; }

        public double Latatude { get; set; }

        public long TimeStamp { get; set; }

        private DateTime _timeZoneDateTime;
        public DateTime TimeZoneDateTime
        {
            get { return _timeZoneDateTime; }
            set
            {
                _timeZoneDateTime = value;

                DateTime unixStart = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
                TimeStamp = (long)Math.Floor((_timeZoneDateTime.ToUniversalTime() - unixStart).TotalSeconds);
            }
        }
    }
}