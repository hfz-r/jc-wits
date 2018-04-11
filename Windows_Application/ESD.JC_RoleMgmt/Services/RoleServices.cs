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

        public IEnumerable<Role> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return roleRepository.GetAll(true);
            else
                return roleRepository.GetAll(false);
        }

        public IEnumerable<Role> GetRoleWithAssociated(long ID)
        {
            return roleRepository.GetRoleWithAssociated(ID);
        }

        public Role GetRole(long ID)
        {
            return roleRepository.GetRole(ID);
        }

        public class Response
        {
            public bool state { get; set; }
            public long id { get; set; }
        }

        public Response Save(Role role, string state = "")
        {
            long ID = 0;

            try
            {
                if (!string.IsNullOrEmpty(state) && state == "Save")
                {
                    ID = roleRepository.Add(role);
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

            return new Response() { id = ID, state = true };
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
