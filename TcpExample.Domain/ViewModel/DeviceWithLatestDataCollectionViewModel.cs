using System;
using System.Collections.Generic;
using System.Text;
using TcpExample.Domain.DBModel;

namespace TcpExample.Domain
{
   public  class DeviceWithLatestDataCollectionViewModel
    {
        public Mark Mark { get; set; }
        public DataCollection DataCollection { get; set; }
        public long SerialNumber { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public bool IsDelete { get; set; }
    }
}
