using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IFCURepository
    {
        void Add(FCU fcu);
        IEnumerable<FCU> GetAll(bool eagerLoading);
        FCU GetFCU(long ID);
        FCU GetFCUBySerialNo(string serial_no);
        void Update(FCU fcu);
        void Delete(long ID);
    }
}