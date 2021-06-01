using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IOTests.Models;
using OpenQA.Selenium;

namespace PageModels.Admin
{
	public class BikesPage : PageWithAdminNavigation
	{
		private IReadOnlyCollection<IWebElement> BikeTableRows =>
			driver.FindElements(By.XPath("//table[@id='bikes-table']/tbody/tr"));

		private IWebElement AddBikeButton => driver.FindElement(By.Id("add-bike-button"));
		private IWebElement AddBikeStationSelect => driver.FindElement(By.Id("station-select"));

		private ILookup<string, IWebElement> AddBikeSelectableStations =>
			driver.FindElements(By.XPath("//li[@id='selectable-station']")).ToLookup(e => e.Text);

		private IWebElement AddBikeConfirmButton =>
			driver.FindElement(By.XPath(
				"//div[@id='add-bike-dialog']//span[contains(text(),'Add') and not(ancestor::div[contains(@style,'hidden')])]//parent::button"));

		private Dictionary<string, (IWebElement block, IWebElement delete)> BikeBlockDelete =>
			driver.FindElements(By.XPath("//table[@id='bikes-table']/tbody/tr")).ToDictionary(
				k => k.FindElement(By.XPath("./th")).Text.Replace("Bike ", ""),
				v => (v.FindElement(By.XPath("./td[4]/button")), v.FindElement(By.XPath("./td[5]/button"))));

		public BikesPage(IWebDriver driver) : base(driver) {}

		public IReadOnlyList<Bike> ListBikes()
		{
			var ret = new List<Bike>();

			foreach (var row in BikeTableRows)
			{
				ret.Add(new Bike
				{
					BikeName = row.FindElement(By.XPath("./th")).Text,
					StationName = row.FindElement(By.XPath("./td[1]")).Text,
					UserName = row.FindElement(By.XPath("./td[2]")).Text,
					Status = row.FindElement(By.XPath("./td[3]")).Text,
				});
			}

			return ret;
		}

		public void AddBike(string stationName)
		{
			AddBikeButton.Click();
			AddBikeStationSelect.Click();
			Thread.Sleep(200);
			AddBikeSelectableStations[stationName].First().Click();
			AddBikeConfirmButton.Click();
		}

		public void BlockBike(string bikeId)
		{
			var currentUsernamesAndActions = BikeBlockDelete;
			if (currentUsernamesAndActions[bikeId].block.Text == "UNBLOCK")
				throw new ArgumentException("Bike already blocked");
			currentUsernamesAndActions[bikeId].block.Click();
		}

		public void UnblockBike(string bikeId)
		{
			var currentUsernamesAndActions = BikeBlockDelete;
			if (currentUsernamesAndActions[bikeId].block.Text == "BLOCK")
				throw new ArgumentException("Bike not blocked");
			currentUsernamesAndActions[bikeId].block.Click();
		}

		public void DeleteBike(string bikeId)
		{
			BikeBlockDelete[bikeId].delete.Click();
		}

		public bool IsBikesListVisible()
		{
			return driver.FindElements(By.Id("bikes-table")).Any();
		}

		public bool IsBikeBlocked(string bikeId)
		{
			var bike = ListBikes().First(b => b.BikeName.Equals($"Bike {bikeId}"));
			return bike.Status.Equals("blocked") && BikeBlockDelete[bikeId].block.Text.Equals("UNBLOCK");
		}

		public bool IsBikeUnblocked(string bikeId)
		{
			var bike = ListBikes().First(b => b.BikeName.Equals($"Bike {bikeId}"));
			return bike.Status.Equals("available") && BikeBlockDelete[bikeId].block.Text.Equals("BLOCK");
		}
	}
}