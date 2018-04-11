using DataLayer;
using DataLayer.Repositories;
using System.Collections.Generic;

namespace ESD.JC_GoodsReceive.Services
{
    public class GRTransactionServices : IGRTransactionServices
    {
        private IGRTransactionRepository grTrnxRepository;
        private IGoodsReceiveRepository grRepository;

        public GRTransactionServices(IGRTransactionRepository grTrnxRepository, IGoodsReceiveRepository grRepository)
        {
            this.grTrnxRepository = grTrnxRepository;
            this.grRepository = grRepository;
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

        public GoodsReceive GetGRDetails(long GRID)
        {
            return grRepository.GetGR(GRID);
        }
    }
}
