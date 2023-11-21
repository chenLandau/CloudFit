using DB.Models;

namespace Server.Interfaces
{
    public interface IAmazonCloud
    {
        NLog.ILogger Logger { get; }
        void InsertInfoToDB(string AccessKey, string SecretKey, string InstanceId, string Region, string StartTime, string EndTime, string MachineType, string Location);
        void InsertDummyInfoToDB(string StartTime, string MachineType, string Location);
        List<VirtualMachineMetricsModel> GetInfoFromDB(string MachineType, string Location);
        List<VirtualMachineMetricsModel> LoadItemsFromTimeStamp(string MachineType, string Location, string TimeStamp);
    }
}
