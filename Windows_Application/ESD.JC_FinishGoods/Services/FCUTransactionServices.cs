using DataLayer;
using DataLayer.Repositories;
using System.Collections.Generic;

namespace ESD.JC_FinishGoods.Services
{
    public class FCUTransactionServices : IFCUTransactionServices
    {
        private IFCUTransactionRepository fcuTrnxRepository;

        public FCUTransactionServices(IFCUTransactionRepository fcuTrnxRepository)
        {
            this.fcuTrnxRepository = fcuTrnxRepository;
        }

        public IEnumerable<FCUTransaction> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return fcuTrnxRepository.GetAll(eagerLoading);
            else
                return fcuTrnxRepository.GetAll(false);
        }

        public IEnumerable<FCUTransaction> GetFCUTransactionByFCUID(long FCUID)
        {
            return fcuTrnxRepository.GetFCUTransactionByFCUID(FCUID);
        }
    }
}
