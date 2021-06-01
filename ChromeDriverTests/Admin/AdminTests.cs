using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ChromeDriverTests.Admin
{
	[TestClass]
	public class AdminTests
	{
		protected IWebDriver driver;

		protected static string backendUrl = "http://localhost:8080";
		protected static string adminUrl = "http://localhost:3000";

		protected static string adminUsername = "admin";
		protected static string adminPassword = "admin";

		[TestInitialize]
		public void ChromeDriverInitialize()
		{
			driver = new ChromeDriver();
			driver.Manage().Window.Maximize();
			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
			driver.Navigate().GoToUrl(adminUrl);
		}

		[TestCleanup]
		public void ChromeDriverCleanup()
		{
			driver.Quit();
		}
	}
}