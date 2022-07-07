using Holidays.Core.Monads;
using OpenQA.Selenium;

namespace Holidays.Selenium;

public class WebScrapingError : ErrorBase<WebDriverException>
{
    public WebScrapingError(WebDriverException exception, string websiteName)
    {
        Message = $"An error occured when scraping website: {websiteName}";
        Exception = exception;
    }

    public override string Message { get; }
    
    public override WebDriverException Exception { get; }
}
