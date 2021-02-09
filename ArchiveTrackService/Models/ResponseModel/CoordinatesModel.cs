using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveTrackService.Models.ResponseModel
{
    public class CoordinatesModel
    {
        public string CoordinateId { get; set; }
        public int DeviceId { get; set; }
        public int? VehicleId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
