using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_GoodsReceive.Services
{
    public interface IGRServices
    {
        List<GoodsReceive> GetAll();

        GoodsReceive GetGR(string purchase_order);

        bool Save(GoodsReceive gr);
    }
}