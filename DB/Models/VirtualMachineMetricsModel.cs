using MongoDB.Bson.Serialization.Attributes;

namespace DB.Models
{
    public class VirtualMachineMetricsModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double PercentageCPU { get; set; }
        public double PercentageMemory { get; set; }
        public double IncomingTraffic { get; set; }
        public double OutcomingTraffic { get; set; }
    }
}
