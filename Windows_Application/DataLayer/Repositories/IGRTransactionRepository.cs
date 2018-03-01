using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IGRTransactionRepository
    {
        IEnumerable<GRTransaction> GetAll(bool eagerLoading);
        IEnumerable<GRTransaction> GetGRTransactionByGRID(long GRID);
    }
}