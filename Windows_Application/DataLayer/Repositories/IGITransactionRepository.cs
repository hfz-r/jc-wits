using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IGITransactionRepository
    {
        IEnumerable<GITransaction> GetAll(bool eagerLoading);
        IEnumerable<GITransaction> GetGITransaction(long ID);
        void Delete(long ID);
    }
}