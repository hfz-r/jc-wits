using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class ReasonRepository : IReasonRepository
    {
        public IEnumerable<Reason> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                return context.Reasons.OrderBy(x => x.ID).ToList();
            }
        }

        public Reason GetReason(long ID)
        {
            using (var context = new InventoryContext())
            {
                var reason = context.Reasons.Find(ID);                
                return reason;
            }
        }

        public void Add(Reason reason)
        {
            using (var context = new InventoryContext())
            {
                var searchReason = context.Reasons.Where(x => x.ReasonDesc == reason.ReasonDesc).FirstOrDefault();
                if (searchReason == null)
                {
                    context.Reasons.Add(reason);
                    context.SaveChanges();
                }
            }
        }

        public void Update(Reason reason)
        {
            using (var context = new InventoryContext())
            {
                var searchReason = context.Reasons.Where(x => x.ReasonDesc == reason.ReasonDesc).FirstOrDefault();
                if (searchReason == null)
                {
                    context.Entry(reason).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public void Delete(long ID)
        {
            using (var context = new InventoryContext())
            {
                var reasons = context.Reasons.Where(id => id.ID == ID);
                context.Reasons.RemoveRange(reasons);

                context.SaveChanges();
            }
        }
    }
}
