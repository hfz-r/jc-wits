using DataLayer;
using DataLayer.Repositories;
using System;
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

        public bool Delete(long ID)
        {
            try
            {
                var giObj = GetGITransaction(ID);
                if (giObj != null)
                {
                    giTrnxRepository.Delete(ID);
                }
                else
                {
                    throw new Exception("Record Not Found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }
    }
}
