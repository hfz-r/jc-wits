using System;
using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_UserMgmt.Services
{
    public interface IUserServices
    {
        IEnumerable<User> GetAll();
        User GetUser(long ID);
        bool Save(User User, string state = "");
        void Delete(long ID);
    }
}