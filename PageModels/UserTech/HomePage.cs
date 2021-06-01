using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PageModels.UserTech
{
    public class HomePage
    {
        private IWebDriver driver;
        // tabs
        private By techTabBy = By.Id("tech-page");

        // rented bike list
        private By rentedBikeEntryBy = By.XPath("//ul[@id='rented-bike-list']//li");
        private By rentedBikeIdBy = By.XPath(".//span");
        private By rentedBikeIdTextBy = By.XPath("//ul[@id='rented-bike-list']//li//span");
        private By rentedBikeReturnButtonBy = By.XPath(".//button[@id='bike-return-button']");
        private By rentedBikeReportMalfunctionButtonBy = By.XPath(".//button[@id='report-malfunction-button']");

        // station list
        private By stationListBy = By.Id("stations");
        private By stationBy = By.Id("station");

        private By stationBikeRowBy = By.XPath(".//div[contains(@class, 'MuiCollapse-entered')]//li");
        private By stationBikeRowTextBy = By.XPath(".//div[@id='bike-id']/span");
        private By stationBikeRowRentButtonBy = By.XPath(".//button[@id='rent-button']");
        private By stationBikeRowBookButtonBy = By.XPath(".//button[@id='reservation-button']");

        private By rentDialogYesButtonBy = By.XPath("//span[contains(text(),'Yes') and not(ancestor::div[contains(@style,'hidden')])]//parent::button");
        
        private By returnDialogRowBy = By.XPath("//div[@id='return-bike-dialog-list' and not(ancestor::div[contains(@style,'hidden')])]//li");
        private By returnDialogTextBy = By.XPath(".//span");
        private By returnDialogButtonBy = By.XPath(".//button");

        private By logoutButtonBy = By.Id("logout-topbar-button");

        private By reportMalfunctionDialogReportButtonBy = By.XPath("//span[contains(text(),'Report') and not(ancestor::div[contains(@style,'hidden')])]//parent::button");
        private By reportMalfunctionDialogTextfieldBy = By.XPath("//*[not(contains(@style,'visibility: hidden'))]//textarea[not(contains(@style,'visibility: hidden'))]");

        private By welcomeTextBy = By.Id("welcome-text");        

        public HomePage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void ClickStations()
        {
            var element = driver.FindElement(stationListBy);
            var stations = element.FindElements(stationBy);
            foreach (var station in stations)
            {
                station.Click();
            }
        }

        public int GetStationCount()
        {
            return driver.FindElements(stationBy).Count;
        }

        public bool IsWelcomeTextVisible()
        {
            return driver.FindElements(welcomeTextBy).Any();
        }

        public Dictionary<string, ISet<string>> ListBikesOnStations()
        {
            var ret = new Dictionary<string, ISet<string>>();
            var element = driver.FindElement(stationListBy);
            var stations = element.FindElements(stationBy);
            foreach (var station in stations)
            {
                var name = station.Text;
                station.Click();
                ret.Add(name, station.FindElements(stationBikeRowBy).Select(e => e.FindElement(stationBikeRowTextBy).Text).ToHashSet());
                station.Click();
            }

            return ret;            
        }

        public string RentBike(string bikeId)
        {            
            var ret = new Dictionary<string, (IWebElement rentButton, string stationName)>();
            var element = driver.FindElement(stationListBy);
            var stations = element.FindElements(stationBy);
            foreach (var station in stations)
            {
                var stationName = station.Text;                
                station.Click();
                foreach (var stationRow in station.FindElements(stationBikeRowBy))
                {
                    ret.Add(stationRow.FindElement(stationBikeRowTextBy).Text, (stationRow.FindElement(stationBikeRowRentButtonBy), stationName));
                }                
            }

            var rentButton = ret[bikeId].rentButton;                        
            rentButton.Click();

            var yesButton = driver.FindElement(rentDialogYesButtonBy);                                   
            yesButton.Click();

            foreach (var station in stations)
            {
                station.Click();             
            }
            return ret[bikeId].stationName;
        }

        public bool IsBikeRented(string bikeId)
        {
            try
            {
                var rentedBikesIds = driver.FindElements(rentedBikeIdTextBy).Select(r => r.Text);
                return rentedBikesIds.Contains(bikeId);
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ReturnBike(string bikeId, string stationName)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var rentedBikesRows = driver.FindElements(rentedBikeEntryBy);
                    var bikeReturnButtons = new Dictionary<string, IWebElement>();
                    foreach (var row in rentedBikesRows)
                    {
                        bikeReturnButtons.Add(row.FindElement(rentedBikeIdBy).Text, row.FindElement(rentedBikeReturnButtonBy));
                    }
                    bikeReturnButtons[bikeId].Click();
                    break;
                }
                catch (StaleElementReferenceException)
                {
                    continue;
                }
            }
            var returnStations = new Dictionary<string, IWebElement>();
            var returnBikeRows = driver.FindElements(returnDialogRowBy);
            foreach (var row in returnBikeRows)
            {
                returnStations.Add(row.FindElement(returnDialogTextBy).Text, row.FindElement(returnDialogButtonBy));
            }
            Thread.Sleep(500);
            returnStations[stationName].Click();            
        }

        public LoginPage LogOut()
        {
            driver.FindElement(logoutButtonBy).Click();
            return new LoginPage(driver);
        }

        public void ReportMalfunction(string bikeId, string description, string stationName)
        {
            var rentedBikesRows = driver.FindElements(rentedBikeEntryBy);
            var bikeReturnButtons = new Dictionary<string, IWebElement>();
            foreach (var row in rentedBikesRows)
            {
                bikeReturnButtons.Add(row.FindElement(rentedBikeIdBy).Text, row.FindElement(rentedBikeReportMalfunctionButtonBy));
            }
            bikeReturnButtons[bikeId].Click();

            driver.FindElement(reportMalfunctionDialogTextfieldBy).SendKeys(description);

            var returnStations = new Dictionary<string, IWebElement>();
            var returnBikeRows = driver.FindElements(returnDialogRowBy);
            foreach (var row in returnBikeRows)
            {
                returnStations.Add(row.FindElement(returnDialogTextBy).Text, row.FindElement(returnDialogButtonBy));
            }
            Thread.Sleep(500);
            returnStations[stationName].Click();
        }

        public TechPage MoveToTechTab()
        {
            driver.FindElement(techTabBy).Click();
            return new TechPage(driver);
        }

        public void ReserveBike(string bikeId)
        {
            var ret = new Dictionary<string, (IWebElement reserveButton, string stationName)>();
            var element = driver.FindElement(stationListBy);
            var stations = element.FindElements(stationBy);
            foreach (var station in stations)
            {
                var stationName = station.Text;
                station.Click();
                foreach (var stationRow in station.FindElements(stationBikeRowBy))
                {
                    ret.Add(stationRow.FindElement(stationBikeRowTextBy).Text, (stationRow.FindElement(stationBikeRowBookButtonBy), stationName));
                }
            }

            ret[bikeId].reserveButton.Click();            
        }

        public void CancelBikeReservation(string bikeId)
        {

        }

        public void RentReservedBike(string bikeId)
        {

        }
    }
}
