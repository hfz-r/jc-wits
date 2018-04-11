using System;
using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_UserMgmt.Services
{
    public interface IUserServices
    {
        IEnumerable<User> GetAll(bool eagerLoading);
        User GetUser(long ID);
        bool Save(List<User> users, string state = "");
        bool Delete(long ID);
        IEnumerable<GRTransaction> GetUserGRTrnx(long ID);
        IEnumerable<GITransaction> GetUserGITrnx(long ID);
    }
}