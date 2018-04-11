using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer;
using DataLayer.Repositories;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace ESD.JC_FinishGoods.Services
{
    public class AHUServices : IAHUServices
    {
        private IAHURepository ahuRepository;

        public AHUServices(IAHURepository ahuRepository)
        {
            this.ahuRepository = ahuRepository;
        }

        public IEnumerable<AHU> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return ahuRepository.GetAll(true);
            else
                return ahuRepository.GetAll(false);
        }

        public AHU GetAHU(long ID)
        {
            return ahuRepository.GetAHU(ID);
        }

        public AHU GetAHUBySerialNo(string serial_no)
        {
            return ahuRepository.GetAHUBySerialNo(serial_no);
        }

        public bool Save(List<AHU> ahus, string state = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(state) && state == "Save")
                {
                    ahus.ForEach(x => ahuRepository.Add(x));
                }
                else if (!string.IsNullOrEmpty(state) && state == "Update")
                {
                    ahus.ForEach(x => ahuRepository.Update(x));
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
                var ahuObj = GetAHU(ID);
                if (ahuObj != null)
                {
                    ahuRepository.Delete(ID);
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