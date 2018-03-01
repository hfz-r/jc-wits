using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IEunKGRepository
    {
        void Add(EunKG kg);
        void Delete(long ID);
        IEnumerable<EunKG> GetAll(bool eagerLoading);
        EunKG GetEunKG(long ID);
        void Update(EunKG kg);
    }
}