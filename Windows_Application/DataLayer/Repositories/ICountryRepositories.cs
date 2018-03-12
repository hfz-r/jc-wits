using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface ICountryRepository
    {
        IEnumerable<Country> GetAll(bool eagerLoading);
        Country GetCountry(long ID);
        void Add(Country reason);
        void Update(Country reason);
        void Delete(long ID);
    }
}