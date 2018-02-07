using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IReasonRepository
    {
        List<Reason> GetAll();

        Reason GetReason(long ID);

        void Save(Reason reason);

        void Delete(long ID);
    }
}