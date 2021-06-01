using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PageModels.Admin;

namespace ChromeDriverTests.Admin
{
	[TestClass]
	public class TechAdminTests : AdminTests
	{
		private static readonly string deleteTechName = "TestTech1_TeamA";

		[ClassInitialize]
		public static async Task InitializeTechTests(TestContext context)
		{
			var setup = new SetupMethods(backendUrl);
			await setup.LoginAdmin(adminUsername, adminPassword);
			await setup.AddTech(deleteTechName, "SecretPassword");
		}

		[TestMethod]
		public void DeleteTechTest()
		{
			var loginPage = new LoginPage(driver);
			var techsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToTechsPage();

			var techName = techsPage.ListTechs().First(tech => tech.Equals(deleteTechName));
			techsPage.DeleteTech(techName);

			bool techVisible = techsPage.ListTechs().Any(tech => tech.Equals(techName));
			Assert.IsTrue(techVisible);
		}

		[TestMethod]
		public void AddTechTest()
		{
			var loginPage = new LoginPage(driver);
			var techsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToTechsPage();

			string techName = "TestTech2_TeamA";
			string techPassword = "SecretPassword";
			techsPage.AddTech(techName, techPassword, techPassword);

			Thread.Sleep(200);

			var techVisible = techsPage.ListTechs().Any(tech => tech.Equals(techName));
			Assert.IsTrue(techVisible);
		}
	}
}