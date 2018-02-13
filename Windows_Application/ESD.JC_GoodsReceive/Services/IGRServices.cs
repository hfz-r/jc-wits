using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_GoodsReceive.Services
{
    public interface IGRServices
    {
        IEnumerable<GoodsReceive> GetAll();
        GoodsReceive GetGR(long ID);
        GoodsReceive GetSAPNo(string sap_no);
        bool Save(List<GoodsReceive> gr, string state = "");
    }
}