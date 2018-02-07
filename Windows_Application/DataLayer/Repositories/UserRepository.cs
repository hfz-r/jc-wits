using System.Linq;
using System.Collections.Generic;
using System;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private InventoryContext context;

        public UserRepository(InventoryContext context)
        {
            this.context = context;
        }

        public User Login(string username, string password)
        {
            return context.Users.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
        }

        public IEnumerable<User> GetAll()
        {
            return context.Users.OrderBy(x => x.Username).ToList();
        }

        public User GetUser(long ID)
        {
            return context.Users.Find(ID);
        }

        public void AddInto(User user)
        {
            context.Users.Add(user);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(User user)
        {
            context.Entry(user).State = EntityState.Modified;
        }

        public void Delete(long ID)
        {
            User user = context.Users.Find(ID);
            context.Users.Remove(user);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
