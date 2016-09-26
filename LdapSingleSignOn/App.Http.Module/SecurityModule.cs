using System;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace App.Http.Module
{
    public class SecurityModule : IHttpModule
    {
        #region IHttpModule Members
        public void Dispose() { }

        public void Init(HttpApplication application)
        {
            application.AuthenticateRequest += OnAuthenticateRequest;
            application.AuthorizeRequest += OnAuthorizeRequest;
            application.EndRequest += OnEndRequest;
        }
        #endregion

        #region Events
        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];

            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    AuthenticateUser(authHeaderVal.Parameter);
                }
            }
        }

        private void OnAuthorizeRequest(object sender, EventArgs e)
        {
        }

        private void OnEndRequest(object sender, EventArgs eventArgs)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                response.ClearContent();
                response.ContentType = "application/json";
                response.Write("{ Error: 'Unauthorized Access'}");
            }
        }
        #endregion

        #region Utils
        private static void AuthenticateUser(string credentials)
        {
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                var parts = credentials.Split(':');
                string name = parts[0];
                string password = parts[1];

                if (CheckPassword(name, password))
                {
                    var identity = new GenericIdentity(name);
                    SetPrincipal(new GenericPrincipal(identity, null));
                }
                else
                {
                    // Invalid username or password.
                    HttpContext.Current.Response.StatusCode = 401;
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                HttpContext.Current.Response.StatusCode = 401;
            }
        }

        private static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current != null)
                HttpContext.Current.User = principal;
        }

        private static bool CheckPassword(string username, string password)
        {
            SecurityService.Reference.SecurityServiceClient client = new SecurityService.Reference.SecurityServiceClient();
            return client.Authenticate(username, password);
        }

        #endregion
    }
}
