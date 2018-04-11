using DataLayer;
using DataLayer.Repositories;
using System.Collections.Generic;

namespace ESD.JC_FinishGoods.Services
{
    public class FCUTransactionServices : IFCUTransactionServices
    {
        private IFCUTransactionRepository fcuTrnxRepository;
        private IFCURepository fcuRepository;

        public FCUTransactionServices(IFCUTransactionRepository fcuTrnxRepository, IFCURepository fcuRepository)
        {
            this.fcuTrnxRepository = fcuTrnxRepository;
            this.fcuRepository = fcuRepository;
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

        public Location GetLocationFromFCUTransaction(long ID)
        {
            return fcuTrnxRepository.GetLocationFromFCUTransaction(ID);
        }

        public FCU GetFCUDetails(long ID)
        {
            return fcuRepository.GetFCU(ID);
        }
    }
}
