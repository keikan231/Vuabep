using System;

namespace CRS.Common.Helpers
{
    public static class DateTimeHelper
    {
        public static string AgoString(this DateTime dateTime)
        {
            TimeSpan span = DateTime.Now - dateTime;
            double day = Math.Floor(span.TotalDays);
            double sec = span.TotalSeconds;

            if (day <= 0)
            {
                if (sec < 60)
                    return "vừa mới đây";
                if (sec < 120)
                    return "khoảng 1 phút trước";
                if (sec < 3600)
                    return string.Format("{0} phút trước", Math.Floor(span.TotalMinutes));
                if (sec < 7200)
                    return "khoảng 1 tiếng trước";
                return string.Format("{0} tiếng trước", Math.Floor(span.TotalHours));
            }

            if (day == 1)
                return "hôm qua";
            if (day < 7)
                return string.Format("{0} ngày trước", day);
            if (day < 31)
                return string.Format("{0} tuần trước", Math.Floor(day / 7));
            return dateTime.ToLongDateTimeDetailsString();
        }

        public static string ShortDate(this DateTime dateTime)
        {
            return string.Format("{0:dd/MM}", dateTime);
        }

        public static string ToShortDateFormatString(this DateTime dateTime)
        {
            return string.Format("{0:dd/MM/yyyy}", dateTime);
        }

        public static string ToLongDateTimeDetailsString(this DateTime dateTime)
        {
            return string.Format("{3}:{4}, ngày {0} tháng {1} năm {2}", dateTime.Day, dateTime.Month, dateTime.Year, dateTime.ToString("HH"), dateTime.ToString("mm"));
        }
    }
}