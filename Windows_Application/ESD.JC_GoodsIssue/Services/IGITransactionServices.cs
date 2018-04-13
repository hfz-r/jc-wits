using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_GoodsIssue.Services
{
    public interface IGITransactionServices
    {
        IEnumerable<GITransaction> GetAll(bool eagerLoading);
        IEnumerable<GITransaction> GetGITransaction(long ID);
        bool Delete(long ID);
    }
}