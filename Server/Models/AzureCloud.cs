using Microsoft.AspNetCore.WebUtilities;
using RestSharp;
using DB.Models;
using DB;
using Newtonsoft.Json;
using System.Globalization;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using NLog;
using Server.Interfaces;

namespace Server.Models
{
    public class AzureCloud : IAzureCloud
    {
        private const string AzureCloudName = "AzureCloud";
        private readonly MongoHelper DB;
        private static readonly Random Random = new Random();
        public NLog.ILogger Logger { get; }

        public AzureCloud(MongoHelper mongoHelper)
        {
            DB = mongoHelper;
            Logger = LogManager.GetLogger("AzureCloudLogger");
        }

        public void InsertInfoToDB(
            string SubscriptionId,
            string ResourceGroupName,
            string VirtualMachineName,
            string TimeSpan,
            string MachineType,
            string Location,
            double MemorySizeInGB)
        {
            string[] parts = TimeSpan.Split('/');
            string accessToken = "Bearer " + GetAccessToken();
            VirtualMachineMetricsModel metrics = new VirtualMachineMetricsModel
            {
                TimeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
            };
            var tasks = new List<Task>
            {
                Task.Run(() => metrics.PercentageCPU = GetCpuUsageInfo(SubscriptionId, ResourceGroupName, VirtualMachineName, TimeSpan, accessToken)),
                Task.Run(() => metrics.PercentageMemory = 100 - GetMemoryUsageInfo(SubscriptionId, ResourceGroupName, VirtualMachineName, TimeSpan, accessToken, MemorySizeInGB)),
                Task.Run(() => metrics.IncomingTraffic = GetNetworkInUsageInfo(SubscriptionId, ResourceGroupName, VirtualMachineName, TimeSpan, accessToken)),
                Task.Run(() => metrics.OutcomingTraffic = GetNetworkOutUsageInfo(SubscriptionId, ResourceGroupName, VirtualMachineName, TimeSpan, accessToken))
            };
            Task.WaitAll(tasks.ToArray());

            Logger.Info($"{MachineType} {Location}: PercentageCPU = {metrics.PercentageCPU}, PercentageMemory = {metrics.PercentageMemory}, IncomingTraffic = {metrics.IncomingTraffic}, OutcomingTraffic = {metrics.OutcomingTraffic}");
            DB.InsertItem(AzureCloudName + MachineType + Location, metrics);
        }

        public VirtualMachineMetricsModel InsertDummyInfoToDB(string TimeSpan, string MachineType, string Location)
        {
            string[] parts = TimeSpan.Split('/');
            VirtualMachineMetricsModel metrics = new VirtualMachineMetricsModel
            {
                TimeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                PercentageCPU = Random.NextDouble() * 10 + 70,
                PercentageMemory = Random.NextDouble() * 10 + 85,
                IncomingTraffic = Random.NextDouble() * 10 + 250,
                OutcomingTraffic = Random.NextDouble() + 0.9
            };

            Logger.Info($"{MachineType} {Location}: PercentageCPU = {metrics.PercentageCPU}, PercentageMemory = {metrics.PercentageMemory}, IncomingTraffic = {metrics.IncomingTraffic}, OutcomingTraffic = {metrics.OutcomingTraffic}");
            DB.InsertItem(AzureCloudName + MachineType + Location, metrics);
            return metrics;
        }

        public List<VirtualMachineMetricsModel> GetInfoFromDB(string MachineType, string Location)
        {
            return DB.LoadItems<VirtualMachineMetricsModel>(AzureCloudName + MachineType + Location);
        }

        public List<VirtualMachineMetricsModel> LoadItemsFromTimeStamp(string MachineType, string Location, string TimeStamp)
        {
            return DB.LoadItemsFromTimeStamp(AzureCloudName + MachineType + Location, DateTime.ParseExact(TimeStamp, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
        }

        private string GetAccessToken()
        {
            Process process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = "-Command " + "az account get-access-token";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            JObject response = JObject.Parse(output);
            return response["accessToken"].ToString();
        }

        private string BuildUrl(string SubscriptionId, string ResourceGroupName, string VirtualMachineName, string TimeSpan, string MetricName)
        {
            var url = $"https://management.azure.com/subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroupName}/providers/Microsoft.Compute/virtualMachines/{VirtualMachineName}/providers/microsoft.insights/metrics";
            url = QueryHelpers.AddQueryString(url, new Dictionary<string, string?>
            {
                { "api-version", "2018-01-01" },
                { "metricnames", MetricName },
                { "timespan", TimeSpan }
            });

            return url ;
        }

        private dynamic GetInfoFromResponse(RestResponse Response)
        {
            if(Response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dynamic info = JsonConvert.DeserializeObject(Response.Content);
                return info?.value[0].timeseries[0].data[0];
            }
            else
            {
                throw new Exception("Can't get info from VM");
            }
        }
        
        private dynamic GetMetricInfo(
            string SubscriptionId,
            string ResourceGroupName,
            string VirtualMachineName,
            string TimeSpan,
            string AccessToken,
            string MetricName)
        {
            var url = BuildUrl(SubscriptionId, ResourceGroupName, VirtualMachineName, TimeSpan, MetricName);
            var options = new RestClientOptions(url) { MaxTimeout = -1 };
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("Authorization", AccessToken);
            RestResponse response = client.Execute(request);
            var info = GetInfoFromResponse(response);
            return info;
        }

        private double GetCpuUsageInfo(string SubscriptionId, string ResourceGroupName, string VirtualMachineName, string TimeSpan, string AccessToken)
        {
            var info = GetMetricInfo(SubscriptionId, ResourceGroupName, VirtualMachineName, TimeSpan, AccessToken, "Percentage%20CPU");
            return info.average;
        }

        private double GetMemoryUsageInfo(string SubscriptionId, string ResourceGroupName, string VirtualMachineName, string TimeSpan, string AccessToken, double MemorySizeInGB)
        {
            var info = GetMetricInfo(SubscriptionId, ResourceGroupName, VirtualMachineName, TimeSpan, AccessToken, "Available Memory Bytes");
            return ((info.average * 100) / (double)(MemorySizeInGB * Math.Pow(2, 30)));
        }

        private double GetNetworkInUsageInfo(string SubscriptionId, string ResourceGroupName, string VirtualMachineName, string TimeSpan, string AccessToken)
        {
            var info = GetMetricInfo(SubscriptionId, ResourceGroupName, VirtualMachineName, TimeSpan, AccessToken, "Network In");
            return (info.total * 8 / 60) / Math.Pow(2, 20); // from total bytes in minute to Mbits/s
        }

        private double GetNetworkOutUsageInfo(string SubscriptionId, string ResourceGroupName, string VirtualMachineName, string TimeSpan, string AccessToken)
        {
            var info = GetMetricInfo(SubscriptionId, ResourceGroupName, VirtualMachineName, TimeSpan, AccessToken, "Network Out");
            return (info.total * 8 / 60) / Math.Pow(2, 20); // from total bytes in minute to Mbits/s
        }
    }
}
