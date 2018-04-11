using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_GoodsReceive.Services
{
    public interface IGRServices
    {
        IEnumerable<GoodsReceive> GetAll();
        GoodsReceive GetGR(long ID);
        GoodsReceive GetGRBySAPNo(string sap_no);
        GoodsReceive GetEunKG(long ID);
        bool Save(List<GoodsReceive> gr, string state = "");
        bool Delete(long ID);
    }
}