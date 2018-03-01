﻿using System.Linq;
using System.Collections.Generic;
using System;
using System.Data.Entity;

namespace DataLayer.Repositories
{
    public class GoodsIssueRepository : IGoodsIssueRepository
    {
        public IEnumerable<GITransaction> GetAll(bool eagerLoading)
        {
            using (var context = new InventoryContext())
            {
                if (eagerLoading)
                    return context.GITransactions.Include(gr => gr.GoodsReceive).OrderBy(x => x.ID).ToList();
                else
                    return context.GITransactions.OrderBy(x => x.ID).ToList();
            }
        }

        public GITransaction GetGoodsIssue(long ID)
        {
            using (var context = new InventoryContext())
            {
                var gi = context.GITransactions.Find(ID);
                if (gi != null)
                {
                    context.Entry(gi).Reference(gr => gr.GoodsReceive).Load();
                }
                return gi;
            }
        }
    }
}