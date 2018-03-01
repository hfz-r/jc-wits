using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_GoodsReceive.Services
{
    public interface IGRTransactionServices
    {
        IEnumerable<GRTransaction> GetAll(bool eagerLoading);
        IEnumerable<GRTransaction> GetGRTransactionByGRID(long GRID);
    }
}