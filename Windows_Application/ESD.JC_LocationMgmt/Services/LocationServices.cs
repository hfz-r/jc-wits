using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace ESD.JC_LocationMgmt.Services
{
    public class LocationServices : ILocationServices
    {
        private ILocationRepository reasonRepository;

        public LocationServices(ILocationRepository _reasonRepository)
        {
            reasonRepository = _reasonRepository;
        }

        public IEnumerable<Location> GetAll()
        {
            return reasonRepository.GetAll(false);
        }

        public Location GetLocation(long ID)
        {
            return reasonRepository.GetLocation(ID);
        }

        public bool Save(List<Location> reasons, string state = "")
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
                var reasonObj = GetLocation(ID);
                if (reasonObj != null)
                {
                    reasonRepository.Delete(ID);
                }
                else
                {
                    throw new Exception("Location Not Found.");
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
