namespace Server.Interfaces
{
    public interface IMetricsResults
    {
        public Dictionary<string, string> GetMetricsResultsFromDB();
    }
}
