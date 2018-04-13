using DataLayer;
using DataLayer.Repositories;
using System.Collections.Generic;

namespace ESD.JC_GoodsIssue.Services
{
    public class GIServices : IGIServices
    {
        private IGoodsIssueRepository giRepository;

        public GIServices(IGoodsIssueRepository _giRepository)
        {
            giRepository = _giRepository;
        }

        public IEnumerable<GITransaction> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return giRepository.GetAll(eagerLoading);
            else
                return giRepository.GetAll(false);
        }

        public GITransaction GetGoodsIssue(long ID)
        {
            return giRepository.GetGoodsIssue(ID);
        }

        public GITransaction GetGI(long ID)
        {
            return giRepository.GetGI(ID);
        }
    }
}
