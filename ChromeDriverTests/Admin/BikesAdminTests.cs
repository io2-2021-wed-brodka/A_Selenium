using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PageModels.Admin;

namespace ChromeDriverTests.Admin
{
	[TestClass]
	public class BikesAdminTests : AdminTests
	{
		// [TestMethod]
		// public void AddBikeTest()
		// {
		// 	var loginPage = new LoginPage(driver);
		// 	var bikesPage = loginPage.LoginValidAdmin(adminUsername, adminPassword);
		// 	var stationName = stations[0].Name;
		//
		// 	var bikesBefore = bikesPage.ListBikes().Where(bike => bike.StationName.Equals(stationName));
		//
		// 	bikesPage.AddBike(stationName);
		//
		// 	var bikesAfter = bikesPage.ListBikes().Where(bike => bike.StationName.Equals(stationName)).ToList();
		//
		// 	Assert.AreEqual(bikesBefore.Count() + 1, bikesAfter.Count());
		// }

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