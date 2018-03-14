using System.Linq;
using System.Collections.Generic;
using System;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public IEnumerable<Role> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.Roles.Include(u => u.Users).OrderBy(x => x.RoleCode).ToList();
                else
                    return context.Roles.OrderBy(x => x.RoleCode).ToList();
            }
        }

        public Role GetRole(long ID)
        {
            using (var context = new InventoryContext())
            {
                var role = context.Roles.Find(ID);
                if (role != null)
                {
                    context.Entry(role).Collection(x => x.Users).Load();
                }
                return role;
            }
        }

        public long Add(Role role)
        {
            using (var context = new InventoryContext())
            {
                context.Roles.Add(role);
                context.SaveChanges();
            }
            return role.ID;
        }

        public void Update(Role role)
        {
            using (var context = new InventoryContext())
            {
                context.Entry(role).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void Delete(long ID)
        {
            using (var context = new InventoryContext())
            {
                var roles = context.Roles.Where(id => id.ID == ID).Include(u => u.Users);
                context.Roles.RemoveRange(roles);

                context.SaveChanges();
            }
        }
    }
}
