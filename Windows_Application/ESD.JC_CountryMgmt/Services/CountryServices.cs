﻿using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace ESD.JC_CountryMgmt.Services
{
    public class CountryServices : ICountryServices
    {
        private ICountryRepository reasonRepository;

        public CountryServices(ICountryRepository _reasonRepository)
        {
            reasonRepository = _reasonRepository;
        }

        public IEnumerable<Country> GetAll()
        {
            return reasonRepository.GetAll(false);
        }

        public Country GetCountry(long ID)
        {
            return reasonRepository.GetCountry(ID);
        }

        public bool Save(List<Country> reasons, string state = "")
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
                var reasonObj = GetCountry(ID);
                if (reasonObj != null)
                {
                    reasonRepository.Delete(ID);
                }
                else
                {
                    throw new Exception("Country Not Found.");
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