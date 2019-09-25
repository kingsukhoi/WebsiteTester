using System;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace WebSiteTester.Selenium
{
    public class Browser : IDisposable
    {
        //TODO implement multiple browsers

        private IWebDriver _webDriver;
        private readonly TimeSpan _timeOut;

        public Browser(TimeSpan timeOut)
        {
            _timeOut = timeOut;
            CreateWebDriver();
        }

        public Browser()
        {
            CreateWebDriver();
        }

        private void CreateWebDriver()
        {
            var options = new ChromeOptions();
            //this is so that I can view developdnn without any issues. 
            options.AddArgument("--ignore-certificate-errors");
            _webDriver = new ChromeDriver(options);
        }

        public void NavigateAndWait(string url)
        {
            _webDriver.Navigate().GoToUrl(url);
            // copied from https://stackoverflow.com/questions/36590274/selenium-how-to-wait-until-page-is-completely-loaded
            new WebDriverWait(_webDriver, _timeOut)
                .Until(d => ((IJavaScriptExecutor) d).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public void WaitForCss(string elemSelector)
        {
            new WebDriverWait(_webDriver, _timeOut)
                .Until((driver => driver.FindElement(By.CssSelector(elemSelector))));
        }

        private static string AddFileExtensionIfNotExist(string path)
        {
            var regex = new Regex(@"\.png$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (!regex.IsMatch(path))
            {
                path = $"{path}.png";
            }

            return path;
        }

        public void TakeScreenShot(string fileLocation)
        {
            fileLocation = AddFileExtensionIfNotExist(fileLocation);

            Screenshot ss = ((ITakesScreenshot) _webDriver).GetScreenshot();
            ss.SaveAsFile(fileLocation, ScreenshotImageFormat.Png);
        }

        public void ScrollToSelector(string selector)
        {
            string cmd = $"document.querySelector('{selector}').scrollIntoView(true)";
            ((IJavaScriptExecutor) _webDriver).ExecuteScript(cmd);
        }

        public void Dispose()
        {
            _webDriver.Close();
            _webDriver.Dispose();
        }
    }
}
