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

        public IEnumerable<Reason> GetAll()
        {
            return reasonRepository.GetAll(false);
        }

        public Reason GetReason(long ID)
        {
            return reasonRepository.GetReason(ID);
        }

        public bool Save(List<Reason> reasons, string state = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(state) && state == "Save")
                {
                    reasons.ForEach(x => reasonRepository.Add(x));
                }
                else if (!string.IsNullOrEmpty(state) && state == "Update")
                {
                    reasons.ForEach(x => reasonRepository.Update(x));
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
                var reasonObj = GetReason(ID);
                if (reasonObj != null)
                {
                    reasonRepository.Delete(ID);
                }
                else
                {
                    throw new Exception("Reason Not Found.");
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
