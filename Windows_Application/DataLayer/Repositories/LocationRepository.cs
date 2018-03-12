using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        public IEnumerable<Location> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                return context.Locations.OrderBy(x => x.ID).ToList();
            }
        }

        public Location GetLocation(long ID)
        {
            using (var context = new InventoryContext())
            {
                var country = context.Locations.Find(ID);
                return country;
            }
        }

        public void Add(Location country)
        {
            using (var context = new InventoryContext())
            {
                var searchLocation = context.Locations.Where(x => x.LocationDesc == country.LocationDesc).FirstOrDefault();
                if (searchLocation == null)
                {
                    context.Locations.Add(country);
                    context.SaveChanges();
                }
            }
        }

        public void Update(Location country)
        {
            using (var context = new InventoryContext())
            {
                var searchLocation = context.Locations.Where(x => x.LocationDesc == country.LocationDesc).FirstOrDefault();
                if (searchLocation == null)
                {
                    context.Entry(country).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public void Delete(long ID)
        {
            using (var context = new InventoryContext())
            {
                var reasons = context.Locations.Where(id => id.ID == ID);
                context.Locations.RemoveRange(reasons);

                context.SaveChanges();
            }
        }
    }
}
