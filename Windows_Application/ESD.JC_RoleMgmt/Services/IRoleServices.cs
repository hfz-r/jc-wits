using System.Collections.Generic;
using DataLayer;
using static ESD.JC_RoleMgmt.Services.RoleServices;

namespace ESD.JC_RoleMgmt.Services
{
    public interface IRoleServices
    {
        IEnumerable<Role> GetAll(bool eagerLoading);
        IEnumerable<Role> GetRoleWithAssociated(long ID);
        Role GetRole(long ID);
        Response Save(Role role, string state = "");
        void Delete(long ID);
    }
}