using DataLayer;
using DataLayer.Repositories;
using System.Collections.Generic;

namespace ESD.JC_FinishGoods.Services
{
    public class AHUTransactionServices : IAHUTransactionServices
    {
        private IAHUTransactionRepository ahuTrnxRepository;

        public AHUTransactionServices(IAHUTransactionRepository ahuTrnxRepository)
        {
            this.ahuTrnxRepository = ahuTrnxRepository;
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
    }
}
