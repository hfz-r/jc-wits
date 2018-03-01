using DataLayer;
using DataLayer.Repositories;
using System.Collections.Generic;

namespace ESD.JC_GoodsReceive.Services
{
    public class GRTransactionServices : IGRTransactionServices
    {
        private IGRTransactionRepository grTrnxRepository;

        public GRTransactionServices(IGRTransactionRepository grTrnxRepository)
        {
            this.grTrnxRepository = grTrnxRepository;
        }

        public IEnumerable<GRTransaction> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return grTrnxRepository.GetAll(eagerLoading);
            else
                return grTrnxRepository.GetAll(false);
        }

        public IEnumerable<GRTransaction> GetGRTransactionByGRID(long GRID)
        {
            return grTrnxRepository.GetGRTransactionByGRID(GRID);
        }
    }
}
