using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

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
    }
}
