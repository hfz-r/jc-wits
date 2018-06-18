using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IGoodsIssueRepository
    {
        IEnumerable<GITransaction> GetAll(bool eagerLoading);
        //GITransaction GetGoodsIssue(long ID);
        GITransaction GetGI(string Material);
        void Delete(string Material);
    }
}