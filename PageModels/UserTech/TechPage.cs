using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PageModels.Models.Backend;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PageModels.UserTech
{
    public class TechPage
    {
        private By userTabBy = By.Id("user-page");
        private IReadOnlyCollection<IWebElement> TechStationCards => driver.FindElements(By.XPath("//div[@id='tech-station-card']"));
        private IReadOnlyCollection<IWebElement> MalfunctionsTableRow => driver.FindElements(By.Id("malfunction-row"));

        private IWebDriver driver;
        public TechPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public HomePage MoveToUserTab()
        {
            driver.FindElement(userTabBy).Click();
            return new HomePage(driver);
        }

        private IDictionary<string, IDictionary<string, IWebElement>> GetVisibleBikes()
        {
            var dict = new Dictionary<string, IDictionary<string, IWebElement>>();

            foreach (var entry in TechStationCards)
            {
                entry.Click();                
            }

            var techStationsCombos = driver.FindElements(By.Id("tech-station"));
            foreach (var combo in techStationsCombos)
            {
                var stations = combo.FindElements(By.XPath(".//div[@id='tech-station-collapse']//li"))
                    .Select(p => new { id = p.FindElement(By.XPath(".//span")).Text, button = p.FindElement(By.XPath(".//button")) }).ToList();
                    
                dict.Add(combo.FindElement(By.XPath(".//div[@id='tech-station-card']//span")).Text, 
                    stations.ToDictionary(k => k.id, v => v.button));
            }

            foreach (var entry in TechStationCards)
            {
                entry.Click();
            }

            return dict;
        }

        public void BlockBike(string id)
        {
            var bikes = GetVisibleBikes();
            foreach (var entry in TechStationCards)
            {
                entry.Click();
            }
            foreach (var value in bikes.Values)
            {
                if(value.TryGetValue(id, out var button))
                {
                    var buttonText = button.Text;
                    if (buttonText.Contains("BLOCK", StringComparison.CurrentCultureIgnoreCase))
                    {
                        button.Click();
                        break;
                    }
                    else
                    {
                        throw new ArgumentException("Bike is not unlocked");
                    }                    
                }
            }

            foreach (var entry in TechStationCards)
            {
                entry.Click();
            }
        }

        public void UnlockBike(string id)
        {
            var bikes = GetVisibleBikes();
            foreach (var entry in TechStationCards)
            {
                entry.Click();
            }
            foreach (var value in bikes.Values)
            {
                if (value.TryGetValue(id, out var button))
                {
                    if (button.Text.Contains("UNBLOCK"))
                    {
                        button.Click();
                    }
                    else
                    {
                        throw new ArgumentException("Bike is not blocked");
                    }
                }
            }

            foreach (var entry in TechStationCards)
            {
                entry.Click();
            }
        }

        public bool IsBikeBlocked(string id)
        {
            var bikes = GetVisibleBikes();
            foreach (var entry in TechStationCards)
            {
                entry.Click();
            }
            foreach (var value in bikes.Values)
            {
                if (value.TryGetValue(id, out var button))
                {
                    if (button.Text.Contains("UNBLOCK"))
                    {
                        foreach (var entry in TechStationCards)
                        {
                            entry.Click();
                        }
                        return true;
                    }
                }
            }

            foreach (var entry in TechStationCards)
            {
                entry.Click();
            }
            return false;
        }

        public IReadOnlyCollection<Malfunction> ListAllMalfunctions()
        {
            return MalfunctionsTableRow.Select(m => new Malfunction
            {
                Id = m.FindElement(By.Id("malfunction-id")).Text,
                BikeId = m.FindElement(By.Id("bike-id")).Text,
                UserId = m.FindElement(By.Id("user-id")).Text,
                Description = m.FindElement(By.Id("malfunction-description")).Text,
            }).ToList();
        }

        public void ApproveMalfunction(string id)
        {
            var denyMalfunctions =
                MalfunctionsTableRow.ToDictionary(k => k.FindElement(By.Id("bike-id")).Text,
                    v => v.FindElement(By.Id("approve-button")));

            denyMalfunctions[id].Click();
        }

        public void DenyMalfunction(string id)
        {
            var denyMalfunctions = 
                MalfunctionsTableRow.ToDictionary(k => k.FindElement(By.Id("bike-id")).Text, 
                    v => v.FindElement(By.Id("deny-button")));

            denyMalfunctions[id].Click();            
        }

        public void FixBike(string id)
        {
            var denyMalfunctions =
                MalfunctionsTableRow.ToDictionary(k => k.FindElement(By.Id("bike-id")).Text,
                    v => v.FindElement(By.Id("fix-button")));

            denyMalfunctions[id].Click();
        }
    }
}
