using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer;
using DataLayer.Repositories;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace ESD.JC_FinishGoods.Services
{
    public class FCUServices : IFCUServices
    {
        private IFCURepository fcuRepository;

        public FCUServices(IFCURepository fcuRepository)
        {
            this.fcuRepository = fcuRepository;
        }

        public IEnumerable<FCU> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return fcuRepository.GetAll(true);
            else
                return fcuRepository.GetAll(false);
        }

        public FCU GetFCU(long ID)
        {
            return fcuRepository.GetFCU(ID);
        }

        public FCU GetFCUBySerialNo(string serial_no)
        {
            return fcuRepository.GetFCUBySerialNo(serial_no);
        }

        public bool Save(List<FCU> fcus, string state = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(state) && state == "Save")
                {
                    fcus.ForEach(x => fcuRepository.Add(x));
                }
                else if (!string.IsNullOrEmpty(state) && state == "Update")
                {
                    fcus.ForEach(x => fcuRepository.Update(x));
                }
            }
            catch (DbEntityValidationException e)
            {
                string exceptionMessage = string.Empty;

                foreach (var eve in e.EntityValidationErrors)
                {
                    exceptionMessage = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        exceptionMessage += string.Format("\n\n- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(exceptionMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }
    }
}
