using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace PageModels.Admin
{
    public class TechsPage : PageWithAdminNavigation
    {
        private IReadOnlyCollection<IWebElement> TechTableRows => driver.FindElements(By.XPath("//table[@id='techs-table']/tbody/tr"));

        private IWebElement AddTechButton => driver.FindElement(By.Id("add-tech-button"));
        private IWebElement AddTechUsernameField => driver.FindElement(By.Id("tech-username-field"));
        private IWebElement AddTechPasswordField => driver.FindElement(By.Id("tech-password-field"));
        private IWebElement AddTechRepasswordField => driver.FindElement(By.Id("tech-repassword-field"));
        private IWebElement AddTechConfirmButton =>
            driver.FindElement(By.XPath("//div[@id='add-tech-dialog']//span[contains(text(),'Add') and not(ancestor::div[contains(@style,'hidden')])]//parent::button"));

        private Dictionary<string, IWebElement> TechUsernameDelete =>
            driver.FindElements(By.XPath("//table[@id='techs-table']/tbody/tr")).ToDictionary(k => k.FindElement(By.XPath("./td[1]")).Text, v => v.FindElement(By.XPath("./td[2]/button")));

        public TechsPage(IWebDriver driver) : base(driver) { }
                
        public IReadOnlyList<string> ListTechs()
        {
            var ret = new List<string>();

            foreach (var row in TechTableRows)
            {
                ret.Add(row.FindElement(By.XPath("./td[1]")).Text);
            }

            return ret;
        }

        public void AddTech(string username, string password, string repassword)
        {
            AddTechButton.Click();
            AddTechUsernameField.SendKeys(username);
            AddTechPasswordField.SendKeys(password);
            AddTechRepasswordField.SendKeys(repassword);

            AddTechConfirmButton.Click();            
        }

        public void DeleteTech(string username)
        {
            TechUsernameDelete[username].Click();            
        }
    }
}
