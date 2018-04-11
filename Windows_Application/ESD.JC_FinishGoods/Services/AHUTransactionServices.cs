using DataLayer;
using DataLayer.Repositories;
using System.Collections.Generic;

namespace ESD.JC_FinishGoods.Services
{
    public class AHUTransactionServices : IAHUTransactionServices
    {
        private IAHUTransactionRepository ahuTrnxRepository;
        private IAHURepository ahuRepository;

        public AHUTransactionServices(IAHUTransactionRepository ahuTrnxRepository, IAHURepository ahuRepository)
        {
            this.ahuTrnxRepository = ahuTrnxRepository;
            this.ahuRepository = ahuRepository;
        }

        public IEnumerable<AHUTransaction> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return ahuTrnxRepository.GetAll(eagerLoading);
            else
                return ahuTrnxRepository.GetAll(false);
        }

        public IEnumerable<AHUTransaction> GetAHUTransactionByAHUID(long AHUID)
        {
            return ahuTrnxRepository.GetAHUTransactionByAHUID(AHUID);
        }

        public Location GetLocationFromAHUTransaction(long ID)
        {
            return ahuTrnxRepository.GetLocationFromAHUTransaction(ID);
        }

        public AHU GetAHUDetails(long ID)
        {
            return ahuRepository.GetAHU(ID);
        }
    }
}
