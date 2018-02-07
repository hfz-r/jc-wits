using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IRoleRepository
    {
        List<Role> GetAll();

        Role GetRole(long ID);

        void Save(Role role);

        void Delete(long ID);
    }
}