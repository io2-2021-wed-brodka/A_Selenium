using OpenQA.Selenium;

namespace PageModels.Admin
{
	public class PageWithAdminNavigation
	{
		protected IWebDriver driver;
		protected IWebElement NavigationDrawerBikes => driver.FindElement(By.Id("bikes-navigation"));
		protected IWebElement NavigationDrawerTechs => driver.FindElement(By.Id("techs-navigation"));
		protected IWebElement NavigationDrawerStations => driver.FindElement(By.Id("stations-navigation"));
		protected IWebElement NavigationDrawerUsers => driver.FindElement(By.Id("user-navigation"));

		protected PageWithAdminNavigation(IWebDriver driver)
		{
			this.driver = driver;
		}

		public BikesPage GoToBikePage()
		{
			NavigationDrawerBikes.Click();
			return new BikesPage(driver);
		}

		public StationsPage GoToStationsPage()
		{
			NavigationDrawerStations.Click();
			return new StationsPage(driver);
		}

		public TechsPage GoToTechsPage()
		{
			NavigationDrawerTechs.Click();
			return new TechsPage(driver);
		}

		public UsersPage GoToUsersPage()
		{
			NavigationDrawerUsers.Click();
			return new UsersPage(driver);
		}

		public LoginPage Logout()
		{
			driver.FindElement(By.Id("logout-button")).Click();
			return new LoginPage(driver);
		}
	}
}