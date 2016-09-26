using System;
using System.Web.Security;

namespace App.Web.SignOn
{
    public partial class LoginProcess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SetAuthCookie(User.Identity.Name, true);
            Response.ClearContent();
            Response.ContentType = "application/json";

            var responseCookie = Response.Cookies[0];
            if (responseCookie != null)
                Response.Write("{ Value: \"" + responseCookie.Value + "\" }");
            else
                Response.Write("{ Value: \"\" }");
        }
    }
}