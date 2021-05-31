using Microsoft.VisualStudio.TestTools.UnitTesting;
using PageModels.Admin;

namespace ChromeDriverTests.Admin
{
	[TestClass]
	public class GeneralAdminTests : AdminTests
	{
		[TestMethod]
		public void LoginTest()
		{
			var loginPage = new LoginPage(driver);
			var bikesPage = loginPage.LoginValidAdmin(adminUsername, adminPassword);
			Assert.IsTrue(bikesPage.IsBikesListVisible());
		}

		[TestMethod]
		public void LogoutTest()
		{
			var loginPage = new LoginPage(driver);
			var bikesPage = loginPage.LoginValidAdmin(adminUsername, adminPassword);
			loginPage = bikesPage.Logout();
			Assert.IsTrue(loginPage.IsLoginFormVisible());
		}
	}
}