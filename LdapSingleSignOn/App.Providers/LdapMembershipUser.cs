using System;
using System.Web.Security;

namespace App.Providers
{
    public class LdapMembershipUser : MembershipUser
    {
        public string Password { get; private set; }
        public string Fax { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string LastName { get; set; }
        public int RoomNumber { get; set; }
        public string Location { get; set; }
        public string[] Roles { get; set; }
        public LdapMembershipProvider.UserAdditionalInfo AdditionalInfo { get; set; }

        public LdapMembershipUser(
            string providerName,
            string userName,
            string password,
            object providerUserKey,
            string email,
            string fax,
            string firstName,
            string fullName,
            string lastName,
            string phone,
            int roomNumber,
            string location,
            string[] roles,
            LdapMembershipProvider.UserAdditionalInfo additionalInfo) : base(
                providerName,
                userName,
                providerUserKey,
                email,
                additionalInfo.PasswordQuestion,
                additionalInfo.Comment,
                additionalInfo.IsApproved,
                additionalInfo.IsLockedOut,
                DateTime.MinValue,
                additionalInfo.LastLoginDate,
                additionalInfo.LastActivityDate,
                DateTime.MinValue, DateTime.MinValue)
        {
            Password = password;
            Fax = fax;
            FirstName = firstName;
            FullName = fullName;
            LastName = lastName;
            Phone = phone;
            RoomNumber = roomNumber;
            Location = location;
            Roles = roles;
            AdditionalInfo = additionalInfo;
        }
    }
}