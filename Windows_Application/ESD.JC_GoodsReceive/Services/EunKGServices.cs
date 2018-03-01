using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace ESD.JC_GoodsReceive.Services
{
    public class EunKGServices : IEunKGServices
    {
        private IEunKGRepository eunKGRepository;

        public EunKGServices(IEunKGRepository eunKGRepository)
        {
            this.eunKGRepository = eunKGRepository;
        }

        public IEnumerable<EunKG> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return eunKGRepository.GetAll(true);
            else
                return eunKGRepository.GetAll(false);
        }

        public EunKG GetEunKG(long ID)
        {
            return eunKGRepository.GetEunKG(ID);
        }

        public bool Save(List<EunKG> kgs, string state = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(state) && state == "Save")
                {
                    kgs.ForEach(x => eunKGRepository.Add(x));
                }
                else if (!string.IsNullOrEmpty(state) && state == "Update")
                {
                    kgs.ForEach(x => eunKGRepository.Update(x));
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

        public bool Delete(long ID)
        {
            try
            {
                var kgObj = GetEunKG(ID);
                if (kgObj != null)
                {
                    eunKGRepository.Delete(ID);
                }
                else
                {
                    throw new Exception("Eun Kilogram (KG) Not Found.");
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
