using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace ESD.JC_ReasonMgmt.Services
{
    public class ReasonServices : IReasonServices
    {
        private IReasonRepository reasonRepository;

        public ReasonServices(IReasonRepository _reasonRepository)
        {
            reasonRepository = _reasonRepository;
        }

        public List<Reason> GetAll()
        {
            return reasonRepository.GetAll();
        }

        public Reason GetReason(long ID)
        {
            return reasonRepository.GetReason(ID);
        }

        public bool Save(Reason reason)
        {
            try
            {
                reasonRepository.Save(reason);
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

        public void Delete(long? ID)
        {
            try
            {
                var reasonObj = GetReason(ID.GetValueOrDefault());
                if (reasonObj != null)
                {
                    reasonRepository.Delete(ID.Value);
                }
                else
                {
                    throw new Exception("ReasonCode Not Found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
