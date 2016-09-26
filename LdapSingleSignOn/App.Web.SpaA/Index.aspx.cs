using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using App.Web.SpaA.LdapService.Reference;

namespace App.Web.SpaA
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LdapServiceClient client = new LdapServiceClient();

            var users = client.GetAllUsers();
            var list = new List<object>();

            foreach (var user in users)
            {
                list.Add(new
                {
                    user.FirstName,
                    user.Email,
                    user.Fax,
                    user.FullName,
                    user.Id,
                    user.LastName,
                    user.Location,
                    user.Phone,
                    user.RoomNumber,
                    user.Username,
                    Role1 = user.Roles != null && user.Roles.Length >= 1 ? user.Roles[0] : null,
                    Role2 = user.Roles != null && user.Roles.Length >= 2 ? user.Roles[1] : null
                });
            }

            rptData.DataSource = list;
            rptData.DataBind();
        }

        public void OnLogoutClick(object sender, EventArgs args)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            // ".ASPXAUTH"
            HttpCookie cookie3 = new HttpCookie(".ASPXAUTH", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie3);

            FormsAuthentication.RedirectToLoginPage();
        }
    }
}