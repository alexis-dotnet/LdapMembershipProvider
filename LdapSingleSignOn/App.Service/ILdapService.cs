using System.Collections.Generic;
using System.ServiceModel;

namespace App.Service
{
    [ServiceContract]
    public interface ILdapService
    {
        [OperationContract]
        LdapUser GetUser(string username);
        [OperationContract]
        List<LdapUser> GetAllUsers();
        [OperationContract]
        List<LdapUser> GetOnlineUsers();
    }
}
