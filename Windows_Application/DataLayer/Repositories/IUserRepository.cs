using System;
using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IUserRepository
    {
        User Login(string username, string password);
        IEnumerable<User> GetAll(bool eagerLoading);
        User GetUser(long ID);
        void Add(User user);
        void Update(User user);
        void Delete(long ID);
    }
}