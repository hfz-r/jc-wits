using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_ReasonMgmt.Services
{
    public interface IReasonServices
    {
        IEnumerable<Reason> GetAll();
        Reason GetReason(long ID);
        bool Save(List<Reason> reasons, string state = "");
        bool Delete(long ID);
    }
}