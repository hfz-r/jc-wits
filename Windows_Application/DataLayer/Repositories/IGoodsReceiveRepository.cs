using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IGoodsReceiveRepository
    {
        IEnumerable<GoodsReceive> GetAll(bool eagerLoading);
        GoodsReceive GetGR(long ID);
        GoodsReceive GetGRBySAPNo(string sap_no);
        GoodsReceive GetEunKGDetails(long ID);
        void Add(GoodsReceive gr);
        void Update(GoodsReceive gr);
    }
}