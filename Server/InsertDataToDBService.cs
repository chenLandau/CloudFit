using Newtonsoft.Json;
using Server.Interfaces;
using Server.VirtualMachineModel;

public class InsertDataToDBService : BackgroundService
{
    private DateTime StartTimeUtc;
    private DateTime EndTimeUtc;
    private VirtualMachines VirtualMachines;
    private string VirtualMachineJsonPath = AppContext.BaseDirectory + "..\\..\\..\\..\\Deployment\\VirtualMachines.json";
    private readonly IAzureCloud AzureCloud;
    private readonly IAmazonCloud AmazonCloud;
    private readonly IGoogleCloud GoogleCloud;

    public InsertDataToDBService(IAzureCloud azureCloud, IAmazonCloud amazonCloud, IGoogleCloud googleCloud)
    {
        StartTimeUtc = DateTime.UtcNow.AddMinutes(-5);
        EndTimeUtc = DateTime.UtcNow.AddMinutes(-4);
        StartTimeUtc = StartTimeUtc.AddSeconds(-StartTimeUtc.Second);
        EndTimeUtc = EndTimeUtc.AddSeconds(-EndTimeUtc.Second);
        AzureCloud = azureCloud;
        AmazonCloud = amazonCloud;
        GoogleCloud = googleCloud;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            StartTimeUtc = StartTimeUtc.AddMinutes(1);
            EndTimeUtc = EndTimeUtc.AddMinutes(1);
            InsertInfoToDB();
            await WaitUntilMinuteOver(stoppingToken); 
        }
    }

    private void InsertInfoToDB()
    {
        string startTime = StartTimeUtc.ToString("yyyy-MM-ddTHH:mm:ssZ");
        string endTime = EndTimeUtc.ToString("yyyy-MM-ddTHH:mm:ssZ");
        VirtualMachines = JsonConvert.DeserializeObject<VirtualMachines>(File.ReadAllText(VirtualMachineJsonPath));

        var tasks = new List<Task>
        {
            Task.Run(() => InsertInfoAzureCloud(startTime, endTime)),
            Task.Run(() => InsertInfoGoogleCloud(startTime, endTime)),
            Task.Run(() => InsertInfoAmazonCloud(startTime, endTime))
        };
        Task.WaitAll(tasks.ToArray());
    }

    private void InsertInfoAzureCloud(string StartTime, string EndTime)
    {
        var tasks = new List<Task>();
        foreach (var vm in VirtualMachines.AzureVirtualMachine)
        {
            Task task = Task.Run(() =>
            {
                try
                {
                    // Due to low budget we are running only VM with RAM size up to 4GB, if we send bigger size we should insert dummy data
                    if (vm.MemorySize > 4)
                    {
                        AzureCloud.InsertDummyInfoToDB(StartTime, vm.MachineType, vm.Location);
                    }
                    else
                    {
                        AzureCloud.InsertInfoToDB(vm.SubscriptionId, vm.ResourceGroupName, vm.VirtualMachineName, StartTime + "/" + EndTime, vm.MachineType, vm.Location, vm.MemorySize);
                    }
                }
                catch (Exception ex)
                {
                    AzureCloud.Logger.Error(ex.Message);
                    //Continue to the next Virtual Machine
                }
            });
            tasks.Add(task);
        }
        Task.WaitAll(tasks.ToArray());
    }

    private void InsertInfoGoogleCloud(string StartTime, string EndTime)
    {
        var tasks = new List<Task>();
        foreach (var vm in VirtualMachines.GoogleVirtualMachine)
        {
            Task task = Task.Run(() =>
            {
                try
                {
                    // Due to low budget we are running only VM with RAM size up to 4GB, if we send bigger size we should insert dummy data
                    if (vm.MemorySize > 4)
                    {
                        GoogleCloud.InsertDummyInfoToDB(StartTime, vm.MachineType, vm.Location);
                    }
                    else
                    {
                        GoogleCloud.InsertInfoToDB(vm.ProjectId, vm.InstanceId, StartTime, EndTime, vm.JsonFileLocation, vm.MachineType, vm.Location);
                    }
                }
                catch(Exception ex) 
                {
                    GoogleCloud.Logger.Error(ex.Message);
                    //Continue to the next Virtual Machine
                }
            });
            tasks.Add(task);
        }
        Task.WaitAll(tasks.ToArray());
    }

    private void InsertInfoAmazonCloud(string StartTime, string EndTime)
    {
        var tasks = new List<Task>();
        foreach (var vm in VirtualMachines.AmazonVirtualMachine)
        {
            Task task = Task.Run(() =>
            {
                try
                {
                    // Due to low budget we are running only VM with RAM size up to 4GB, if we send bigger size we should insert dummy data
                    if (vm.MemorySize > 4)
                    {
                        AmazonCloud.InsertDummyInfoToDB(StartTime, vm.MachineType, vm.Location);
                    }
                    else
                    {
                        AmazonCloud.InsertInfoToDB(vm.AccessKey, vm.SecretKey, vm.InstanceId, vm.Region, StartTime, EndTime, vm.MachineType, vm.Location);
                    }
                }
                catch (Exception ex)
                {
                    AmazonCloud.Logger.Error(ex.Message);
                    //Continue to the next Virtual Machine
                }
            });
            tasks.Add(task);
        }
        Task.WaitAll(tasks.ToArray());
    }

    private async Task WaitUntilMinuteOver(CancellationToken stoppingToken)
    {
        TimeSpan delayDuration = StartTimeUtc.AddMinutes(1) - DateTime.UtcNow.AddMinutes(-5);
        if (delayDuration <= TimeSpan.Zero)
        {
            return;
        }
        await Task.Delay(delayDuration, stoppingToken);
    }
}
