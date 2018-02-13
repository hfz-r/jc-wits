namespace ESD.JC_Infrastructure.Utilities
{
    public interface ISequencedObject
    {
        int SequenceNumber { get; set; }

        long ID { get; set; }

        System.DateTime ModifiedOn { get; set; }
    }
}