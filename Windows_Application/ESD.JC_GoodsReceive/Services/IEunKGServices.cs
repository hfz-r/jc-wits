using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_GoodsReceive.Services
{
    public interface IEunKGServices
    {
        bool Delete(long ID);
        IEnumerable<EunKG> GetAll(bool eagerLoading);
        EunKG GetEunKG(long ID);
        bool Save(List<EunKG> kgs, string state = "");
    }
}