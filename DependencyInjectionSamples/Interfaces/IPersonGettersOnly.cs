namespace NP.Samples.Interfaces
{
    public interface IPersonGettersOnly
    {
        string PersonName { get; set; }

        IAddress Address { get; }
    }
}
