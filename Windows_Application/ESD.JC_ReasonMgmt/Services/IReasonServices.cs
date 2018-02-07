using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_ReasonMgmt.Services
{
    public interface IReasonServices
    {
        List<Reason> GetAll();

        Reason GetReason(long ID);

        bool Save(Reason reason);

        void Delete(long? ID);
    }
}