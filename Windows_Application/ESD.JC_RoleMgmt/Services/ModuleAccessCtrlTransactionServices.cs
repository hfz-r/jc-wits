using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

namespace ESD.JC_RoleMgmt.Services
{
    public class ModuleAccessCtrlTransactionServices : IModuleAccessCtrlTransactionServices
    {
        private IModuleAccessCtrlTransactionRepository moduleAccessCtrlTransactionRepository;

        public ModuleAccessCtrlTransactionServices(IModuleAccessCtrlTransactionRepository _moduleAccessCtrlTransactionRepository)
        {
            moduleAccessCtrlTransactionRepository = _moduleAccessCtrlTransactionRepository;
        }

        public IEnumerable<ModuleAccessCtrlTransaction> GetAll()
        {
            return moduleAccessCtrlTransactionRepository.GetAll();
        }

        public List<ModuleAccessCtrlTransaction> GetModuleAccessCtrlTransaction(long ID)
        {
            return moduleAccessCtrlTransactionRepository.GetModuleAccessCtrlTransaction(ID);
        }

        public bool Save(ModuleAccessCtrlTransaction module, string state = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(state) && state == "Save")
                {
                    moduleAccessCtrlTransactionRepository.Add(module);
                }
                else if (!string.IsNullOrEmpty(state) && state == "Update")
                {
                    moduleAccessCtrlTransactionRepository.Update(module);
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
