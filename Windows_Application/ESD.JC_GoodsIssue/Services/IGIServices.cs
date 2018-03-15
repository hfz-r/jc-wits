using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_GoodsIssue.Services
{
    public interface IGIServices
    {
        IEnumerable<GITransaction> GetAll(bool eagerLoading);
        GITransaction GetGoodsIssue(long ID);
        GITransaction GetGI(long ID);
    }
}