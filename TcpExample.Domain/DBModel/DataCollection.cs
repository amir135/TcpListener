using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TcpExample.Domain.DBModel
{
    public class DataCollection:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
       // [ForeignKey("Device"), Column(Order = 0)]
        public long SerialNumber { get; set; }
       // [ForeignKey("Device"), Column(Order = 1)]
        public string MarkCode { get; set; }
        public int Signal { get; set; }

        public virtual Device Device { get; set; }
        
    }
}
