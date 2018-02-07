using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IGoodsReceiveRepository
    {
        List<GoodsReceive> GetAll();

        GoodsReceive GetGR(string purchase_order);

        void Save(GoodsReceive gr);
    }
}