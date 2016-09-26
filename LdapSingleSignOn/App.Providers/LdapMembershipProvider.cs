using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace App.Providers
{
    public class LdapMembershipProvider : MembershipProvider
    {
        [Serializable]
        public class UserAdditionalInfo
        {
            public string ApplicationName { get; set; }
            public string PasswordQuestion { get; set; }
            public string PasswordAnswer { get; set; }
            public bool IsApproved { get; set; }
            public DateTime LastLoginDate { get; set; }
            public DateTime LastActivityDate { get; set; }
            public DateTime LastPasswordChangedDate { get; set; }
            public bool IsOnline { get; set; }
            public bool IsLockedOut { get; set; }
            public DateTime LastLockedOutDate { get; set; }
            public int FailedPasswordAttemptCount { get; set; }
            public DateTime FailedPasswordAttemptWindowStart { get; set; }
            public int FailedPasswordAnswerAttemptCount { get; set; }
            public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }
            public bool IsSubscriber { get; set; }
            public string Comment { get; set; }
        }

        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        // Constant values
        private const int NewPasswordLength = 8;
        private const string EventSource = "LdapMembershipProvider";
        private const string EventLog = "Application";
        private const string ProviderDescription = "UnboundId LDAP Membership Provider";
        private const string ExceptionMessage = "An exception occurred. Please check the Event Log.";

        // Holding MembershipProvider variables
        private int _pMaxInvalidPasswordAttempts;
        private int _pPasswordAttemptWindow;
        private int _pMinRequiredNonAlphanumericCharacters;
        private int _pMinRequiredPasswordLength;
        private bool _pEnablePasswordReset;
        private bool _pEnablePasswordRetrieval;
        private bool _pRequiresQuestionAndAnswer;
        private bool _pRequiresUniqueEmail;
        private string _pPasswordStrengthRegularExpression;
        private MembershipPasswordFormat _pPasswordFormat;

        // LDAP specific variables
        private string[] Domain { get; set; }
        private string LdapServerPath { get; set; }
        private int LdapServerPort { get; set; }
        private bool WriteExceptionsToEventLog { get; set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrEmpty(name))
                name = EventSource;

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", ProviderDescription);
            }

            base.Initialize(name, config);

            // Initializing general MembershipProvider variables
            ApplicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            _pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            _pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            _pPasswordStrengthRegularExpression = "";
            _pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            _pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            _pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            _pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            _pRequiresUniqueEmail = true;
            _pPasswordFormat = MembershipPasswordFormat.Clear;

            // Initializing LDAP specific variables
            var entireDomain = GetConfigValue(config["domain"], "example.com");
            Domain = entireDomain.Split('.');
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "false"));
            LdapServerPath = GetConfigValue(config["ldapServer"], "localhost");
            LdapServerPort = Convert.ToInt32(GetConfigValue(config["ldapPort"], "389"));
        }

        #region Inherited Members

        // Intentionally not implemented, I need to find the LDAP query for UnboundID. Some help is welcomed.
        public override MembershipUser CreateUser(string username, string password, string email,
            string passwordQuestion, string passwordAnswer,
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        // Intentionally not implemented, I need to find the LDAP query for UnboundID. Some help is welcomed.
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string pwd, string newPwdQuestion, string newPwdAnswer)
        {
            if (!ValidateUser(username, pwd))
                return false;

            var user = (LdapMembershipUser)GetLocalUser(username);

            if (user != null)
            {
                var uAdd = user.AdditionalInfo;

                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    uAdd.PasswordQuestion = newPwdQuestion;
                    uAdd.PasswordAnswer = newPwdAnswer;
                    var request = new ModifyRequest($"uid={username},ou=People,dc=example,dc=com", DirectoryAttributeOperation.Replace, "description", _serializer.Serialize(uAdd));
                    con.SendRequest(request);

                    return true;
                }
            }

            return false;
        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
                throw new ProviderException("Password Retrieval Not Enabled.");

            var user = (LdapMembershipUser)GetLocalUser(username, true);

            if (user != null)
            {
                if (user.AdditionalInfo.IsLockedOut)
                    throw new MembershipPasswordException("The supplied user is locked out.");

                if (RequiresQuestionAndAnswer && (answer != user.AdditionalInfo.PasswordAnswer))
                {
                    UpdateFailureCount(username, "passwordAnswer");
                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                return user.Password;
            }
            return null;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;

            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
            {
                con.AuthType = AuthType.Anonymous;
                con.SessionOptions.ProtocolVersion = 3;

                var request = new ModifyRequest($"uid={username},ou=People,dc=example,dc=com", DirectoryAttributeOperation.Replace, "userpassword", newPassword);
                var response = (ModifyResponse)con.SendRequest(request);

                if (response != null && response.ResultCode == ResultCode.Success)
                {
                    var user = (LdapMembershipUser)GetLocalUser(username);

                    if (user != null)
                    {
                        var uAdd = user.AdditionalInfo;

                        uAdd.LastPasswordChangedDate = DateTime.Now;
                        request = new ModifyRequest($"uid={username},ou=People,dc=example,dc=com", DirectoryAttributeOperation.Replace, "description", _serializer.Serialize(uAdd));
                        con.SendRequest(request);

                        return true;
                    }
                }

                return false;
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
                throw new NotSupportedException("Password reset is not enabled.");

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new ProviderException("Password answer required for password reset.");
            }

            string newPassword = Membership.GeneratePassword(NewPasswordLength, MinRequiredNonAlphanumericCharacters);
            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            var user = (LdapMembershipUser)GetLocalUser(username);

            if (user != null)
            {
                if (user.AdditionalInfo.IsLockedOut)
                    throw new MembershipPasswordException("The supplied user is locked out.");
            }
            else
                throw new MembershipPasswordException("The supplied user name is not found.");

            var passwordAnswer = user.AdditionalInfo.PasswordAnswer;

            if (RequiresQuestionAndAnswer && (answer != passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new MembershipPasswordException("Incorrect password answer.");
            }

            user.AdditionalInfo.LastPasswordChangedDate = DateTime.Now;

            var req1 = new ModifyRequest(
                $"uid={username},ou=People,dc=example,dc=com",
                DirectoryAttributeOperation.Replace,
                "userpassword",
                newPassword);

            var req2 = new ModifyRequest(
                $"uid={username},ou=People,dc=example,dc=com",
                DirectoryAttributeOperation.Replace,
                "description",
                _serializer.Serialize(user.AdditionalInfo));

            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    con.SendRequest(req1);
                    con.SendRequest(req2);

                    return newPassword;
                }
            }
            catch
            {
                throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            var uAdd = ((LdapMembershipUser)user).AdditionalInfo;

            uAdd.Comment = user.Comment;
            uAdd.IsApproved = user.IsApproved;

            var req1 = new ModifyRequest(
                $"uid={user.UserName},ou=People,dc=example,dc=com",
                DirectoryAttributeOperation.Replace,
                "mail",
                user.Email);

            var req2 = new ModifyRequest(
                $"uid={user.UserName},ou=People,dc=example,dc=com",
                DirectoryAttributeOperation.Replace,
                "description",
                _serializer.Serialize(uAdd));

            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    con.SendRequest(req1);
                    con.SendRequest(req2);
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateUser");
                    throw new ProviderException(e.Message);
                }

                throw;
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;
            var user = (LdapMembershipUser)GetLocalUser(username);

            if (user != null)
            {
                var path = $"ou=People,dc={Domain[0]},dc={Domain[1]}";
                var uAdd = user.AdditionalInfo;

                try
                {
                    using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                    {
                        con.Credential = new NetworkCredential($"uid={username},{path}", password);
                        con.AuthType = AuthType.Basic;
                        con.SessionOptions.ProtocolVersion = 3;
                        con.Bind();

                        if (uAdd.IsApproved)
                        {
                            uAdd.LastLoginDate = DateTime.Now;
                            var request = new ModifyRequest(
                                $"uid={username},ou=People,dc=example,dc=com",
                                DirectoryAttributeOperation.Replace,
                                "description",
                                _serializer.Serialize(user.AdditionalInfo));

                            con.SendRequest(request);
                            isValid = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "ValidateUser");
                        throw new ProviderException(ExceptionMessage);
                    }
                }
            }

            return isValid;
        }

        public override bool UnlockUser(string userName)
        {
            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    string sFilter = $"(&(objectClass=person)(uid={userName}))";
                    var path = $"ou=People,dc={Domain[0]},dc={Domain[1]}";

                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    SearchRequest request = new SearchRequest(path, sFilter, SearchScope.Subtree);

                    request.Attributes.Add("uid");
                    request.Attributes.Add("facsimiletelephonenumber");
                    request.Attributes.Add("givenname");
                    request.Attributes.Add("cn");
                    request.Attributes.Add("telephonenumber");
                    request.Attributes.Add("sn");
                    request.Attributes.Add("roomnumber");
                    request.Attributes.Add("mail");
                    request.Attributes.Add("l");
                    request.Attributes.Add("ou");
                    request.Attributes.Add("uidNumber");
                    request.Attributes.Add("description");
                    request.Attributes.Add("userpassword");
                    request.SizeLimit = 1;

                    var response = (SearchResponse)con.SendRequest(request);

                    if (response != null)
                    {
                        var user = GetUserFromEntry(response.Entries[0]);
                        var uAdd = user.AdditionalInfo;

                        uAdd.IsLockedOut = false;
                        uAdd.LastLockedOutDate = DateTime.Now;
                        var mRequest = new ModifyRequest($"uid={user.UserName},ou=People,dc=example,dc=com", DirectoryAttributeOperation.Replace, "description", _serializer.Serialize(uAdd));
                        con.SendRequest(mRequest);

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                    throw new ProviderException(ExceptionMessage);
                }

                throw;
            }

            return false;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            LdapMembershipUser user = null;

            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    string sFilter = $"(&(objectClass=person)(uidNumber={providerUserKey}))";
                    var path = $"ou=People,dc={Domain[0]},dc={Domain[1]}";

                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    SearchRequest request = new SearchRequest(path, sFilter, SearchScope.Subtree);

                    request.Attributes.Add("uid");
                    request.Attributes.Add("facsimiletelephonenumber");
                    request.Attributes.Add("givenname");
                    request.Attributes.Add("cn");
                    request.Attributes.Add("telephonenumber");
                    request.Attributes.Add("sn");
                    request.Attributes.Add("roomnumber");
                    request.Attributes.Add("mail");
                    request.Attributes.Add("l");
                    request.Attributes.Add("ou");
                    request.Attributes.Add("uidNumber");
                    request.Attributes.Add("description");
                    request.Attributes.Add("userpassword");
                    request.SizeLimit = 1;

                    var response = (SearchResponse)con.SendRequest(request);

                    if (response != null)
                    {
                        user = GetUserFromEntry(response.Entries[0]);

                        if (userIsOnline)
                        {
                            var uAdd = GetUserFromEntry(response.Entries[0]);

                            uAdd.LastActivityDate = DateTime.Now;
                            var mRequest = new ModifyRequest($"uid={user.UserName},ou=People,dc=example,dc=com", DirectoryAttributeOperation.Replace, "description", _serializer.Serialize(uAdd));
                            con.SendRequest(mRequest);
                        }


                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                    throw new ProviderException(ExceptionMessage);
                }

                throw;
            }

            return user;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            LdapMembershipUser user = null;

            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    string sFilter = $"(&(objectClass=person)(uid={username}))";
                    var path = $"ou=People,dc={Domain[0]},dc={Domain[1]}";

                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    SearchRequest request = new SearchRequest(path, sFilter, SearchScope.Subtree);

                    request.Attributes.Add("uid");
                    request.Attributes.Add("facsimiletelephonenumber");
                    request.Attributes.Add("givenname");
                    request.Attributes.Add("cn");
                    request.Attributes.Add("telephonenumber");
                    request.Attributes.Add("sn");
                    request.Attributes.Add("roomnumber");
                    request.Attributes.Add("mail");
                    request.Attributes.Add("l");
                    request.Attributes.Add("ou");
                    request.Attributes.Add("uidNumber");
                    request.Attributes.Add("description");
                    request.Attributes.Add("userpassword");
                    request.SizeLimit = 1;

                    var response = (SearchResponse)con.SendRequest(request);

                    if (response != null)
                    {
                        user = GetUserFromEntry(response.Entries[0]);

                        if (userIsOnline)
                        {
                            var uAdd = user.AdditionalInfo;

                            uAdd.LastActivityDate = DateTime.Now;
                            var mRequest = new ModifyRequest($"uid={username},ou=People,dc=example,dc=com", DirectoryAttributeOperation.Replace, "description", _serializer.Serialize(uAdd));
                            con.SendRequest(mRequest);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                    throw new ProviderException(ExceptionMessage);
                }

                throw;
            }

            return user;
        }

        public override string GetUserNameByEmail(string email)
        {
            string username = "";

            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    string sFilter = $"(&(objectClass=person)(mail={email}))";
                    var path = $"ou=People,dc={Domain[0]},dc={Domain[1]}";

                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    SearchRequest request = new SearchRequest(path, sFilter, SearchScope.Subtree);

                    request.Attributes.Add("uid");
                    request.Attributes.Add("facsimiletelephonenumber");
                    request.Attributes.Add("givenname");
                    request.Attributes.Add("cn");
                    request.Attributes.Add("telephonenumber");
                    request.Attributes.Add("sn");
                    request.Attributes.Add("roomnumber");
                    request.Attributes.Add("mail");
                    request.Attributes.Add("l");
                    request.Attributes.Add("ou");
                    request.Attributes.Add("uidNumber");
                    request.Attributes.Add("description");
                    request.Attributes.Add("userpassword");
                    request.SizeLimit = 1;

                    var response = (SearchResponse)con.SendRequest(request);

                    if (response != null)
                    {
                        var u = GetUserFromEntry(response.Entries[0]);
                        username = u.UserName;
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                    throw new ProviderException(ExceptionMessage);
                }

                throw;
            }

            return username;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    string sFilter = "(&(objectClass=person)(sn=*))";
                    var path = $"ou=People,dc={Domain[0]},dc={Domain[1]}";

                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    SearchRequest request = new SearchRequest(path, sFilter, SearchScope.Subtree);

                    request.Attributes.Add("uid");
                    request.Attributes.Add("facsimiletelephonenumber");
                    request.Attributes.Add("givenname");
                    request.Attributes.Add("cn");
                    request.Attributes.Add("telephonenumber");
                    request.Attributes.Add("sn");
                    request.Attributes.Add("roomnumber");
                    request.Attributes.Add("mail");
                    request.Attributes.Add("l");
                    request.Attributes.Add("ou");
                    request.Attributes.Add("uidNumber");
                    request.Attributes.Add("description");
                    request.Attributes.Add("userpassword");
                    request.SizeLimit = 100000;

                    var response = (SearchResponse)con.SendRequest(request);

                    if (response != null)
                    {
                        foreach (SearchResultEntry entry in response.Entries)
                        {
                            var u = GetUserFromEntry(entry);
                            users.Add(u);
                            totalRecords++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllUsers");
                    throw new ProviderException(ExceptionMessage);
                }

                throw;
            }

            return users;
        }

        public override int GetNumberOfUsersOnline()
        {
            int count;
            var users = GetAllUsers(1, 1000, out count).Cast<LdapMembershipUser>();
            TimeSpan onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            int numOnline = users.Count(x => x.AdditionalInfo.LastActivityDate > compareTime);

            return numOnline;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var coll = new MembershipUserCollection();

            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    string sFilter = $"(&(objectClass=person)(uid={usernameToMatch}))";
                    var path = $"ou=People,dc={Domain[0]},dc={Domain[1]}";

                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    SearchRequest request = new SearchRequest(path, sFilter, SearchScope.Subtree);

                    request.Attributes.Add("uid");
                    request.Attributes.Add("facsimiletelephonenumber");
                    request.Attributes.Add("givenname");
                    request.Attributes.Add("cn");
                    request.Attributes.Add("telephonenumber");
                    request.Attributes.Add("sn");
                    request.Attributes.Add("roomnumber");
                    request.Attributes.Add("mail");
                    request.Attributes.Add("l");
                    request.Attributes.Add("ou");
                    request.Attributes.Add("uidNumber");
                    request.Attributes.Add("description");
                    request.Attributes.Add("userpassword");
                    request.SizeLimit = 1000;

                    var response = (SearchResponse)con.SendRequest(request);

                    if (response != null)
                    {
                        int counter = 0;

                        foreach (SearchResultEntry entry in response.Entries)
                        {
                            var u = GetUserFromEntry(entry);
                            coll.Add(u);
                            counter += 1;
                        }

                        totalRecords = counter;

                        var users = coll
                            .Cast<LdapMembershipUser>()
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize);

                        var toReturn = new MembershipUserCollection();

                        foreach (LdapMembershipUser user in users)
                            toReturn.Add(user);

                        return toReturn;
                    }

                    totalRecords = 0;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                    throw new ProviderException(ExceptionMessage);
                }

                throw;
            }

            return coll;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var coll = new MembershipUserCollection();

            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    string sFilter = $"(&(objectClass=person)(mail={emailToMatch}))";
                    var path = $"ou=People,dc={Domain[0]},dc={Domain[1]}";

                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    SearchRequest request = new SearchRequest(path, sFilter, SearchScope.Subtree);

                    request.Attributes.Add("uid");
                    request.Attributes.Add("facsimiletelephonenumber");
                    request.Attributes.Add("givenname");
                    request.Attributes.Add("cn");
                    request.Attributes.Add("telephonenumber");
                    request.Attributes.Add("sn");
                    request.Attributes.Add("roomnumber");
                    request.Attributes.Add("mail");
                    request.Attributes.Add("l");
                    request.Attributes.Add("ou");
                    request.Attributes.Add("uidNumber");
                    request.Attributes.Add("description");
                    request.Attributes.Add("userpassword");
                    request.SizeLimit = 1000;

                    var response = (SearchResponse)con.SendRequest(request);

                    if (response != null)
                    {
                        int counter = 0;

                        foreach (SearchResultEntry entry in response.Entries)
                        {
                            var u = GetUserFromEntry(entry);
                            coll.Add(u);
                            counter += 1;
                        }

                        totalRecords = counter;

                        var users = coll
                            .Cast<LdapMembershipUser>()
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize);

                        var toReturn = new MembershipUserCollection();

                        foreach (LdapMembershipUser user in users)
                            toReturn.Add(user);

                        return toReturn;
                    }

                    totalRecords = 0;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                    throw new ProviderException(ExceptionMessage);
                }

                throw;
            }

            return coll;
        }

        public override bool EnablePasswordRetrieval => _pEnablePasswordRetrieval;

        public override bool EnablePasswordReset => _pEnablePasswordReset;

        public override bool RequiresQuestionAndAnswer => _pRequiresQuestionAndAnswer;

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts => _pMaxInvalidPasswordAttempts;

        public override int PasswordAttemptWindow => _pPasswordAttemptWindow;

        public override bool RequiresUniqueEmail => _pRequiresUniqueEmail;

        public override MembershipPasswordFormat PasswordFormat => _pPasswordFormat;

        public override int MinRequiredPasswordLength => _pMinRequiredPasswordLength;

        public override int MinRequiredNonAlphanumericCharacters => _pMinRequiredNonAlphanumericCharacters;

        public override string PasswordStrengthRegularExpression => _pPasswordStrengthRegularExpression;

        #endregion

        #region Util Functions

        private MembershipUser GetLocalUser(string username, bool getPassword = false)
        {
            try
            {
                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    string sFilter = $"(&(objectClass=person)(uid={username}))";
                    var path = $"ou=People,dc={Domain[0]},dc={Domain[1]}";

                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    SearchRequest request = new SearchRequest(path, sFilter, SearchScope.Subtree);

                    request.Attributes.Add("uid");
                    request.Attributes.Add("facsimiletelephonenumber");
                    request.Attributes.Add("givenname");
                    request.Attributes.Add("cn");
                    request.Attributes.Add("telephonenumber");
                    request.Attributes.Add("sn");
                    request.Attributes.Add("roomnumber");
                    request.Attributes.Add("mail");
                    request.Attributes.Add("l");
                    request.Attributes.Add("ou");
                    request.Attributes.Add("uidNumber");
                    request.Attributes.Add("description");
                    request.Attributes.Add("userpassword");
                    request.SizeLimit = 1;

                    var response = (SearchResponse)con.SendRequest(request);

                    if (response != null)
                    {
                        var user = GetUserFromEntry(response.Entries[0], getPassword);
                        return user;
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                    throw new ProviderException(ExceptionMessage);
                }

                throw;
            }

            return null;
        }

        private bool CheckPassword(string password, string dbpassword)
        {
            return password == dbpassword;
        }

        private void UpdateFailureCount(string username, string failureType)
        {
            var user = (LdapMembershipUser)GetLocalUser(username, true);

            if (user != null)
            {
                DateTime windowStart = new DateTime();
                int failureCount = 0;

                if (failureType == "password")
                {
                    failureCount = user.AdditionalInfo.FailedPasswordAttemptCount;
                    windowStart = user.AdditionalInfo.FailedPasswordAttemptWindowStart;
                }

                if (failureType == "passwordAnswer")
                {
                    failureCount = user.AdditionalInfo.FailedPasswordAnswerAttemptCount;
                    windowStart = user.AdditionalInfo.FailedPasswordAnswerAttemptWindowStart;
                }

                DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);
                var uAdd = user.AdditionalInfo;

                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    if (failureType == "password")
                    {
                        uAdd.FailedPasswordAttemptCount = 1;
                        uAdd.FailedPasswordAttemptWindowStart = DateTime.Now;
                    }

                    if (failureType == "passwordAnswer")
                    {
                        uAdd.FailedPasswordAnswerAttemptCount = 1;
                        uAdd.FailedPasswordAnswerAttemptWindowStart = DateTime.Now;
                    }
                }
                else
                {
                    if (failureCount++ >= MaxInvalidPasswordAttempts)
                    {
                        uAdd.IsLockedOut = true;
                        uAdd.LastLockedOutDate = DateTime.Now;
                    }
                    else
                    {
                        if (failureType == "password")
                            uAdd.FailedPasswordAttemptCount = failureCount;

                        if (failureType == "passwordAnswer")
                            uAdd.FailedPasswordAnswerAttemptCount = failureCount;
                    }
                }

                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    var request = new ModifyRequest($"uid={username},ou=People,dc=example,dc=com", DirectoryAttributeOperation.Replace, "description", _serializer.Serialize(uAdd));
                    con.SendRequest(request);
                }
            }
        }

        private LdapMembershipUser GetUserFromEntry(SearchResultEntry entry, bool getPassword = false)
        {
            var userName = entry.Attributes["uid"][0].ToString();
            var userKey = Convert.ToInt32(entry.Attributes["uidNumber"][0]);
            var email = entry.Attributes["mail"][0].ToString();
            var fax = entry.Attributes["facsimiletelephonenumber"][0].ToString();
            var firstName = entry.Attributes["givenname"][0].ToString();
            var fullName = entry.Attributes["cn"][0].ToString();
            var phone = entry.Attributes["telephonenumber"][0].ToString();
            var lastName = entry.Attributes["sn"][0].ToString();
            var room = Convert.ToInt32(entry.Attributes["roomnumber"][0]);
            var location = entry.Attributes["l"][0].ToString();
            var description = entry.Attributes["description"].Count == 0 ? null : entry.Attributes["description"][0].ToString();
            var password = entry.Attributes["userpassword"][0].ToString();
            var roles = new List<string>();

            for (int i = 0; i < entry.Attributes["ou"].Count; i++)
            {
                var d = entry.Attributes["ou"][i];
                roles.Add(d.ToString());
            }

            UserAdditionalInfo uAdd;

            if (string.IsNullOrEmpty(description))
            {
                uAdd = new UserAdditionalInfo
                {                    
                    // We start without information, so we are going to assign default values
                    IsApproved = true,
                    ApplicationName = ApplicationName,
                    IsLockedOut = false,
                    Comment = string.Empty,
                    IsOnline = false,
                    IsSubscriber = false,
                    LastLoginDate = DateTime.MinValue,
                    LastActivityDate = DateTime.MinValue,
                    PasswordQuestion = string.Empty,
                    PasswordAnswer = string.Empty,
                    LastPasswordChangedDate = DateTime.MinValue,
                    FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue,
                    FailedPasswordAttemptWindowStart = DateTime.MinValue,
                    LastLockedOutDate = DateTime.MinValue,
                    FailedPasswordAnswerAttemptCount = 0,
                    FailedPasswordAttemptCount = 0                    
                };

                using (var con = new LdapConnection(new LdapDirectoryIdentifier(LdapServerPath, LdapServerPort)))
                {
                    con.AuthType = AuthType.Anonymous;
                    con.SessionOptions.ProtocolVersion = 3;

                    var request = new ModifyRequest($"uid={userName},ou=People,dc=example,dc=com", DirectoryAttributeOperation.Replace, "description", _serializer.Serialize(uAdd));
                    con.SendRequest(request);
                }
            }
            else
                uAdd = _serializer.Deserialize<UserAdditionalInfo>(description);

            var user = new LdapMembershipUser(
                Name,
                userName,
                getPassword ? password : null,
                userKey,
                email, fax, firstName, fullName, lastName, phone, room, location, roles.ToArray(),
                uAdd);

            return user;
        }

        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog
            {
                Source = EventSource,
                Log = EventLog
            };

            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e;

            log.WriteEntry(message);
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        #endregion
    }
}