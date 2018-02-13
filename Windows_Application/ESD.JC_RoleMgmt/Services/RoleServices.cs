using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

namespace ESD.JC_RoleMgmt.Services
{
    public class RoleServices : IRoleServices
    {
        private IRoleRepository roleRepository;

        public RoleServices(IRoleRepository _roleRepository)
        {
            roleRepository = _roleRepository;
        }

        public IEnumerable<Role> GetAll()
        {
            return roleRepository.GetAll(false);
        }

        public Role GetRole(long ID)
        {
            return roleRepository.GetRole(ID);
        }

        public bool Save(Role role, string state = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(state) && state == "Save")
                {
                    roleRepository.Add(role);
                }
                else if (!string.IsNullOrEmpty(state) && state == "Update")
                {
                    roleRepository.Update(role);
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

        public void Delete(long ID)
        {
            try
            {
                var roleObj = GetRole(ID);
                if (roleObj != null)
                {
                    roleRepository.Delete(ID);
                }
                else
                {
                    throw new Exception("Role Not Found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
