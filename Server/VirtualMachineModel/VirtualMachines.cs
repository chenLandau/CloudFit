namespace Server.VirtualMachineModel
{
    public class VirtualMachines
    {
        public List<AzureVirtualMachine> AzureVirtualMachine { get; set; }
        public List<GoogleVirtualMachine> GoogleVirtualMachine { get; set; }
        public List<AmazonVirtualMachine> AmazonVirtualMachine { get; set; }
    }
}
