using OpenQA.Selenium;

namespace IOTests.Admin
{
    class PageWithAdminNavigation
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
            return new BikesPage(driver);
        }

        public StationsPage GoToStationsPage()
        {
            return new StationsPage(driver);
        }

        public TechsPage GoToTechsPage()
        {
            return new TechsPage(driver);
        }

        public UsersPage GoToUsersPage()
        {
            return new UsersPage(driver);
        }
    }
}
