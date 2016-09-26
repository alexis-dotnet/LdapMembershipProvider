using System;
using System.Web;
using System.Web.Security;
using App.Web.SpaB.LdapService.Reference;

namespace App.Web.SpaB
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LdapServiceClient client = new LdapServiceClient();

            var user = client.GetUser(User.Identity.Name);

            txtUsername.Text = User.Identity.Name;
            txtFullName.Text = user.FullName;
            txtFax.Text = user.Fax;
            txtPhone.Text = user.Phone;
            txtRoom.Text = user.RoomNumber.ToString();
            txtEmail.Text = user.Email;
            txtLocation.Text = user.Location;
            txtId.Text = user.Id.ToString();
            rptRoles.DataSource = user.Roles;
            rptRoles.DataBind();
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