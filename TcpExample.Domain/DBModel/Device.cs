using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace TcpExample.Domain.DBModel
{
    public class Device : BaseEntity
    {
        //[Key, Column(Order = 0)]
        public long SerialNumber { get; set; }
        [ForeignKey("Mark")]
       // [Key, Column(Order = 1)]
        public string MarkCode { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }

        public virtual Mark Mark { get; set; }
        [JsonIgnore]
        public List<DataCollection> DataCollections { get; set; }

    }
}
