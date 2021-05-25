using PageModels.UserTech;
using OpenQA.Selenium;
using System.Linq;

namespace PageModels.UserTech
{
    public class LoginPage
    {
        private IWebDriver driver;
        private By usernameFieldBy = By.Id("username-field");
        private By passwordFieldBy = By.Id("password-field");
        private By loginButtonBy = By.Id("sign-in-button");
        private By createAccountButtonBy = By.Id("create-account-button");

        private By welcomeTextBy = By.Id("welcome-text");

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void loginInvalidUser(string username, string password)
        {
            driver.FindElement(usernameFieldBy).SendKeys(username);
            driver.FindElement(passwordFieldBy).SendKeys(password);
            driver.FindElement(loginButtonBy).Click();            
        }

        public HomePage loginValidUser(string username, string password)
        {
            driver.FindElement(usernameFieldBy).SendKeys(username);
            driver.FindElement(passwordFieldBy).SendKeys(password);
            driver.FindElement(loginButtonBy).Click();
            return new HomePage(driver);
        }

        public RegisterPage GoToCreateAccountPage()
        {
            driver.FindElement(createAccountButtonBy).Click();
            return new RegisterPage(driver);
        }

        public bool IsWelcomeTextVisible()
        {
            return driver.FindElements(welcomeTextBy).Any();
        }
    }

    
}
