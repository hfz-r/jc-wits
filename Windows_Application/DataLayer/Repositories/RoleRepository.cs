using System.Linq;
using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public List<Role> GetAll()
        {
            using (var context = new InventoryContext())
            {
                return context.Roles.OrderBy(x => x.RoleCode).ToList();
            }
        }

        public Role GetRole(long ID)
        {
            using (var context = new InventoryContext())
            {
                var roles = GetAll();

                return roles.Where(x => x.ID == ID).FirstOrDefault();
            }
        }

        public void Save(Role role)
        {
            using (var context = new InventoryContext())
            {
                var obj = context.Roles.Where(x => x.ID == role.ID).FirstOrDefault();

                if (obj == null)
                {
                    context.Roles.Add(role);
                }
                else
                {
                    obj.RoleCode = role.RoleCode;
                    obj.RoleName = role.RoleName;
                    obj.Module = role.Module;
                    obj.Description = role.Description;
                    obj.ModifiedOn = role.ModifiedOn;
                    obj.ModifiedBy = role.ModifiedBy;
                }

                context.SaveChanges();
            }
        }

        public void Delete(long ID)
        {
            using (var context = new InventoryContext())
            {
                var obj = context.Roles.Where(x => x.ID == ID).FirstOrDefault();
                if (obj != null)
                {
                    context.Roles.Remove(obj);
                }

                context.SaveChanges();
            }
        }
    }
}
