using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_FinishGoods.Services
{
    public interface IAHUServices
    {
        AHU GetAHU(long ID);
        AHU GetAHUBySerialNo(string serial_no);
        IEnumerable<AHU> GetAll(bool eagerLoading);
        bool Save(List<AHU> ahus, string state = "");
    }
}