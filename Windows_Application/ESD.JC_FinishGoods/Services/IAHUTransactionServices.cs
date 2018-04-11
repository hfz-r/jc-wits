using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_FinishGoods.Services
{
    public interface IAHUTransactionServices
    {
        IEnumerable<AHUTransaction> GetAHUTransactionByAHUID(long AHUID);
        IEnumerable<AHUTransaction> GetAll(bool eagerLoading);
        Location GetLocationFromAHUTransaction(long ID);
        AHU GetAHUDetails(long ID);
    }
}