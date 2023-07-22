using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class CertMasterModel
    {
       public int BuildingNo { get; set; }
       public int CivilNo { get; set; }
       public int MechanicalNo { get; set; }
       public int ElectricalNo { get; set; }
       public string PartitionKey { get; set; }
       public string RowKey { get; set; }

       public string Timestamp { get; set; }
    }

    public class CertForm4Model
    {
        public int CivilNo { get; set; }
        public int MechanicalNo { get; set; }
        public int ElectricalNo { get; set; }
        public int ArchitectureNo { get; set; }
        public int QuantityNo { get; set; }
        public int AlliedNo { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string Timestamp { get; set; }
    }

    public class CertForm7Model
    {
        public int ManufacturersNo { get; set; }
        public int SuppliersNo { get; set; }
        
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string Timestamp { get; set; }
    }

    public class CertForm6Model
    {
        public int ArtisanNo { get; set; }
       
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string Timestamp { get; set; }
    }

    public class CertForm3Model
    {
        public int JVNo { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string Timestamp { get; set; }
    }

}
