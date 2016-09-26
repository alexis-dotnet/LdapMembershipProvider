using System.Web.Security;

namespace App.Service
{
    public class SecurityService : ISecurityService
    {
        public bool Authenticate(string username, string password)
        {
            return Membership.ValidateUser(username, password);
        }
    }
}
