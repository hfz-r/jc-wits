namespace ESD.JC_GoodsReceive.Utilities
{
    public interface ISequencedObject
    {
        int SequenceNumber { get; set; }

        long ID { get; set; }

        long GRID { get; set; }

        string PO { get; set; }

        string SAPNO { get; set; }

        decimal Qty { get; set; }

        string EN { get; set; }

        string EUN { get; set; }

        string BIN { get; set; }

        System.DateTime ModifiedOn { get; set; }

        bool IsChecked { get; set; }
    }
}