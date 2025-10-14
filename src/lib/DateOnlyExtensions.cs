using System;

namespace Shipstone.Utilities;

public static class DateOnlyExtensions
{
    public static int GetAgeYears(this DateOnly date, DateOnly today)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(date, today);
        int age = today.Year - date.Year;
        int todayMonth = today.Month;
        int dateMonth = date.Month;

        if (
            todayMonth < dateMonth
            || (todayMonth == dateMonth && today.Day < date.Day)
        )
        {
            -- age;
        }

        return age;
    }
}
