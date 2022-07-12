using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Holidays.Selenium;

public class WebDriverFactory
{
    private readonly SeleniumSettings _settings;

    public WebDriverFactory(SeleniumSettings settings)
    {
        _settings = settings;
    }

    public IWebDriver Create()
    {
        return _settings.UseRemoteWebDriver
            ? CreateRemoteWebDriver()
            : CreateLocalWebDriver();
    }

    private IWebDriver CreateLocalWebDriver()
    {
        var chromeOptions = GetChromeOptions();

        var webDriver = new ChromeDriver(
            ChromeDriverService.CreateDefaultService(), 
            chromeOptions, 
            TimeSpan.FromSeconds(_settings.CommandTimeoutSeconds));

        return webDriver;
    }

    private IWebDriver CreateRemoteWebDriver()
    {
        var chromeOptions = GetChromeOptions();

        var webDriver = new RemoteWebDriver(
            new Uri(_settings.RemoteWebDriverUrl),
            chromeOptions.ToCapabilities(), 
            TimeSpan.FromSeconds(_settings.CommandTimeoutSeconds));

        return webDriver;
    }

    private static ChromeOptions GetChromeOptions()
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--no-sandbox");
        return chromeOptions;
    }
}
