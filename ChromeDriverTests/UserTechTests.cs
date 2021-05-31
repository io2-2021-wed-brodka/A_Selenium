using ChromeDriverTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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

        private static string backendUrl = "http://localhost:8080/";
        private static string userTechUrl = "";        

        private static string userUsername = "sampleUserTeamA1";
        private static string userPassword = "sampleUserPassword";

        private static string adminUsername = "admin";
        private static string adminPassword = "admin";

        private static readonly Dictionary<string, ISet<string>> stationBikes = new Dictionary<string, ISet<string>>();

        //[AssemblyInitialize]
        //public static async Task HttpDriverInitializeAsync(TestContext context)
        //{
            // var databaseSetup = new SetupMethods(backendUrl);
            //
            // await databaseSetup.RegisterUser(userUsername, userPassword);
            // await databaseSetup.LoginAdmin(adminUsername, adminPassword);
            // var stations = await databaseSetup.AddStation(new[]{
            //     "TestStation1 TeamA",
            //     "TestStation2 TeamA",
            //     "TestStation3 TeamA",
            //     "TestStation4 TeamA",
            //     "TestStation5 TeamA",
            // });
            //
            // foreach (var station in stations)
            // {
            //     stationBikes.Add(station.Name, new HashSet<string>());
            //     for (int i = 0; i < 2; i++)
            //     {
            //         stationBikes[station.Name].Add((await databaseSetup.AddBike(station.Id)).Id);
            //     }
            // }
            //
            // Console.WriteLine("Initiated!");
        //}

        [TestInitialize]
        public void ChromeDriverInitialize()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            driver.Navigate().GoToUrl( "http://localhost:3000/" );
        }


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

        [TestMethod]
        public void StationsVisibleTest()
        {
            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            var stationsCount = homePage.GetStationCount();

            Assert.IsTrue(stationsCount > 0);
        }

        [TestMethod]
        public void LoginInvalidUserTest()
        {
            var loginPage = new LoginPage(driver);
            loginPage.loginValidUser("invalid", "user");

            var isWelcomeTextVisible = loginPage.IsWelcomeTextVisible();

            Assert.IsFalse(isWelcomeTextVisible);
        }

        [TestMethod]
        public void LogoutTest()
        {
            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            loginPage = homePage.LogOut();

            var isWelcomeTextVisible = loginPage.IsWelcomeTextVisible();

            Assert.IsFalse(isWelcomeTextVisible);
        }

        [TestMethod]
        public void ListBikeOnStations()
        {
            var loginPage = new LoginPage(driver);
            var homePage = loginPage.loginValidUser(userUsername, userPassword);
            var stationBikesUser = homePage.ListBikesOnStations();
            
            bool dictionariesEqual = true;

            Assert.IsTrue(stationBikes.Keys.ToHashSet().SetEquals(stationBikesUser.Keys.ToHashSet()));

            foreach (var (key, value) in stationBikes)
            {
                if(stationBikesUser.TryGetValue(key, out var bikesSet))
                {
                    if(!bikesSet.SetEquals(value))
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

        [TestCleanup]
        public void ChromeDriverCleanup()
        {
            driver.Quit();
        }
    }
}
