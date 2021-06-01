﻿using OpenQA.Selenium;
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
        // rented bike list
        private By rentedBikeEntryBy = By.XPath("//ul[@id='rented-bike-list']//li");
        private By rentedBikeIdBy = By.XPath(".//p");
        private By rentedBikeReturnButtonBy = By.XPath(".//button[@id='bike-return-button']");
        private By rentedBikeReportMalfunctionButtonBy = By.XPath(".//button[@id='report-malfunction-button']");

        // station list
        private By stationListBy = By.Id("stations");
        private By stationBy = By.Id("station");

        private By stationBikeRowBy = By.XPath(".//div[contains(@class, 'MuiCollapse-entered')]//li");
        private By stationBikeRowTextBy = By.XPath(".//div[@id='bike-id']/span");
        private By stationBikeRowButtonBy = By.XPath(".//button");

        private By rentDialogYesButtonBy = By.XPath("//span[contains(text(),'Yes') and not(ancestor::div[contains(@style,'hidden')])]//parent::button");
        
        private By returnDialogRowBy = By.XPath("//div[@id='return-bike-dialog-list' and not(ancestor::div[contains(@style,'hidden')])]//li");
        private By returnDialogTextBy = By.XPath(".//span");
        private By returnDialogButtonBy = By.XPath(".//button");

        private By logoutButtonBy = By.Id("logout-topbar-button");

        private By reportMalfunctionDialogReportButtonBy = By.XPath("//span[contains(text(),'Report') and not(ancestor::div[contains(@style,'hidden')])]//parent::button");
        private By reportMalfunctionDialogTextfieldBy = By.XPath("//div[@id='report-malfunction-dialog' and not(contains(@style,'visibility: hidden'))]//textarea[not(contains(@style,'visibility: hidden'))]");

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
                    ret.Add(stationRow.FindElement(stationBikeRowTextBy).Text, (stationRow.FindElement(stationBikeRowButtonBy), stationName));
                }                
            }

            var rentButton = ret[bikeId].rentButton;
            var actions = new Actions(driver);
            actions.MoveToElement(rentButton);
            actions.Perform();
            rentButton.Click();
            var yesButton = driver.FindElement(rentDialogYesButtonBy);            
            actions.MoveToElement(yesButton);
            actions.Perform();
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
                var rentedBikesRows = driver.FindElements(rentedBikeEntryBy);
                var rentedBikesIds = rentedBikesRows.Select(r => r.FindElement(rentedBikeIdBy).Text);

                return rentedBikesIds.Contains(bikeId);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ReturnBike(string bikeId, string stationName)
        {
            var rentedBikesRows = driver.FindElements(rentedBikeEntryBy);
            var bikeReturnButtons = new Dictionary<string, IWebElement>();
            foreach (var row in rentedBikesRows)
            {
                bikeReturnButtons.Add(row.FindElement(rentedBikeIdBy).Text, row.FindElement(rentedBikeReturnButtonBy));
            }
            bikeReturnButtons[bikeId].Click();

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

        public void ReportMalfunction(string bikeId, string description)
        {
            var rentedBikesRows = driver.FindElements(rentedBikeEntryBy);
            var bikeReturnButtons = new Dictionary<string, IWebElement>();
            foreach (var row in rentedBikesRows)
            {
                bikeReturnButtons.Add(row.FindElement(rentedBikeIdBy).Text, row.FindElement(rentedBikeReportMalfunctionButtonBy));
            }
            bikeReturnButtons[bikeId].Click();

            driver.FindElement(reportMalfunctionDialogTextfieldBy).SendKeys(description);
            driver.FindElement(reportMalfunctionDialogReportButtonBy).Click();
        }
    }
}
