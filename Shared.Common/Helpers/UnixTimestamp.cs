using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.Helpers
{
    public static class UnixTimestamp
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return (long)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        public static DateTime UnixTimestampToDateTime(long unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }

        public static long ToUnixTimestamp(DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }

        public static DateTime FromUnixTimestamp(long unixTimestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
        }

        public static DateTime FromUnixTimestamp(string unixTimestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(unixTimestamp)).DateTime;
        }

        public static long ConvertToMiliseconds(long unixTimestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).ToUnixTimeMilliseconds();
        }


        public static string Iso8061DateStringFromUnixTimestamp(long unixTimestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        }
        public static DateTime Iso8061DateFromUnixTimestamp(long unixTimestamp)
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            return DateTime.Parse(time);
        }

        public static long GetCurrentEpoch()
        {
            return ToUnixTimestamp(DateTime.UtcNow);
        }

        public static long GetChartDataBeginTime(long from, long interval)
        {
            return (long)Math.Floor((double)from / interval) * interval;
        }
    }
}

