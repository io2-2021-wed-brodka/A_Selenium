using OpenQA.Selenium;

namespace PageModels.UserTech
{
    public class RegisterPage
    {
        private IWebDriver driver;
        private By usernameFieldBy = By.Id("username-register-field");
        private By passwordFieldBy = By.Id("password-register-field");
        private By repasswordFieldBy = By.Id("repassword-field");
        private By createAccountButtonBy = By.Id("create-account-register-button");

        public RegisterPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public LoginPage createUser(string username, string password)
        {
            driver.FindElement(usernameFieldBy).SendKeys(username);
            driver.FindElement(passwordFieldBy).SendKeys(password);
            driver.FindElement(repasswordFieldBy).SendKeys(password);
            driver.FindElement(createAccountButtonBy).Click();
            return new LoginPage(driver);
        }
    }
}
