using System.Linq;
using System.Collections.Generic;
using System;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        public User Login(string username, string password)
        {
            using (var context = new InventoryContext())
            {
                var user = context.Users.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
                if (user != null)
                {
                    context.Entry(user).Reference(x => x.Role).Load();
                }
                return user;
            }
        }

        public IEnumerable<User> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.Users.Include(r => r.Role).OrderBy(x => x.Username).ToList();
                else
                    return context.Users.OrderBy(x => x.Username).ToList();
            }
        }

        public User GetUser(long ID)
        {
            using (var context = new InventoryContext())
            {
                var user = context.Users.Find(ID);
                if (user != null)
                {
                    context.Entry(user).Reference(x => x.Role).Load();
                }
                return user;
            }
        }

        public void Add(User user)
        {
            using (var context = new InventoryContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public void Update(User user)
        {
            using (var context = new InventoryContext())
            {
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void Delete(long ID)
        {
            using (var context = new InventoryContext())
            {
                User user = context.Users.Find(ID);
                context.Users.Remove(user);

                context.SaveChanges();
            }
        }
    }
}
