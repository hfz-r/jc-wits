using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IAHUTransactionRepository
    {
        IEnumerable<AHUTransaction> GetAHUTransactionByAHUID(long AHUID);
        IEnumerable<AHUTransaction> GetAll(bool eagerLoading);
    }
}