using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PageModels.Admin;

namespace ChromeDriverTests.Admin
{
	[TestClass]
	public class BikesAdminTests : AdminTests
	{
		private static readonly List<string> bikesInDB = new List<string>();
		private static readonly List<string> stationNamesInDB = new List<string>();

		[ClassInitialize]
		public static async Task InitializeBikesTests(TestContext context)
		{
			var setup = new SetupMethods(backendUrl);
			await setup.LoginAdmin(adminUsername, adminPassword);

			var bikesMap = await setup.GetAllBikes();

			stationNamesInDB.AddRange(bikesMap.Keys);

			foreach (var bikeSet in bikesMap.Values)
			{
				bikesInDB.AddRange(bikeSet);
			}
		}

		[TestMethod]
		public void AddBikeTest()
		{
			var loginPage = new LoginPage(driver);
			var bikesPage = loginPage.LoginValidAdmin(adminUsername, adminPassword);
			var stationName = stationNamesInDB[0];

			var bikesBefore = bikesPage.ListBikes().Where(bike => bike.StationName.Equals(stationName));

			bikesPage.AddBike(stationName);
			Thread.Sleep(200);

			var bikesAfter = bikesPage.ListBikes().Where(bike => bike.StationName.Equals(stationName)).ToList();

			Assert.AreEqual(bikesBefore.Count() + 1, bikesAfter.Count());
		}

		[TestMethod]
		public void ListBikesTest()
		{
			var loginPage = new LoginPage(driver);
			var bikesPage = loginPage.LoginValidAdmin(adminUsername, adminPassword);
			Thread.Sleep(300);
			
			var bikesList = bikesPage.ListBikes();
		
			foreach (string bikeId in bikesInDB)
			{
				Assert.IsTrue(bikesList.Any(bike => bike.BikeName.Equals($"Bike {bikeId}")));
			}
		}
		
		[TestMethod]
		public void BlockBikeTest()
		{
			var loginPage = new LoginPage(driver);
			var bikesPage = loginPage.LoginValidAdmin(adminUsername, adminPassword);
			var bikesList = bikesPage.ListBikes();
		
			var bike = bikesList[0];
			var bikeId = bike.BikeName.Replace("Bike ", "");
		
			if (bike.Status.Equals("blocked"))
				bikesPage.UnblockBike(bikeId);
		
			bikesPage.BlockBike(bikeId);
		
			Assert.IsTrue(bikesPage.IsBikeBlocked(bikeId));
		}
		
		[TestMethod]
		public void UnblockBikeTest()
		{
			var loginPage = new LoginPage(driver);
			var bikesPage = loginPage.LoginValidAdmin(adminUsername, adminPassword);
			var bikesList = bikesPage.ListBikes();
		
			var bike = bikesList[0];
			var bikeId = bike.BikeName.Replace("Bike ", "");
		
			if (bike.Status.Equals("available"))
				bikesPage.BlockBike(bikeId);
		
			bikesPage.UnblockBike(bikeId);
		
			Assert.IsTrue(bikesPage.IsBikeUnblocked(bikeId));
		}
		
		[TestMethod]
		public void DeleteUnblockedBikeTest()
		{
			var loginPage = new LoginPage(driver);
			var bikesPage = loginPage.LoginValidAdmin(adminUsername, adminPassword);
		
			var bike = bikesPage.ListBikes()[0];
			var bikeId = bike.BikeName.Replace("Bike ", "");
		
			if (bike.Status.Equals("blocked"))
				bikesPage.UnblockBike(bikeId);
		
			bikesPage.DeleteBike(bikeId);
		
			bool bikeVisible = bikesPage.ListBikes().Any(b => b.BikeName.Equals($"Bike {bikeId}"));
			Assert.IsTrue(bikeVisible);
		}
		
		[TestMethod]
		public void DeleteBlockedBikeTest()
		{
			var loginPage = new LoginPage(driver);
			var bikesPage = loginPage.LoginValidAdmin(adminUsername, adminPassword);
		
			var bike = bikesPage.ListBikes()[0];
			var bikeId = bike.BikeName.Replace("Bike ", "");
		
			if (bike.Status.Equals("available"))
				bikesPage.BlockBike(bikeId);
		
			bikesPage.DeleteBike(bikeId);
		
			bool bikeVisible = bikesPage.ListBikes().Any(b => b.BikeName.Equals($"Bike {bikeId}"));
			Assert.IsFalse(bikeVisible);
		}
	}
}