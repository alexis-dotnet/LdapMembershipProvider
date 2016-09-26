using System;
using System.Web.Security;

namespace App.Web.SpaA
{
    public partial class LoginRedirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                var path = Request.Url.AbsoluteUri;
                var returnPage = Request.QueryString["ReturnUrl"];

                path = path.Substring(0, path.IndexOf("LoginRedirect.aspx", StringComparison.Ordinal) - 1);

                var protocol = path.Substring(0, path.IndexOf("://", StringComparison.Ordinal));
                var domain = path.Substring(path.IndexOf("://", StringComparison.Ordinal) + 3).Split(':');

                string extPath;
                if (domain.Length == 1)
                    extPath = string.Format("{0}_{1}_", protocol, domain[0]);
                else
                    extPath = string.Format("{0}_{1}_{2}", protocol, domain[0], domain[1]);


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

                        Response.Redirect(Request.QueryString["ReturnUrl"]);
                    }
                }

                Response.Redirect("http://localhost:57369/Login.aspx?ReturnUrl=" + Server.UrlEncode(extPath + returnPage));
            }
        }
    }
}