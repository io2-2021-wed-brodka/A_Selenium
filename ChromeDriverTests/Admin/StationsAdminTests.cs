using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChromeDriverTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PageModels.Admin;
using PageModels.Models;

namespace ChromeDriverTests.Admin
{
	[TestClass]
	public class StationsAdminTests : AdminTests
	{
		private static readonly string emptyTestStation = "TestEmptyStation_TeamA";

		private static readonly List<string> malfunctionStations = new List<string>
		{
			"TestMalfunctionStation1_TeamA", "TestMalfunctionStation2_TeamA"
		};

		private static readonly List<StationResponse> stationsInDB = new List<StationResponse>();

		private static readonly Dictionary<string, List<Malfunction>> malfunctionsMap =
			new Dictionary<string, List<Malfunction>>();

		[ClassInitialize]
		public static async Task InitializeStationsTests(TestContext context)
		{
			var setup = new SetupMethods(backendUrl);
			await setup.LoginAdmin(adminUsername, adminPassword);
			await setup.AddStation(new[] { emptyTestStation });

			var stationResponses = await setup.AddStation(malfunctionStations);

			foreach (var station in stationResponses)
			{
				var malfunctions = new List<Malfunction>();

				for (int i = 0; i < 2; i++)
				{
					var bikeResponse = await setup.AddBike(station.Id);
					await setup.RentBike(bikeResponse.Id);

					var malfunction = new Malfunction()
					{
						Id = bikeResponse.Id,
						Description = $"Malfunction of bike {bikeResponse.Id} from station {station.Name}"
					};
					malfunctions.Add(malfunction);

					await setup.AddMalfunction(malfunction);
					await setup.ReturnBike(bikeResponse.Id, station.Id);
				}

				malfunctionsMap.Add(station.Name, malfunctions);
			}

			var stations = await setup.GetAllStations();
			stationsInDB.AddRange(stations);

			// remove empty station, so that we don't test its presence in a list (it is to be deleted)
			stationsInDB.RemoveAll(s => s.Name.Equals(emptyTestStation));
		}

		[TestMethod]
		public void ListAllStations()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();
			var stationsList = stationsPage.ListStations();

			foreach (var station in stationsInDB)
			{
				var stationVisible = stationsList.Any(s => s.Name.Equals(station.Name));
				Assert.IsTrue(stationVisible);
			}
		}

		[TestMethod]
		public void ListAllStationsWithBikesLimits()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();
			Thread.Sleep(200);

			foreach (var station in stationsInDB)
			{
				var stationDetails = stationsPage.GetStationDetails(station.Name);
				Assert.AreEqual(station.BikesLimit, stationDetails.BikesLimit);
			}
		}

		[TestMethod]
		public void ListAllStationsWithMalfunctions()
		{
			var loginPage = new LoginPage(driver);
			var stationsPage = loginPage.LoginValidAdmin(adminUsername, adminPassword).GoToStationsPage();

			foreach (string stationName in malfunctionStations)
			{
				var stationDetails = stationsPage.GetStationDetails(stationName);
				Thread.Sleep(200);

				Assert.AreEqual(malfunctionsMap[stationName].Count, stationDetails.RelatedMalfunctions.Count);

				foreach (var malfunction in stationDetails.RelatedMalfunctions)
				{
					Assert.IsTrue(malfunctionsMap[stationName].Contains(malfunction));
				}
			}
		}

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

			var station = stationsPage.ListStations().First(s => s.ActiveBikesCount > 0);
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