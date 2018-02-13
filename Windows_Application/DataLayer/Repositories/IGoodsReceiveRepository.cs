using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IGoodsReceiveRepository
    {
        IEnumerable<GoodsReceive> GetAll(bool eagerLoading);
        GoodsReceive GetGR(long ID);
        GoodsReceive GetSAPNo(string sap_no);
        void Add(GoodsReceive gr);
        void Update(GoodsReceive gr);
    }
}