namespace ESD.JC_LocationMgmt.Utilities
{
    public interface ISequencedObject
    {
        int SequenceNumber { get; set; }

        long ID { get; set; }

        System.DateTime ModifiedOn { get; set; }
    }
}