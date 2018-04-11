using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IAHURepository
    {
        void Add(AHU ahu);
        AHU GetAHU(long ID);
        AHU GetAHUBySerialNo(string serial_no);
        IEnumerable<AHU> GetAll(bool eagerLoading);
        void Update(AHU ahu);
        void Delete(long ID);
    }
}