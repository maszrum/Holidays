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
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--no-sandbox");

        var webDriver = new RemoteWebDriver(
            new Uri(_settings.RemoteWebDriverUrl),
            chromeOptions.ToCapabilities(), 
            TimeSpan.FromSeconds(_settings.CommandTimeoutSeconds));

        return webDriver;
    }
}
