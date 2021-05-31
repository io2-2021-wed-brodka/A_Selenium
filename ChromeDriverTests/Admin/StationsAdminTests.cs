using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PageModels.Admin;

namespace ChromeDriverTests.Admin
{
	[TestClass]
	public class StationsAdminTests : AdminTests
	{
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
		public void DeleteBlockedStationTest()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();
		
			var station = stationsPage.ListStations()[0];
			if (station.Status.Equals("active"))
				stationsPage.BlockStation(station.Name);
		
			stationsPage.DeleteStation(station.Name);
		
			bool stationVisible = stationsPage.ListStations().Any(s => s.Name.Equals(station.Name));
			Assert.IsFalse(stationVisible);
		}
	}
}