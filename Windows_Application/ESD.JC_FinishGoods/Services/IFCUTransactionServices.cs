using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_FinishGoods.Services
{
    public interface IFCUTransactionServices
    {
        IEnumerable<FCUTransaction> GetAll(bool eagerLoading);
        IEnumerable<FCUTransaction> GetFCUTransactionByFCUID(long FCUID);
        Location GetLocationFromFCUTransaction(long ID);
    }
}