namespace DB.Models
{
    public class MetricsResultsModel
    {
        public double PercentageCPU { get; set; }
        public double PercentageMemory { get; set; }
        public double IncomingTraffic { get; set; }
        public double OutcomingTraffic { get; set; }
    }
}
