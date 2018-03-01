namespace ESD.JC_ReasonMgmt.Utilities
{
    public interface ISequencedObject
    {
        int SequenceNumber { get; set; }

        long ID { get; set; }

        System.DateTime ModifiedOn { get; set; }
    }
}