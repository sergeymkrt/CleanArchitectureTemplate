using CleanArchitectureTemplate.Application.Extensions;

namespace CleanArchitectureTemplate.Infrastructure.Shared.Helpers;

public static class DateTimeHelper
{

    /// <summary>
    /// Function to return a date corresponding to the day of week and the week ordinal supplied 
    /// </summary>
    /// <param name="date">The base date for determining the start of the month date to use to calculate the Nth day of week</param>
    /// <param name="nthWeek">Interger representing weekly ordinals First - 1 , Second - 2, Third - 3, Fourth - 4 and Last - 5 week of the month </param>
    /// <param name="dayOfWeek"></param>
    /// <returns> A date that falling on day of week of the Nth week ordinal</returns>
    public static DateTime GetNthDayOfWeekForTheMonth(DateTime date, int nthWeek, DayOfWeek dayOfWeek)
    {
        var startOfMonthDate = new DateTime(date.Year, date.Month, 1);

        var nthDayOfWeekDate = startOfMonthDate.Next(dayOfWeek).AddDays((nthWeek - 1) * 7);

        /*The the Last Week Ordinal nthWeek = (5) check if the month crosses to the next month based on above nthDayOfWeekDate.
            If the date crosses to next month consider the 4th week as last hence subtract nthWeek by 2
         */
        if (nthWeek == 5 && nthDayOfWeekDate.Month > startOfMonthDate.Month)
            nthDayOfWeekDate = startOfMonthDate.Next(dayOfWeek).AddDays((nthWeek - 2) * 7);

        return nthDayOfWeekDate;
    }

}
