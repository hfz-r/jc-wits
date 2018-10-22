using DataLayer;
using DataLayer.Repositories;
using System.Collections.Generic;

namespace ESD.JC_GoodsIssue.Services
{
    public class GITransactionServices : IGITransactionServices
    {
        private IGITransactionRepository giTrnxRepository;

        public GITransactionServices(IGITransactionRepository giTrnxRepository)
        {
            this.giTrnxRepository = giTrnxRepository;
        }

        public IEnumerable<GITransaction> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return giTrnxRepository.GetAll(eagerLoading);
            else
                return giTrnxRepository.GetAll(false);
        }

        public IEnumerable<GITransaction> GetGITransaction(long ID)
        {
            return giTrnxRepository.GetGITransaction(ID);
        }
    }
}
