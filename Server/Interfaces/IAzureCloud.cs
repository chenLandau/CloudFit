using DB.Models;

namespace Server.Interfaces
{
    public interface IAzureCloud
    {
        NLog.ILogger Logger { get; }
        void InsertInfoToDB(string SubscriptionId, string ResourceGroupName, string VirtualMachineName, string TimeSpan, string MachineType, string Location, double MemorySizeInGB);
        VirtualMachineMetricsModel InsertDummyInfoToDB(string TimeSpan, string MachineType, string Location);
        List<VirtualMachineMetricsModel> GetInfoFromDB(string MachineType, string Location);
        List<VirtualMachineMetricsModel> LoadItemsFromTimeStamp(string MachineType, string Location, string TimeStamp);
    }
}
