using OpenQA.Selenium.Chrome;
using PageModels.UserTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageModels
{
    class Program
    {
        public static void Main()
        {
            ChromeOptions options = new();
            options.AddArgument("test-type");
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--allow-running-insecure-content");

            var driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            driver.Navigate().GoToUrl("http://localhost:3000/");



            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser("admin", "admin");
            var techPage = homePage.MoveToTechTab();
            var duh = techPage.ListAllMalfunctions();
            Console.WriteLine("Ayy");
        }
    }
}
