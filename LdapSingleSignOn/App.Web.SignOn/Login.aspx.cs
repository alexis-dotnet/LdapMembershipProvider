using System;
using System.Web.Security;
using System.Web.UI;

namespace App.Web.SignOn
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                var returnPage = Request.QueryString["ReturnUrl"];

                if (returnPage != null)
                {
                    var parts = returnPage.Split('_');
                    returnPage = parts[0] + "://" + parts[1] + (parts[2][0] == '/' ? parts[2] : ":" + parts[2]);
                }
                else
                    returnPage = "Logged.html";


                var authCookie = Request.Cookies[".ASPXAUTH"];

                if (authCookie != null && authCookie.Value != "")
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    if (authTicket != null && !authTicket.Expired)
                    {
                        FormsAuthenticationTicket newAuthTicket = authTicket;

                        if (FormsAuthentication.SlidingExpiration)
                        {
                            newAuthTicket = FormsAuthentication.RenewTicketIfOld(authTicket);
                        }
                        string userData = newAuthTicket.UserData;
                        string[] roles = userData.Split(',');

                        System.Web.HttpContext.Current.User =
                            new System.Security.Principal.GenericPrincipal(new FormsIdentity(newAuthTicket), roles);

                        FormsAuthentication.SetAuthCookie(User.Identity.Name, true);

                        Response.Redirect(returnPage);
                    }
                }
            }
        }
    }
}