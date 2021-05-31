using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PageModels.Admin;

namespace ChromeDriverTests.Admin
{
	[TestClass]
	public class UsersAdminTests : AdminTests
	{
		private static readonly string userToBlock = "TestUser1_TeamA";
		private static readonly string userToUnblock = "TestUser2_TeamA";

		[ClassInitialize]
		public static async Task InitializeUsersTests(TestContext context)
		{
			var setup = new SetupMethods(backendUrl);
			await setup.LoginAdmin(adminUsername, adminPassword);
			await setup.RegisterUser(userToBlock, "SecretPassword");
			await setup.RegisterUser(userToUnblock, "SecretPassword");
		}

		[TestMethod]
		public void ListUsersTest()
		{
			var loginPage = new LoginPage(driver);
			var usersPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToUsersPage();

			var usersList = usersPage.ListUsers();

			Assert.IsTrue(usersList.Contains(userToBlock));
			Assert.IsTrue(usersList.Contains(userToUnblock));
		}

		[TestMethod]
		public void BlockUserTest()
		{
			var loginPage = new LoginPage(driver);
			var usersPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToUsersPage();

			var username = usersPage.ListUsers().First(user => user.Equals(userToBlock));
			if (usersPage.IsUserBlocked(username))
				usersPage.UnblockUser(username);

			usersPage.BlockUser(username);
			Assert.IsTrue(usersPage.IsUserBlocked(username));
		}

		[TestMethod]
		public void UnblockUserTest()
		{
			var loginPage = new LoginPage(driver);
			var usersPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToUsersPage();

			var username = usersPage.ListUsers().First(user => user.Equals(userToUnblock));
			if (!usersPage.IsUserBlocked(username))
				usersPage.BlockUser(username);

			usersPage.UnblockUser(username);
			Assert.IsFalse(usersPage.IsUserBlocked(username));
		}
	}
}