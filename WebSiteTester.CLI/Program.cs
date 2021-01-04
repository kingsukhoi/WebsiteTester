using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using OpenQA.Selenium;
using WebSiteTester.Selenium;

namespace WebSiteTester.CLI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine(@"
This program will open a browser and navigate to the urls that you put in the json file.
Usage: WebsiteTester <Config file location>.
Config file example found on Github. 
");
                Environment.Exit(1);
            }

            string configFileLocation = args[0];

            configFileLocation = Path.GetFullPath(configFileLocation);
            if (!File.Exists(configFileLocation))
            {
                Console.WriteLine($"No file at {configFileLocation}");
                Environment.Exit(1);
            }

            Config config;
            using (var ms = new MemoryStream(File.ReadAllBytes(configFileLocation)))
            {
                var deserializer = new DataContractJsonSerializer(typeof(Config));
                config = (Config) deserializer.ReadObject(ms);
            }
            string logDir = Path.GetFullPath(config.LogPath)+$"WebsiteTest_{DateTime.Now:yyyy-MM-dd-HH-mm}";
            Directory.CreateDirectory(logDir);

            TimeSpan timeout = config.Timeout > 0 ? TimeSpan.FromSeconds(config.Timeout) : TimeSpan.FromSeconds(15);
            
            List<Timing> timings = new List<Timing>();
            using (var browser = new Browser(timeout))
            {
                foreach (Page page in config.Pages)
                {
                    timings.Add(RunTest(page, browser, logDir));
                    
                    if (config.Wait <= 0) continue;
                    Console.WriteLine("Waiting");
                    System.Threading.Thread.Sleep(config.Wait);
                }
            }

            string filePath = $"{logDir}\\WebsiteTest_{DateTime.Now:yyyy-MM-dd-HH-mm}.json";
            using (var file = new StreamWriter(filePath, true, Encoding.UTF8))
            {
                file.Write(JsonConvert.SerializeObject(timings, Formatting.Indented));
            }
            
        }

        private static Timing RunTest(Page page, Browser browser, string logDir)
        {
            //doing this up here because string checking takes longer than boolean checking. 
            bool cssAvailable = !(string.IsNullOrEmpty(page.WaitSelector));
            string currPage = page.Url;
            string currTitle = string.IsNullOrEmpty(page.Title) ? null : page.Title;
            
            var timing = new Timing
            {
                Page = currPage,
                Title = currTitle
            };

            var pageLoadTime = new Stopwatch();
            var cssLoadTime = new Stopwatch();
            pageLoadTime.Start();
            cssLoadTime.Start();
            try
            {
                browser.NavigateAndWait(currPage);
                pageLoadTime.Stop();

                if (cssAvailable)
                {
                    string waitSelector = page.WaitSelector;
                    try
                    {
                        browser.WaitForCss(waitSelector);
                        cssLoadTime.Stop();
                        browser.ScrollToSelector(waitSelector);
                        timing.CssLoadTime = cssLoadTime.Elapsed.ToString();

                    }
                    catch (WebDriverException)
                    {
                        cssLoadTime.Stop();
                        timing.CssLoadTime = "Selector Timed out or not found";
                    }


                }

                timing.PageLoadTime = pageLoadTime.Elapsed.ToString();


                if (page.TakeScreenShot)
                {
                    if (page.Title == null) throw 
                        new ArgumentException($"Error for {timing.Page}. \r\n You need a title to take a screen shot");
                    browser.TakeScreenShot(Path.Combine(logDir, timing.Title));
                }

            }
            catch (WebDriverTimeoutException)
            {
                timing.PageLoadTime = "Page timed out or not found";
            }
            finally
            {
                cssLoadTime.Stop();
                pageLoadTime.Stop();
            }

            return timing;
        }
    }
}