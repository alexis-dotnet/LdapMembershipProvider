using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Security;
using App.Providers;

namespace App.Service
{
    public class LdapService : ILdapService
    {
        public LdapUser GetUser(string username)
        {
            var user = (LdapMembershipUser)Membership.GetUser(username);

            if (user != null)
                return new LdapUser
                {
                    Id = (int?) user.ProviderUserKey ?? 0,
                    Username = user.UserName,
                    Fax = user.Fax,
                    IsOnline = user.IsOnline,
                    FirstName = user.FirstName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = user.Roles,
                    LastActivityDate = user.LastActivityDate,
                    LastLoginDate = user.LastLoginDate,
                    LastName = user.LastName,
                    Location = user.Location,
                    Phone = user.Phone,
                    RoomNumber = user.RoomNumber
                };

            return null;
        }

        public List<LdapUser> GetAllUsers()
        {
            var users = Membership.GetAllUsers();
            var list = new List<LdapUser>();

            foreach (LdapMembershipUser user in users)
            {
                list.Add(new LdapUser
                {
                    Id = (int?) user.ProviderUserKey ?? 0,
                    Username = user.UserName,
                    Fax = user.Fax,
                    IsOnline = user.IsOnline,
                    FirstName = user.FirstName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = user.Roles,
                    LastActivityDate = user.LastActivityDate,
                    LastLoginDate = user.LastLoginDate,
                    LastName = user.LastName,
                    Location = user.Location,
                    Phone = user.Phone,
                    RoomNumber = user.RoomNumber
                });
            }

            return list;
        }

        public List<LdapUser> GetOnlineUsers()
        {
            var users = Membership.GetAllUsers();
            var list = new List<LdapUser>();

            foreach (LdapMembershipUser user in users)
            {
                if (user.IsOnline)
                    list.Add(new LdapUser
                    {
                        Id = (int?) user.ProviderUserKey ?? 0,
                        Username = user.UserName,
                        Fax = user.Fax,
                        IsOnline = user.IsOnline,
                        FirstName = user.FirstName,
                        Email = user.Email,
                        FullName = user.FullName,
                        Roles = user.Roles,
                        LastActivityDate = user.LastActivityDate,
                        LastLoginDate = user.LastLoginDate,
                        LastName = user.LastName,
                        Location = user.Location,
                        Phone = user.Phone,
                        RoomNumber = user.RoomNumber
                    });
            }

            return list;
        }
    }

    [DataContract]
    public class LdapUser
    {
        [DataMember]
        public string Fax { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public int RoomNumber { get; set; }
        [DataMember]
        public string Location { get; set; }
        [DataMember]
        public string[] Roles { get; set; }
        [DataMember]
        public DateTime LastLoginDate { get; set; }
        [DataMember]
        public DateTime LastActivityDate { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public bool IsOnline { get; set; }
    }
}
