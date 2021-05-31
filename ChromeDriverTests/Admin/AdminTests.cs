using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChromeDriverTests.Models;
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

		protected static List<StationResponse> stations = new List<StationResponse>();

		[ClassInitialize]
		public static async Task InitializeTests(TestContext context)
		{
			var setup = new SetupMethods(backendUrl);

			await setup.LoginAdmin(adminUsername, adminPassword);
			stations.AddRange(setup.GetAllStations().Result.Stations);

			await setup.AddTech("TestTech_TeamA", "SecretPassword");
			await setup.RegisterUser("TestUser_TeamA", "SecretPassword");
		}

		[TestInitialize]
		public void ChromeDriverInitialize()
		{
			driver = new ChromeDriver();
			driver.Manage().Window.Maximize();
			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
			driver.Navigate().GoToUrl(adminUrl);
		}
	}
}