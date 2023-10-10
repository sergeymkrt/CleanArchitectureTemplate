namespace CleanArchitectureTemplate.Application.DTOs;

public class DatePeriodDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public DatePeriodDto() { }

    public DatePeriodDto(DateTime? fromDate, DateTime? toDate)
    {
        FromDate = fromDate;
        ToDate = toDate;
    }
}
