using System.Collections.Generic;
using System.ServiceModel;
using UserStorage.Interfaces.Entities;

namespace WcfServiceLibrary
{
    [ServiceContract]
    public interface IWcfUserService
    {
        [OperationContract]
        int Add(User user);

        [OperationContract]
        void Delete(int personalId);
        
        [OperationContract]
        IList<int> Search(User criteria);
    }
}
