using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_RoleMgmt.Services
{
    public interface IRoleServices
    {
        IEnumerable<Role> GetAll();
        Role GetRole(long ID);
        bool Save(Role role, string state = "");
        void Delete(long ID);
    }
}