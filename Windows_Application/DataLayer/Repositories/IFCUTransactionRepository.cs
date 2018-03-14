using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IFCUTransactionRepository
    {
        IEnumerable<FCUTransaction> GetAll(bool eagerLoading);
        IEnumerable<FCUTransaction> GetFCUTransactionByFCUID(long FCUID);
    }
}