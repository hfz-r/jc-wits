using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<Role> GetAll(bool eagerLoading);
        IEnumerable<Role> GetRoleWithAssociated(long ID);
        Role GetRole(long ID);
        long Add(Role user);
        void Update(Role user);
        void Delete(long ID);
    }
}