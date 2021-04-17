using System;
using Foundation;

namespace Chino
{
    public class Utils
    {
        private Utils() { }

        public static long GetDateMillisSinceEpoch(NSDate date)
        {
            DateTime dateTime = (DateTime)date;

            // TODO: Check TimeZone
            var dto = new DateTimeOffset(dateTime.Ticks, new TimeSpan(0, 00, 00));

            return dto.ToUnixTimeMilliseconds();
        }
    }
}
