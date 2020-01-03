using System;
using System.Text.RegularExpressions;

namespace StupidFirewallManager.Core
{
    public static class DateTimeEncodingHelper
    {
        public static bool TryParse(String value, out DateTime dateTime)
        {
            dateTime = DateTime.MinValue;
            var parsed = Regex.Match(value, $@"{Constants.TcpRulePrefix}(?<date>\d\d\d\d\d\d\d\d\d\d\d\d)");
            if (parsed.Success)
            {
                var dateTimeGroup = parsed.Groups["date"].Value;

                try
                {
                    var year = Int32.Parse(dateTimeGroup.Substring(0, 4));
                    var month = Int32.Parse(dateTimeGroup.Substring(4, 2));
                    var day = Int32.Parse(dateTimeGroup.Substring(6, 2));
                    var hour = Int32.Parse(dateTimeGroup.Substring(8, 2));
                    var minute = Int32.Parse(dateTimeGroup.Substring(10, 2));
                    dateTime = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
