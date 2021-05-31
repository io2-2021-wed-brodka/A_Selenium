using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using PageModels.Models;

namespace PageModels.Admin
{
	public class StationsPage : PageWithAdminNavigation
	{
		private IReadOnlyCollection<IWebElement> StationTableRows
		{
			get
			{
				var allRows = driver.FindElements(By.XPath("//table[@id='stations-table']/tbody/tr"));
				var mainRows = new List<IWebElement>();
				
				for (int i = 0; i < allRows.Count; i++)
				{
					if (i % 2 == 0)
						mainRows.Add(allRows[i]);
				}

				return mainRows;
			}
		}


		private IWebElement AddStationButton => driver.FindElement(By.Id("add-station-button"));

		private IWebElement AddStationConfirmButton =>
			driver.FindElement(By.XPath(
				"//div[@id='add-station-dialog']//span[contains(text(),'Add') and not(ancestor::div[contains(@style,'hidden')])]//parent::button"
			));

		private Dictionary<string, (IWebElement block, IWebElement delete)> StationBlockDelete =>
			StationTableRows.ToDictionary(
				k => k.FindElement(By.XPath("./th")).Text.Replace("Station ", ""),
				v => (v.FindElement(By.XPath("./td[4]/button")), v.FindElement(By.XPath("./td[5]/button")))
			);

		public StationsPage(IWebDriver driver) : base(driver) {}

		public IReadOnlyList<Station> ListStations()
		{
			var ret = new List<Station>();

			foreach (var row in StationTableRows)
			{
				var name = row.FindElement(By.XPath("./th")).Text.Replace("Station ", "");
				var status = row.FindElement(By.XPath("./td[2]")).Text;
				int count = int.Parse(row.FindElement(By.XPath("./td[3]")).Text);

				ret.Add(new Station
				{
					Name = row.FindElement(By.XPath("./th")).Text.Replace("Station ", ""),
					Status = row.FindElement(By.XPath("./td[2]")).Text,
					ActiveBikesCount = int.Parse(row.FindElement(By.XPath("./td[3]")).Text)
				});
			}

			return ret;
		}

		public void AddStation(string stationName)
		{
			AddStationButton.Click();
			AddStationConfirmButton.Click();
		}

		public void BlockStation(string stationName)
		{
			var currentUsernamesAndActions = StationBlockDelete;
			if (currentUsernamesAndActions[stationName].block.Text == "UNBLOCK")
				throw new ArgumentException("Station already blocked");
			currentUsernamesAndActions[stationName].block.Click();
		}

		public void UnblockStation(string stationName)
		{
			var currentUsernamesAndActions = StationBlockDelete;
			if (currentUsernamesAndActions[stationName].block.Text == "BLOCK")
				throw new ArgumentException("Station not blocked");
			currentUsernamesAndActions[stationName].block.Click();
		}

		public void DeleteStation(string stationName)
		{
			StationBlockDelete[stationName].delete.Click();
		}

		public bool IsStationBlocked(string stationName)
		{
			var station = ListStations().First(s => s.Name.Equals(stationName));
			return station.Status.Equals("blocked") && StationBlockDelete[stationName].block.Text.Equals("UNBLOCK");
		}

		public bool IsStationUnblocked(string stationName)
		{
			var bike = ListStations().First(b => b.Name.Equals(stationName));
			return bike.Status.Equals("active") && StationBlockDelete[stationName].block.Text.Equals("BLOCK");
		}
	}
}