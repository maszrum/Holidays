using System.Diagnostics.CodeAnalysis;
using OpenQA.Selenium;

namespace Holidays.Selenium;

public static class SeleniumExtensions
{
    public static bool TryFindElement(
        this IWebDriver driver,
        By by,
        [NotNullWhen(true)] out IWebElement? element)
    {
        try
        {
            element = driver.FindElement(by);
            return true;
        }
        catch (NoSuchElementException)
        {
            element = default;
            return false;
        }
    }
}
