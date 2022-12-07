namespace NP.Samples.Interfaces
{
    public interface IOrgGettersOnly
    {
        string OrgName { get; set; }

        IPersonGettersOnly Manager { get; }

        ILog Log { get; }

        void LogOrgInfo();
    }
}
