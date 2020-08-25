using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveTrackService.Models
{
    public class DevicesModel
    {
        public int DeviceId { get; set; }
        public string SerialNumber { get; set; }
        public string SimSerialNumber { get; set; }
        public int? VehicleId { get; set; }
    }
}
