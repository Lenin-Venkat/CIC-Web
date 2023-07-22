using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class AzureStorageConfiguration
    {
        public string StorageConnectionString { get; set; }
        public string StorageAccount { get; set; }
        public string StorageKey1 { get; set; }
        public string StorageKey2 { get; set; }
        public string MyProperty { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AadTenantId { get; set; }
        public string Authority { get; set; }
        public string BCURL { get; set; }
        public string TokenURL { get; set; }
    }
}
