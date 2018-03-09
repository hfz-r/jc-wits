using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_FinishGoods.Services
{
    public interface IFCUServices
    {
        IEnumerable<FCU> GetAll(bool eagerLoading);
        FCU GetFCU(long ID);
        FCU GetFCUBySerialNo(string serial_no);
        bool Save(List<FCU> fcus, string state = "");
    }
}