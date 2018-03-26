using DataLayer;
using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;
using System.Linq;

namespace ESD.JC_GoodsReceive.Notifications
{
    public class ObjectSelectionNotification : Confirmation, IObjectSelectionNotification
    {
        public ObjectSelectionNotification()
        {
            this.Items = new List<EunKG>();
            this.SelectedItem = new EunKG();
            this.ParentItem = new GoodsReceive();
            this.ReturnItem = null;

            this.AuthenticatedUser = string.Empty;
        }

        public ObjectSelectionNotification(IEnumerable<EunKG> items)
            : this()
        {
            foreach (var item in items)
            {
                this.Items.Add(item);
            }
        }

        public ObjectSelectionNotification(GoodsReceive parentItem)
           : this()
        {
            this.ParentItem = parentItem;
        }

        public ObjectSelectionNotification(string user)
           : this()
        {
            this.AuthenticatedUser = user;
        }

        public IList<EunKG> Items { get; private set; }

        public EunKG SelectedItem { get; set; }

        public GoodsReceive ParentItem { get; set; }

        public string AuthenticatedUser { get; set; }

        public string ReturnItem { get; set; }
    } 
}
