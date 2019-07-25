using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneAzureWebAPI.Models
{
    public class VirtualMachine
    {
        public string subscriptionId { get; set; }
        public string resourcegroupName { get; set; }
        public string virtualmachineName { get; set; }
        public string apiVersion { get; set; }
    }
}