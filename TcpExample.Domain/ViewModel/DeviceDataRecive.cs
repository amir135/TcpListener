using System;
using System.Collections.Generic;
using System.Text;

namespace TcpExample.Domain.ViewModel
{
    public class DeviceDataRecive
    {
        public string MarkCode { get; set; }
        public long SerialNumber { get; set; }

        public int Signal { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public DateTime DateTime { get; set; }


    }
}
