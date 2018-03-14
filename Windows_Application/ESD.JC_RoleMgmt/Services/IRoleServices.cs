using System.Collections.Generic;
using DataLayer;
using static ESD.JC_RoleMgmt.Services.RoleServices;

namespace ESD.JC_RoleMgmt.Services
{
    public interface IRoleServices
    {
        IEnumerable<Role> GetAll();
        Role GetRole(long ID);
        Response Save(Role role, string state = "");
        void Delete(long ID);
    }
}