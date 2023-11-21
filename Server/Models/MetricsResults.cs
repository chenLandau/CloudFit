using Newtonsoft.Json;
using Server.VirtualMachineModel;
using DB.Models;
using DB;
using Server.Interfaces;

namespace Server.Models
{
    public class MetricsResults : IMetricsResults
    {
        private readonly MongoHelper DB;
        private static string VirtualMachineJsonPath = AppContext.BaseDirectory + "..\\..\\..\\..\\Deployment\\VirtualMachines.json";
        private VirtualMachines VirtualMachines;
        private List<string> MachineTypes;
        private Dictionary<string, int> AzurePoints;
        private Dictionary<string, int> AmazonPoints;
        private Dictionary<string, int> GooglePoints;

        public MetricsResults(MongoHelper mongoHelper)
        {
            DB = mongoHelper;
        }

        public Dictionary<string, string> GetMetricsResultsFromDB()
        {
            AzurePoints = ResetPointsDictionary();
            AmazonPoints = ResetPointsDictionary();
            GooglePoints = ResetPointsDictionary();
            VirtualMachines = JsonConvert.DeserializeObject<VirtualMachines>(System.IO.File.ReadAllText(VirtualMachineJsonPath));
            SetMachinesTypes();

            foreach (var type in MachineTypes)
            {
                AddPoints(
                    CalculateAverageForMachineTypeAzureCloud(type),
                    CalculateAverageForMachineTypeAmazonCloud(type),
                    CalculateAverageForMachineTypeGoogleCloud(type));
            }

            return CalculateRank();
        }

        private Dictionary<string, int> ResetPointsDictionary()
        {
            return new Dictionary<string, int>
            {
                { "PercentageCPU", 0 },
                { "PercentageMemory", 0 },
                { "IncomingTraffic", 0 },
                { "OutcomingTraffic", 0 }
            };
        }

        private void SetMachinesTypes()
        {
            List<string> machineTypes = new List<string>();

            foreach(var vm in VirtualMachines.AzureVirtualMachine)
            {
                machineTypes.Add(vm.MachineType);
            }
            foreach (var vm in VirtualMachines.AmazonVirtualMachine)
            {
                machineTypes.Add(vm.MachineType);
            }
            foreach (var vm in VirtualMachines.GoogleVirtualMachine)
            {
                machineTypes.Add(vm.MachineType);
            }

            MachineTypes = machineTypes.Distinct().ToList();
        }

        private MetricsResultsModel CalculateAverageForMachineTypeAzureCloud(string MachineType)
        {
            int numberOfMachines = 0;
            MetricsResultsModel metrics = new MetricsResultsModel
            {
                PercentageCPU = 0,
                PercentageMemory = 0,
                IncomingTraffic = 0,
                OutcomingTraffic = 0
            }; 
            
            foreach (var vm in VirtualMachines.AzureVirtualMachine)
            {
                if (vm.MachineType == MachineType)
                {
                    var data = DB.GetResultsMetricsFromTable("AzureCloud" + vm.MachineType + vm.Location);
                    metrics.PercentageCPU += data.PercentageCPU;
                    metrics.PercentageMemory += data.PercentageMemory;
                    metrics.IncomingTraffic += data.IncomingTraffic;
                    metrics.OutcomingTraffic += data.OutcomingTraffic;
                    numberOfMachines++;
                }
            }

            metrics.PercentageCPU /= numberOfMachines;
            metrics.PercentageMemory /= numberOfMachines;
            metrics.IncomingTraffic /= numberOfMachines;
            metrics.OutcomingTraffic /= numberOfMachines;

            return metrics;
        }

        private MetricsResultsModel CalculateAverageForMachineTypeAmazonCloud(string MachineType)
        {
            int numberOfMachines = 0;
            MetricsResultsModel metrics = new MetricsResultsModel
            {
                PercentageCPU = 0,
                PercentageMemory = 0,
                IncomingTraffic = 0,
                OutcomingTraffic = 0
            };

            foreach (var vm in VirtualMachines.AzureVirtualMachine)
            {
                if (vm.MachineType == MachineType)
                {
                    var data = DB.GetResultsMetricsFromTable("AmazonCloud" + vm.MachineType + vm.Location);
                    metrics.PercentageCPU += data.PercentageCPU;
                    metrics.PercentageMemory += data.PercentageMemory;
                    metrics.IncomingTraffic += data.IncomingTraffic;
                    metrics.OutcomingTraffic += data.OutcomingTraffic;
                    numberOfMachines++;
                }
            }

            metrics.PercentageCPU /= numberOfMachines;
            metrics.PercentageMemory /= numberOfMachines;
            metrics.IncomingTraffic /= numberOfMachines;
            metrics.OutcomingTraffic /= numberOfMachines;

            return metrics;
        }

        private MetricsResultsModel CalculateAverageForMachineTypeGoogleCloud(string MachineType)
        {
            int numberOfMachines = 0;
            MetricsResultsModel metrics = new MetricsResultsModel
            {
                PercentageCPU = 0,
                PercentageMemory = 0,
                IncomingTraffic = 0,
                OutcomingTraffic = 0
            };

            foreach (var vm in VirtualMachines.AzureVirtualMachine)
            {
                if (vm.MachineType == MachineType)
                {
                    var data = DB.GetResultsMetricsFromTable("GoogleCloud" + vm.MachineType + vm.Location);
                    metrics.PercentageCPU += data.PercentageCPU;
                    metrics.PercentageMemory += data.PercentageMemory;
                    metrics.IncomingTraffic += data.IncomingTraffic;
                    metrics.OutcomingTraffic += data.OutcomingTraffic;
                    numberOfMachines++;
                }
            }

            metrics.PercentageCPU /= numberOfMachines;
            metrics.PercentageMemory /= numberOfMachines;
            metrics.IncomingTraffic /= numberOfMachines;
            metrics.OutcomingTraffic /= numberOfMachines;

            return metrics;
        }

        private void AddPoints(MetricsResultsModel AzureResults, MetricsResultsModel AmazonResults, MetricsResultsModel GoogleResults)
        {
            AddPercentageCPUPoints(AzureResults, AmazonResults, GoogleResults);
            AddPercentageMemoryPoints(AzureResults, AmazonResults, GoogleResults);
            AddIncomingTrafficPoints(AzureResults, AmazonResults, GoogleResults);
            AddOutcomingTrafficPoints(AzureResults, AmazonResults, GoogleResults);
        }

        private void AddPercentageCPUPoints(MetricsResultsModel AzureResults, MetricsResultsModel AmazonResults, MetricsResultsModel GoogleResults)
        {
            if (AzureResults.PercentageCPU <= AmazonResults.PercentageCPU && AzureResults.PercentageCPU <= GoogleResults.PercentageCPU)
            {
                AzurePoints["PercentageCPU"] += 3;
                if (AmazonResults.PercentageCPU <= GoogleResults.PercentageCPU)
                {
                    AmazonPoints["PercentageCPU"] += 2;
                    GooglePoints["PercentageCPU"] += 1;
                }
                else
                {
                    GooglePoints["PercentageCPU"] += 2;
                    AmazonPoints["PercentageCPU"] += 1;
                }
            }
            else if (AmazonResults.PercentageCPU <= GoogleResults.PercentageCPU)
            {
                AmazonPoints["PercentageCPU"] += 3;
                if (AzureResults.PercentageCPU <= GoogleResults.PercentageCPU)
                {
                    AzurePoints["PercentageCPU"] += 2;
                    GooglePoints["PercentageCPU"] += 1;
                }
                else
                {
                    GooglePoints["PercentageCPU"] += 2;
                    AzurePoints["PercentageCPU"] += 1;
                }
            }
            else
            {
                GooglePoints["PercentageCPU"] += 3;
                if (AzureResults.PercentageCPU <= AmazonResults.PercentageCPU)
                {
                    AzurePoints["PercentageCPU"] += 2;
                    AmazonPoints["PercentageCPU"] += 1;
                }
                else
                {
                    AmazonPoints["PercentageCPU"] += 2;
                    AzurePoints["PercentageCPU"] += 1;
                }
            }
        }

        private void AddPercentageMemoryPoints(MetricsResultsModel AzureResults, MetricsResultsModel AmazonResults, MetricsResultsModel GoogleResults)
        {
            if (AzureResults.PercentageMemory <= AmazonResults.PercentageMemory && AzureResults.PercentageMemory <= GoogleResults.PercentageMemory)
            {
                AzurePoints["PercentageMemory"] += 3;
                if (AmazonResults.PercentageMemory <= GoogleResults.PercentageMemory)
                {
                    AmazonPoints["PercentageMemory"] += 2;
                    GooglePoints["PercentageMemory"] += 1;
                }
                else
                {
                    GooglePoints["PercentageMemory"] += 2;
                    AmazonPoints["PercentageMemory"] += 1;
                }
            }
            else if (AmazonResults.PercentageMemory <= GoogleResults.PercentageMemory)
            {
                AmazonPoints["PercentageMemory"] += 3;
                if (AzureResults.PercentageMemory <= GoogleResults.PercentageMemory)
                {
                    AzurePoints["PercentageMemory"] += 2;
                    GooglePoints["PercentageMemory"] += 1;
                }
                else
                {
                    GooglePoints["PercentageMemory"] += 2;
                    AzurePoints["PercentageMemory"] += 1;
                }
            }
            else
            {
                GooglePoints["PercentageMemory"] += 3;
                if (AzureResults.PercentageMemory <= AmazonResults.PercentageMemory)
                {
                    AzurePoints["PercentageMemory"] += 2;
                    AmazonPoints["PercentageMemory"] += 1;
                }
                else
                {
                    AmazonPoints["PercentageMemory"] += 2;
                    AzurePoints["PercentageMemory"] += 1;
                }
            }
        }

        private void AddIncomingTrafficPoints(MetricsResultsModel AzureResults, MetricsResultsModel AmazonResults, MetricsResultsModel GoogleResults)
        {
            if (AzureResults.IncomingTraffic >= AmazonResults.IncomingTraffic && AzureResults.IncomingTraffic >= GoogleResults.IncomingTraffic)
            {
                AzurePoints["IncomingTraffic"] += 3;
                if (AmazonResults.IncomingTraffic >= GoogleResults.IncomingTraffic)
                {
                    AmazonPoints["IncomingTraffic"] += 2;
                    GooglePoints["IncomingTraffic"] += 1;
                }
                else
                {
                    GooglePoints["IncomingTraffic"] += 2;
                    AmazonPoints["IncomingTraffic"] += 1;
                }
            }
            else if (AmazonResults.IncomingTraffic >= GoogleResults.IncomingTraffic)
            {
                AmazonPoints["IncomingTraffic"] += 3;
                if (AzureResults.IncomingTraffic >= GoogleResults.IncomingTraffic)
                {
                    AzurePoints["IncomingTraffic"] += 2;
                    GooglePoints["IncomingTraffic"] += 1;
                }
                else
                {
                    GooglePoints["IncomingTraffic"] += 2;
                    AzurePoints["IncomingTraffic"] += 1;
                }
            }
            else
            {
                GooglePoints["IncomingTraffic"] += 3;
                if (AzureResults.IncomingTraffic >= AmazonResults.IncomingTraffic)
                {
                    AzurePoints["IncomingTraffic"] += 2;
                    AmazonPoints["IncomingTraffic"] += 1;
                }
                else
                {
                    AmazonPoints["IncomingTraffic"] += 2;
                    AzurePoints["IncomingTraffic"] += 1;
                }
            }
        }

        private void AddOutcomingTrafficPoints(MetricsResultsModel AzureResults, MetricsResultsModel AmazonResults, MetricsResultsModel GoogleResults)
        {
            if (AzureResults.OutcomingTraffic >= AmazonResults.OutcomingTraffic && AzureResults.OutcomingTraffic >= GoogleResults.OutcomingTraffic)
            {
                AzurePoints["OutcomingTraffic"] += 3;
                if (AmazonResults.OutcomingTraffic >= GoogleResults.OutcomingTraffic)
                {
                    AmazonPoints["OutcomingTraffic"] += 2;
                    GooglePoints["OutcomingTraffic"] += 1;
                }
                else
                {
                    GooglePoints["OutcomingTraffic"] += 2;
                    AmazonPoints["OutcomingTraffic"] += 1;
                }
            }
            else if (AmazonResults.OutcomingTraffic >= GoogleResults.OutcomingTraffic)
            {
                AmazonPoints["OutcomingTraffic"] += 3;
                if (AzureResults.OutcomingTraffic >= GoogleResults.OutcomingTraffic)
                {
                    AzurePoints["OutcomingTraffic"] += 2;
                    GooglePoints["OutcomingTraffic"] += 1;
                }
                else
                {
                    GooglePoints["OutcomingTraffic"] += 2;
                    AzurePoints["OutcomingTraffic"] += 1;
                }
            }
            else
            {
                GooglePoints["OutcomingTraffic"] += 3;
                if (AzureResults.OutcomingTraffic >= AmazonResults.OutcomingTraffic)
                {
                    AzurePoints["OutcomingTraffic"] += 2;
                    AmazonPoints["OutcomingTraffic"] += 1;
                }
                else
                {
                    AmazonPoints["OutcomingTraffic"] += 2;
                    AzurePoints["OutcomingTraffic"] += 1;
                }
            }
        }

        private Dictionary<string, string> CalculateRank()
        {
            return new Dictionary<string, string>
            {
                { "PercentageCPU", CalculateRankByMetric("PercentageCPU") },
                { "PercentageMemory", CalculateRankByMetric("PercentageMemory") },
                { "IncomingTraffic", CalculateRankByMetric("IncomingTraffic") },
                { "OutcomingTraffic", CalculateRankByMetric("OutcomingTraffic") }
            };
        }

        private string CalculateRankByMetric(string Metric)
        {
            string result;

            if (AzurePoints[Metric] >= AmazonPoints[Metric] && AzurePoints[Metric] >= GooglePoints[Metric])
            {
                if (AmazonPoints[Metric] >= GooglePoints[Metric])
                {
                    result = "Azure,Amazon,Google";
                }
                else
                {
                    result = "Azure,Google,Amazon";
                }
            }
            else if(AmazonPoints[Metric] >= GooglePoints[Metric])
            {
                if(AzurePoints[Metric] >= GooglePoints[Metric])
                {
                    result = "Amazon,Azure,Google";
                }
                else
                {
                    result = "Amazon,Google,Azure";
                }
            }
            else
            {
                if (AzurePoints[Metric] >= AmazonPoints[Metric])
                {
                    result = "Google,Azure,Amazon";
                }
                else
                {
                    result = "Google,Amazon,Azure";
                }
            }

            return result;
        }
    }
}
