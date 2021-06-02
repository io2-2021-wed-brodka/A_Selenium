using ChromeDriverTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using PageModels.UserTech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UserTech
{
    [TestClass]
    public class UserTechTests
    {

        private IWebDriver driver;

        private static string backendUrl = "http://localhost:8080";
        private static string userTechUrl = "http://localhost:3000/";

        private static string userUsername = "sampleUserTeamA1";
        private static string userPassword = "sampleUserPassword";

        private static string techUsername = "sampleTechTeamA";
        private static string techPassword = "sampleTechPassword";

        private static string adminUsername = "admin";
        private static string adminPassword = "admin";

        private static IDictionary<string, HashSet<string>> stationBikes;

        [ClassInitialize]
        public static async Task HttpDriverInitializeAsync(TestContext context)
        {
            var databaseSetup = new SetupMethods(backendUrl);

            await databaseSetup.RegisterUser(userUsername, userPassword);
            await databaseSetup.LoginAdmin(adminUsername, adminPassword);
            await databaseSetup.AddTech(techUsername, techPassword);

            stationBikes = await databaseSetup.GetAllBikes();

            var stations = await databaseSetup.AddStation(new[]{
                "TestStation1 TeamA",
            });

            foreach (var station in stations)
            {
                stationBikes.Add(station.Name, new HashSet<string>());
                for (int i = 0; i < 2; i++)
                {
                    stationBikes[station.Name].Add((await databaseSetup.AddBike(station.Id)).Id);
                }
            }

            Console.WriteLine("Initiated!");
        }


        [TestInitialize]
        public void ChromeDriverInitialize()
        {
            ChromeOptions options = new();            
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--allow-running-insecure-content");

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            driver.Navigate().GoToUrl(userTechUrl);

        }

        // 1) Jako nowy użytkownik mogę utworzyć konto
        // 2) Jako użytkownik mogę się zalogować
        [TestMethod]
        public void RegisterAndLoginTest()
        {
            string testUsername = Path.GetRandomFileName().Replace(".", "");
            string testPassword = Path.GetRandomFileName().Replace(".", "");

            var loginPage = new LoginPage(driver);
            var registerPage = loginPage.GoToCreateAccountPage();
            loginPage = registerPage.createUser(testUsername, testPassword);
            var homePage = loginPage.loginValidUser(testUsername, testPassword);

            Assert.IsTrue(homePage.IsWelcomeTextVisible());
        }

        // 6) Jako zalogowany użytkownik mogę wypożyczyć wybrany rower (z wybranej stacji)
        [TestMethod]
        public void RentedBikeAppearsRentedTest()
        {
            string rentedBikeId = stationBikes.Values.ElementAt(1).ElementAt(1);

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            var stationName = homePage.RentBike(rentedBikeId);
            var isBikeRented = homePage.IsBikeRented(rentedBikeId);
            Assert.IsTrue(isBikeRented);

            homePage.ReturnBike(rentedBikeId, stationName);
        }

        // 7) Jako zalogowany użytkownik mogę zwrócić rower (na wybraną stację)
        [TestMethod]
        public void CannotReturnUnrentedBike()
        {
            string stationName = stationBikes.Keys.ElementAt(1);
            string rentedBikeId = stationBikes.Values.ElementAt(1).ElementAt(1);

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            Assert.ThrowsException<KeyNotFoundException>(() => homePage.ReturnBike(rentedBikeId, stationName));
        }

        // 6) Jako zalogowany użytkownik mogę wypożyczyć wybrany rower (z wybranej stacji)
        [TestMethod]
        public void CannotRentBikeFromEmptyStation()
        {
            string stationName = stationBikes.Keys.ElementAt(2);
            var rentedBikeIds = stationBikes[stationName];

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            foreach (var id in rentedBikeIds)
            {
                homePage.RentBike(id);
            }

            var bikesOnStations = homePage.ListBikesOnStations();
            Assert.IsTrue(bikesOnStations[stationName].Count == 0);

            foreach (var id in rentedBikeIds)
            {
                homePage.ReturnBike(id, stationName);
            }
        }

        // 6) Jako zalogowany użytkownik mogę wypożyczyć wybrany rower (z wybranej stacji)
        [TestMethod]
        public void RentedBikeDisappearsFromStationTest()
        {
            string rentedBikeId = stationBikes.Values.ElementAt(2).ElementAt(1);

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            var stationName = homePage.RentBike(rentedBikeId);

            var bikesOnStations = homePage.ListBikesOnStations();
            var bikeAssignedToStation = bikesOnStations[stationName].Contains(rentedBikeId);

            Assert.IsFalse(bikeAssignedToStation);

            homePage.ReturnBike(rentedBikeId, stationName);
        }

        // 4) Jako zalogowany użytkownik mogę wyświetlić listę stacji(i którąś z nich wybrać)
        [TestMethod]
        public void StationsVisibleTest()
        {
            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            var stationsCount = homePage.GetStationCount();

            Assert.IsTrue(stationsCount > 0);
        }

        // 2) Jako użytkownik mogę się zalogować
        [TestMethod]
        public void LoginInvalidUserTest()
        {
            var loginPage = new LoginPage(driver);
            loginPage.loginValidUser("invalid", "user");

            var isWelcomeTextVisible = loginPage.IsWelcomeTextVisible();

            Assert.IsFalse(isWelcomeTextVisible);
        }

        // 3) Jako zalogowany użytkownik mogę się wylogować
        [TestMethod]
        public void LogoutTest()
        {
            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            loginPage = homePage.LogOut();

            var isWelcomeTextVisible = loginPage.IsWelcomeTextVisible();

            Assert.IsFalse(isWelcomeTextVisible);
        }

        // 4) Jako zalogowany użytkownik mogę wyświetlić listę stacji (i którąś z nich wybrać) 
        // 5) Jako zalogowany użytkownik mogę wyświetlić listę rowerów na wybranej stacji
        [TestMethod]
        public void ListBikeOnStations()
        {
            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            var stationBikesUser = homePage.ListBikesOnStations();

            bool dictionariesEqual = true;

            Assert.IsTrue(stationBikes.Keys.All(k => stationBikesUser.Keys.Contains(k)));

            foreach (var (key, value) in stationBikes)
            {
                if (stationBikesUser.TryGetValue(key, out var bikesSet))
                {
                    if (!bikesSet.SetEquals(value))
                    {
                        dictionariesEqual = false;
                        break;
                    }
                }
                else
                {
                    dictionariesEqual = false;
                    break;
                }
            }

            Assert.IsTrue(dictionariesEqual);
        }

        // 6) Jako zalogowany użytkownik mogę wypożyczyć wybrany rower (z wybranej stacji)
        [TestMethod]
        public void ReturnedBikeDisappearsTest()
        {
            string stationName = stationBikes.Keys.First();
            string rentedBikeId = stationBikes[stationName].First();

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            homePage.RentBike(rentedBikeId);
            homePage.ReturnBike(rentedBikeId, stationName);

            Assert.IsFalse(homePage.IsBikeRented(rentedBikeId));
        }

        // 7) Jako zalogowany użytkownik mogę zwrócić rower (na wybraną stację)
        [TestMethod]
        public void ReturnedBikeReturnToStationTest()
        {
            string stationName = stationBikes.Keys.First();
            string rentedBikeId = stationBikes[stationName].First();

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            homePage.RentBike(rentedBikeId);
            homePage.ReturnBike(rentedBikeId, stationName);
            var bikesOnStations = homePage.ListBikesOnStations();
            var bikeAassignedToCorrectStation = bikesOnStations[stationName].Contains(rentedBikeId);

            Assert.IsTrue(bikeAassignedToCorrectStation);
        }

        // 13) jako serwisant mogę blokować/odblokowywać rowery
        [TestMethod]
        public void TechBlockBike()
        {
            string bikeId = stationBikes.ElementAt(1).Value.First();

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(techUsername, techPassword);
            var techPage = homePage.MoveToTechTab();
            techPage.BlockBike(bikeId);
            var isBikeBlocked = techPage.IsBikeBlocked(bikeId);
            Assert.IsTrue(isBikeBlocked);
            techPage.UnlockBike(bikeId);
        }

        // 13) jako serwisant mogę blokować/odblokowywać rowery
        [TestMethod]
        public void TechUnblockBike()
        {
            string bikeId = stationBikes.ElementAt(1).Value.First();

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(techUsername, techPassword);
            var techPage = homePage.MoveToTechTab();
            techPage.BlockBike(bikeId);
            techPage.UnlockBike(bikeId);
            var isBikeBlocked = techPage.IsBikeBlocked(bikeId);
            Assert.IsFalse(isBikeBlocked);
        }

        // 14) jako serwisant mam dostęp do listy usterek 
        // 10) jako użytkownik mogę zglosic usterke roweru
        [TestMethod]
        public void ReportMalfunction()
        {
            string bikeId = stationBikes.ElementAt(1).Value.First();
            string malfunctionDescription = "borked";

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            var rentedStationName = homePage.RentBike(bikeId);
            homePage.ReportMalfunction(bikeId, malfunctionDescription, rentedStationName);
            loginPage = homePage.LogOut();
            homePage = loginPage.loginValidUser(techUsername, techPassword);
            var techPage = homePage.MoveToTechTab();
            var malfunctions = techPage.ListAllMalfunctions();
            var isMalfunctionOnList = malfunctions.Any(m => m.BikeId.Contains(bikeId, StringComparison.CurrentCultureIgnoreCase)
                                   && m.Description.Contains(malfunctionDescription, StringComparison.CurrentCultureIgnoreCase));
            Assert.IsTrue(isMalfunctionOnList);
        }

        //  8) jako użytkownik mogę zarezerwować/anulować rezerwację rower/u
        //  (+odpowiednie wyświetlanie i automatyczne wygaśnięcie rezerwacji)
        [TestMethod]
        public void ReserveBikeTest()
        {
            string bikeId = stationBikes.ElementAt(1).Value.First();

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            homePage.ReserveBike(bikeId);
            var isBikeReserved = homePage.IsBikeReserved(bikeId);
            Assert.IsTrue(isBikeReserved);
            homePage.CancelBikeReservation(bikeId);
        }

        //  8) jako użytkownik mogę zarezerwować/anulować rezerwację rower/u
        //  (+odpowiednie wyświetlanie i automatyczne wygaśnięcie rezerwacji)
        [TestMethod]
        public void CancelBikeReservationTest()
        {
            string bikeId = stationBikes.ElementAt(1).Value.First();

            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            homePage.ReserveBike(bikeId);
            var isBikeReserved = homePage.IsBikeReserved(bikeId);
            Assert.IsTrue(isBikeReserved);
            homePage.CancelBikeReservation(bikeId);
            isBikeReserved = homePage.IsBikeReserved(bikeId);
            Assert.IsFalse(isBikeReserved);
        }


        [TestCleanup]
        public void ChromeDriverCleanup()
        {
            driver.Quit();
        }
    }
}
