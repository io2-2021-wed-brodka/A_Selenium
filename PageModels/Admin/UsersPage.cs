using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace PageModels.Admin
{
	public class UsersPage : PageWithAdminNavigation
	{
		private IReadOnlyCollection<IWebElement> UserTableRows =>
			driver.FindElements(By.XPath("//table[@id='users-table']/tbody/tr"));

		private Dictionary<string, IWebElement> UserActionButtons =>
			driver.FindElements(By.XPath("//table[@id='users-table']/tbody/tr")).ToDictionary(
				k => k.FindElement(By.XPath("./td[1]")).Text, v => v.FindElement(By.XPath("./td[2]/button")));

		public UsersPage(IWebDriver driver) : base(driver) {}

		public IReadOnlyList<string> ListUsers()
		{
			var ret = new List<string>();

			foreach (var row in UserTableRows)
			{
				ret.Add(row.FindElement(By.XPath("./td[1]")).Text);
			}

			return ret;
		}

		public void BlockUser(string username)
		{
			var currentUsernamesAndActions = UserActionButtons;
			if (currentUsernamesAndActions[username].Text == "UNBLOCK")
				throw new ArgumentException("User already blocked");
			currentUsernamesAndActions[username].Click();
		}

		public void UnblockUser(string username)
		{
			var currentUsernamesAndActions = UserActionButtons;
			if (currentUsernamesAndActions[username].Text == "BLOCK")
				throw new ArgumentException("User not blocked");
			currentUsernamesAndActions[username].Click();
		}

		public bool IsUserBlocked(string username)
		{
			string text = UserActionButtons[username].Text;
			return UserActionButtons[username].Text.Equals("UNBLOCK");
		}
	}
}