using System.Collections.Generic;
using DataLayer;

namespace ESD.JC_CountryMgmt.Services
{
    public interface ICountryServices
    {
        IEnumerable<Country> GetAll();
        Country GetCountry(long ID);
        bool Save(List<Country> reasons, string state = "");
        bool Delete(long ID);
    }
}