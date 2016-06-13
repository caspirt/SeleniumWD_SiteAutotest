using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SeleniumWD_SiteAutotest
{
    [TestClass]
    public class TutByWebriverFindTextInJob 
    {


        private String used_Browser = "chrome";// "firefox" "IE"

        private String used_URL = "http://www.tut.by/";// "firefox" "IE"

        private String searchQuery = "Специалист по тестированию";

        private String xPath1 = "//a[@title=\"Работа\"]";

        private String xPath2 = "//input[contains(@data-qa,'vacancy')]";

        private String xPath3 = "//div[contains(@class,'search-result-item__head')]/*";

        private String searchQueryWithRegex;

        private IWebDriver driver;

        private void driver_init(String browser)
        {
            switch (used_Browser.ToLower())
            {
                case "IE":
                    driver = new InternetExplorerDriver();
                    break;
                case "firefox":
                    driver = new FirefoxDriver();
                    break;
                default: //chrome as default
                    driver = new ChromeDriver();
                    break;
            }
        }

        private void searchQueryRegexParser (String searchQuery)
        {
            //init part of regex - for exampel (^\W*|^)(специалист)\W*(по)\W*(тестированию)
            string template = @"(\W*|^)(";

            foreach (char c in searchQuery.ToLower())
            {
                if (c.CompareTo(' ')==0)
                { //if space - make delimeter for search
                    template = string.Concat(@template, @")(\W*)(");
                }
                else
                { //if not space - just add char to regex
                    template = string.Concat(@template, @c.ToString());
                }
            }
            //end part of regex
            template = string.Concat(@template, @")\W*");
            template.Replace(@"\\", @"\");
            //return regexp for matching
            searchQueryWithRegex = @template;
        }


        [TestMethod]
        public void FindNumberOfTextOccurence()
        {
            //Selenium driver initialize
            driver_init(used_Browser);
            
            // implicity waiting time 
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(6));
            
            // open page
            driver.Navigate().GoToUrl(used_URL);
            
            // go to xPath1 link
            IWebElement element = driver.FindElement(By.XPath(xPath1));
            element.Click();
            
            // go to xPath2 field and send searchQuery text
            element = driver.FindElement(By.XPath(xPath2));
            element.SendKeys(searchQuery);
            element.Submit();
            
            // Find all titles in page
            ReadOnlyCollection <IWebElement> ElementCollection = driver.FindElements(By.XPath(xPath3));
            
            //init for counter 
            int answer = 0;
            //init regexp for search query
            searchQueryRegexParser(searchQuery);

            //all elements are checked for matching with search query
            foreach (IWebElement Element in ElementCollection)
            {
                if (Regex.IsMatch (Element.Text.ToLower(), @searchQueryWithRegex))
                {
                    answer++;
                }
            }

            if (answer == 0)
            {
                Console.WriteLine(string.Concat("Can not find ", searchQuery, " on this page"));
            }
            else
            {
                Console.WriteLine("Can find ", answer.ToString(), " instance(s) of " ,searchQuery, " on this page");
            }
            // test is failed if found 0 records
            Assert.AreNotEqual(answer,0, " test is failed due to 0 records returned ");
        }

        [TestCleanup]
        public void driver_down()
        {
            driver.Quit();
        }
    }
}
