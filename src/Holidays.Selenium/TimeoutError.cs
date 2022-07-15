using Holidays.Core.Monads;

namespace Holidays.Selenium;

public class TimeoutError : ErrorBase
{
    public TimeoutError(string websiteName)
    {
        Message = $"Scraping website timed out, unable to load offers on website: {websiteName}";
    }

    public override string Message { get; }
}
