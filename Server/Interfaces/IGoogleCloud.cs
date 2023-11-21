using DB.Models;

namespace Server.Interfaces
{
    public interface IGoogleCloud
    {
        NLog.ILogger Logger { get; }
        void InsertInfoToDB(string ProjectId, string InstanceId, string StartTime, string EndTime, string JsonFileLocation, string MachineType, string Location);
        void InsertDummyInfoToDB(string StartTime, string MachineType, string Location);
        List<VirtualMachineMetricsModel> GetInfoFromDB(string MachineType, string Location);
        List<VirtualMachineMetricsModel> LoadItemsFromTimeStamp(string MachineType, string Location, string TimeStamp);
    }
}
