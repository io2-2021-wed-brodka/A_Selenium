using System.Linq;
using OpenQA.Selenium;

namespace PageModels.Admin
{
	public class LoginPage
	{
		private IWebDriver driver;
		private By usernameFieldBy = By.Id("username-field");
		private By passwordFieldBy = By.Id("password-field");
		private By loginButtonBy = By.Id("sign-in-button");
		private By createAccountButtonBy = By.Id("create-account-button");

		public LoginPage(IWebDriver driver)
		{
			this.driver = driver;
		}

		public BikesPage LoginValidAdmin(string username, string password)
		{
			driver.FindElement(usernameFieldBy).SendKeys(username);
			driver.FindElement(passwordFieldBy).SendKeys(password);
			driver.FindElement(loginButtonBy).Click();
			return new BikesPage(driver);
		}

		public bool IsLoginFormVisible()
		{
			return driver.FindElements(By.Id("signin-header")).Any();
		}
	}
}