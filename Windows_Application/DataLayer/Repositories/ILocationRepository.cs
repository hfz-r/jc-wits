using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public interface ILocationRepository
    {
        IEnumerable<Location> GetAll(bool eagerLoading);
        Location GetLocation(long ID);
        void Add(Location reason);
        void Update(Location reason);
        void Delete(long ID);
    }
}