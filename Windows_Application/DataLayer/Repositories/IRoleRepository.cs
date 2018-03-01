﻿using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<Role> GetAll(bool eagerLoading);
        Role GetRole(long ID);
        void Add(Role user);
        void Update(Role user);
        void Delete(long ID);
    }
}