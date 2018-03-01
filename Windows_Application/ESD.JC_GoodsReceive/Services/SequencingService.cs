using ESD.JC_GoodsReceive.Utilities;
using DataLayer;
using System.Collections.ObjectModel;

namespace ESD.JC_GoodsReceive.Services
{
    public static class SequencingService
    {
        public static GoodsReceive SequenceParent { get; set; }

        public static ObservableCollection<T> SetCollectionSequence<T>(ObservableCollection<T> targetCollection) where T : ISequencedObject
        {
            var sequenceNumber = 1;
            var currentDateTime = System.DateTime.Now;
            var parent = SequenceParent;

            foreach (ISequencedObject sequencedObject in targetCollection)
            {
                sequencedObject.SequenceNumber = sequenceNumber;
                sequencedObject.GRID = parent.ID;
                sequencedObject.PO = parent.PurchaseOrder;
                sequencedObject.SAPNO = parent.Material;
                sequencedObject.EN = parent.MaterialShortText;
                sequencedObject.EUN = parent.Eun;
                sequencedObject.BIN = parent.StorageBin;
                sequencedObject.IsChecked = false;

                if (sequencedObject.ID == 0)
                    sequencedObject.ModifiedOn = currentDateTime;

                sequenceNumber++;
            }

            return targetCollection;
        }
    }
}
