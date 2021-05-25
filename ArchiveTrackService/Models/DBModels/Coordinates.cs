﻿using System;
using System.Collections.Generic;

namespace ArchiveTrackService.Models.DBModels
{
    public partial class Coordinates
    {
        public string CoordinateId { get; set; }
        public int DeviceId { get; set; }
        public int? VehicleId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }
}
