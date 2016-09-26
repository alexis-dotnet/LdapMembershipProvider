using App.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace _App.UnitTest
{
    [TestClass]
    public class LdapProviderUnitTest
    {
        [TestMethod]
        public void TestGetUserNameByEmail()
        {
            var provider = GetInitializedProvider();
            var username = provider.GetUserNameByEmail("ashelton@example.com");
            Assert.AreEqual(username, "ashelton");

            int count;
            var users = provider.FindUsersByEmail("ashelton@example.com", 1, 1, out count);
            Assert.IsNotNull(users["ashelton"]);
        }

        [TestMethod]
        public void TestChangePassword()
        {
            var provider = GetInitializedProvider();
            var newPwd = provider.ResetPassword("ashelton", "");

            provider.ChangePassword("ashelton", newPwd, "ldap123");
            Assert.AreEqual(provider.GetPassword("ashelton", ""), "ldap123");

            provider.ChangePassword("ashelton", "ldap123", "appointe");
            Assert.AreEqual(provider.GetPassword("ashelton", ""), "appointe");

            Assert.IsTrue(provider.ChangePasswordQuestionAndAnswer("ashelton", "appointe", "my first mascot", "chini"));
            Assert.IsTrue(provider.ChangePasswordQuestionAndAnswer("ashelton", "appointe", "", ""));
        }

        [TestMethod]
        public void TestValidationSuccess()
        {
            var provider = GetInitializedProvider();
            Assert.IsTrue(provider.ValidateUser("ashelton", "appointe"));
        }

        [TestMethod]
        public void TestValidationError()
        {
            var provider = GetInitializedProvider();
            Assert.IsFalse(provider.ValidateUser("ashelton", "12345"));
        }

        [TestMethod]
        public void TestGetUser()
        {
            var provider = GetInitializedProvider();
            var user = (LdapMembershipUser) provider.GetUser("ashelton", true);

            if (user != null)
            {
                Assert.AreEqual("Alexander", user.FirstName);
                Assert.AreEqual("Shelton", user.LastName);
                Assert.AreEqual("+1 408 555 7472", user.Fax);
                Assert.AreEqual("+1 408 555 1081", user.Phone);
                Assert.AreEqual(1987, user.RoomNumber);
                Assert.AreEqual("ashelton@example.com", user.Email);
                Assert.AreEqual("Santa Clara", user.Location);
                Assert.AreEqual(1060, user.ProviderUserKey);
            }

            Assert.IsNotNull(user);

            var msg = "casting all your anxieties on him, becasuse he cares for you";
            user.Comment = msg;
            provider.UpdateUser(user);

            user = (LdapMembershipUser)provider.GetUser("ashelton", true);

            if (user != null)
                Assert.AreEqual(msg, user.Comment);
        }

        [TestMethod]
        public void TestGetAllUsers()
        {
            var provider = GetInitializedProvider();
            int counter;
            var users = provider.GetAllUsers(1, 200, out counter);

            Assert.AreEqual(counter, 150);
            Assert.IsNotNull(users);
        }

        private LdapMembershipProvider GetInitializedProvider()
        {
            var provider = new LdapMembershipProvider();
            var parameters = new NameValueCollection
            {
                {"applicationName", "/" },
                {"ldapServer", "localhost"},
                {"ldapPort", "1234"},
                {"domain", "example.com"}
            };


            provider.Initialize("LdapMembershipProvider", parameters);
            return provider;
        }

        [TestMethod]
        public void TestUsersOnLine()
        {
            var provider = GetInitializedProvider();
            Assert.IsTrue(provider.ValidateUser("ashelton", "appointe"));
            var count = provider.GetNumberOfUsersOnline();
            Assert.IsNotNull(count);
        }
    }
}
