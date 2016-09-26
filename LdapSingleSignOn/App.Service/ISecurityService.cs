using System.ServiceModel;

namespace App.Service
{
    [ServiceContract]
    public interface ISecurityService
    {

        [OperationContract]
        bool Authenticate(string username, string password);
    }
}
