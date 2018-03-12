using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_LocationMgmt.Services
{
    public interface ILocationServices
    {
        IEnumerable<Location> GetAll();
        Location GetLocation(long ID);
        bool Save(List<Location> reasons, string state = "");
        bool Delete(long ID);
    }
}