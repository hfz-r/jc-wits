using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_RoleMgmt.Services
{
    public interface IRoleServices
    {
        List<Role> GetAll();

        Role GetRole(long ID);

        bool Save(Role role);

        void Delete(long? ID);
    }
}