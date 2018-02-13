using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IReasonRepository
    {
        IEnumerable<Reason> GetAll(bool eagerLoading);
        Reason GetReason(long ID);
        void Add(Reason reason);
        void Update(Reason reason);
        void Delete(long ID);
    }
}