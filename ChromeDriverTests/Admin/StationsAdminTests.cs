using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PageModels.Admin;

namespace ChromeDriverTests.Admin
{
	[TestClass]
	public class StationsAdminTests : AdminTests
	{
		private static readonly string emptyTestStation = "TestEmptyStation_TeamA";

		[ClassInitialize]
		public static async Task InitializeStationsTests(TestContext context)
		{
			var setup = new SetupMethods(backendUrl);
			await setup.LoginAdmin(adminUsername, adminPassword);
			await setup.AddStation(new string[] { emptyTestStation });
		}

		// [TestMethod]
		// public void ListStationsWithDetailsTest()
		// {
		// 	var loginPage = new LoginPage(driver);
		// 	var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();
		// 	var stationsList = stationsPage.ListStations();
		//
		// 	var station = stationsList[0];
		// 	var stationDetails = stationsPage.GetStationDetails(station.Name);
		// }

		[TestMethod]
		public void AddStationTest()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();

			string stationName = "TestStation1_TeamA";
			stationsPage.AddStation(stationName);
			Thread.Sleep(200);

			var stationsList = stationsPage.ListStations();
			bool stationVisible = stationsList.Any(station => station.Name.Equals(stationName));
			Assert.IsTrue(stationVisible);
		}

		[TestMethod]
		public void AddStationWithBikesLimitTest()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();

			string stationName = "TestStation2_TeamA";
			int bikesLimit = 5;

			stationsPage.AddStation(stationName, bikesLimit);
			Thread.Sleep(200);

			var stationsList = stationsPage.ListStations();
			bool stationVisible = stationsList.Any(station => station.Name.Equals(stationName));
			Assert.IsTrue(stationVisible);

			var stationDetails = stationsPage.GetStationDetails(stationName);
			Assert.AreEqual(bikesLimit, stationDetails.BikesLimit);
		}

		[TestMethod]
		public void BlockStationTest()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();
			var stationsList = stationsPage.ListStations();

			var station = stationsList[0];
			if (station.Status.Equals("blocked"))
				stationsPage.UnblockStation(station.Name);

			stationsPage.BlockStation(station.Name);

			Assert.IsTrue(stationsPage.IsStationBlocked(station.Name));
		}

		[TestMethod]
		public void UnblockStationTest()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();
			var stationsList = stationsPage.ListStations();

			var station = stationsList[0];
			if (station.Status.Equals("active"))
				stationsPage.BlockStation(station.Name);

			stationsPage.UnblockStation(station.Name);

			Assert.IsTrue(stationsPage.IsStationUnblocked(station.Name));
		}

		[TestMethod]
		public void DeleteUnblockedStationTest()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();

			var station = stationsPage.ListStations()[0];
			if (station.Status.Equals("blocked"))
				stationsPage.UnblockStation(station.Name);

			stationsPage.DeleteStation(station.Name);

			bool stationVisible = stationsPage.ListStations().Any(s => s.Name.Equals(station.Name));
			Assert.IsTrue(stationVisible);
		}

		[TestMethod]
		public void DeleteBlockedStationWithBikesTest()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();

			var station = stationsPage.ListStations().First(s => !s.Name.Equals(emptyTestStation));
			if (station.Status.Equals("active"))
				stationsPage.BlockStation(station.Name);

			stationsPage.DeleteStation(station.Name);

			bool stationVisible = stationsPage.ListStations().Any(s => s.Name.Equals(station.Name));
			Assert.IsTrue(stationVisible);
		}

		[TestMethod]
		public void DeleteBlockedStationWithoutBikesTest()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();

			var station = stationsPage.ListStations().First(s => s.Name.Equals(emptyTestStation));
			if (station.Status.Equals("active"))
				stationsPage.BlockStation(station.Name);

			stationsPage.DeleteStation(station.Name);
			Thread.Sleep(200);

			bool stationVisible = stationsPage.ListStations().Any(s => s.Name.Equals(station.Name));
			Assert.IsFalse(stationVisible);
		}
	}
}