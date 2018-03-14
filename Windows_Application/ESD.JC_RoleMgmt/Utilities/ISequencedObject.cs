namespace ESD.JC_RoleMgmt.Utilities
{
    public interface ISequencedObject
    {
        long ID { get; set; }

        string Module { get; set; }

        bool IsChecked { get; set; }
    }
}