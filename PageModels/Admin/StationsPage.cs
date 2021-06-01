using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using PageModels.Models;

namespace PageModels.Admin
{
	public class StationsPage : PageWithAdminNavigation
	{
		private IReadOnlyCollection<(IWebElement header, IWebElement details)> StationTableRows
		{
			get
			{
				var allRows = driver.FindElements(By.XPath("//table[@id='stations-table']/tbody/tr"));
				var rows = new List<(IWebElement header, IWebElement details)>();

				for (int i = 0; i < allRows.Count; i += 2)
				{
					var row = (allRows[i], allRows[i + 1]);
					rows.Add(row);
				}

				return rows;
			}
		}


		private IWebElement AddStationButton => driver.FindElement(By.Id("add-station-button"));

		private IWebElement AddStationNameField => driver.FindElement(By.Id("station-name-field"));
		private IWebElement AddStationLimitField => driver.FindElement(By.Id("station-limit-field"));

		private IWebElement AddStationConfirmButton =>
			driver.FindElement(By.XPath(
				"//div[@id='add-station-dialog']//span[contains(text(),'Add') and not(ancestor::div[contains(@style,'hidden')])]//parent::button"
			));

		private Dictionary<string, (IWebElement block, IWebElement delete, IWebElement expand)> StationActions =>
			StationTableRows.ToDictionary(
				k => k.header.FindElement(By.XPath("./th")).Text.Replace("Station ", ""),
				v => (
					v.header.FindElement(By.XPath("./td[4]/button")),
					v.header.FindElement(By.XPath("./td[5]/button")),
					v.header.FindElement(By.XPath("./td[1]/button"))
				)
			);

		public StationsPage(IWebDriver driver) : base(driver) {}

		public IReadOnlyList<Station> ListStations()
		{
			var ret = new List<Station>();

			foreach (var row in StationTableRows)
			{
				ret.Add(new Station
				{
					Name = row.header.FindElement(By.XPath("./th")).Text.Replace("Station ", ""),
					Status = row.header.FindElement(By.XPath("./td[2]")).Text,
					ActiveBikesCount = int.Parse(row.header.FindElement(By.XPath("./td[3]")).Text)
				});
			}

			return ret;
		}

		public StationDetails GetStationDetails(string stationName)
		{
			var stationRow = StationTableRows.First(
				r => r.header.FindElement(By.XPath("./th")).Text.Contains(stationName)
			);

			ToggleStationDetailsCollapse(stationName);
			Thread.Sleep(200);

			var bikesLimitSpan = stationRow.details.FindElement(
				By.XPath(".//span[contains(text(), 'Bike limit')]")
			);
			string bikeLimitStr = bikesLimitSpan.Text.Replace("Bike limit: ", "");

			var stationDetails = new StationDetails()
			{
				StationName = stationName,
				BikesLimit = int.Parse(bikeLimitStr),
				RelatedMalfunctions = ListVisibleMalfunctions()
			};

			ToggleStationDetailsCollapse(stationName);
			return stationDetails;
		}

		public void AddStation(string stationName)
		{
			AddStationButton.Click();
			AddStationNameField.SendKeys(stationName);
			AddStationConfirmButton.Click();
		}

		public void AddStation(string stationName, int bikesLimit)
		{
			AddStationButton.Click();
			AddStationNameField.SendKeys(stationName);
			AddStationLimitField.Clear();
			AddStationLimitField.SendKeys(bikesLimit.ToString());
			AddStationConfirmButton.Click();
		}

		public void BlockStation(string stationName)
		{
			var currentUsernamesAndActions = StationActions;
			if (currentUsernamesAndActions[stationName].block.Text == "UNBLOCK")
				throw new ArgumentException("Station already blocked");
			currentUsernamesAndActions[stationName].block.Click();
		}

		public void UnblockStation(string stationName)
		{
			var currentUsernamesAndActions = StationActions;
			if (currentUsernamesAndActions[stationName].block.Text == "BLOCK")
				throw new ArgumentException("Station not blocked");
			currentUsernamesAndActions[stationName].block.Click();
		}

		public void DeleteStation(string stationName)
		{
			StationActions[stationName].delete.Click();
		}

		public bool IsStationBlocked(string stationName)
		{
			var station = ListStations().First(s => s.Name.Equals(stationName));
			return station.Status.Equals("blocked") && StationActions[stationName].block.Text.Equals("UNBLOCK");
		}

		public bool IsStationUnblocked(string stationName)
		{
			var bike = ListStations().First(b => b.Name.Equals(stationName));
			return bike.Status.Equals("active") && StationActions[stationName].block.Text.Equals("BLOCK");
		}

		private List<Malfunction> ListVisibleMalfunctions()
		{
			var malfunctionTableRows = driver.FindElements(By.XPath("//table[@id='malfunctions-table']/tbody/tr"));
			var malfunctions = new List<Malfunction>();

			foreach (var row in malfunctionTableRows)
			{
				var malfunction = new Malfunction()
				{
					Id = row.FindElement(By.XPath("./td[1]")).Text,
					Description = row.FindElement(By.XPath("./td[3]")).Text
				};
				malfunctions.Add(malfunction);
			}

			return malfunctions;
		}

		private void ToggleStationDetailsCollapse(string stationName)
		{
			StationActions[stationName].expand.Click();
		}
	}
}