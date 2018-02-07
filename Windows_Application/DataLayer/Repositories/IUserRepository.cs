using System;
using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface IUserRepository : IDisposable
    {
        User Login(string username, string password);
        IEnumerable<User> GetAll();
        User GetUser(long ID);
        void AddInto(User user);
        void Save();
        void Update(User user);
        void Delete(long ID);
    }
}