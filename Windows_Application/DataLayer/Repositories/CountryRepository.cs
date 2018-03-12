using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        public IEnumerable<Country> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                return context.Countries.OrderBy(x => x.ID).ToList();
            }
        }

        public Country GetCountry(long ID)
        {
            using (var context = new InventoryContext())
            {
                var country = context.Countries.Find(ID);
                return country;
            }
        }

        public void Add(Country country)
        {
            using (var context = new InventoryContext())
            {
                var searchCountry = context.Countries.Where(x => x.CountryDesc == country.CountryDesc).FirstOrDefault();
                if (searchCountry == null)
                {
                    context.Countries.Add(country);
                    context.SaveChanges();
                }
            }
        }

        public void Update(Country country)
        {
            using (var context = new InventoryContext())
            {
                var searchCountry = context.Countries.Where(x => x.CountryDesc == country.CountryDesc).FirstOrDefault();
                if (searchCountry == null)
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
                var reasons = context.Countries.Where(id => id.ID == ID);
                context.Countries.RemoveRange(reasons);

                context.SaveChanges();
            }
        }
    }
}
