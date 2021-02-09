using System;

namespace ArchiveTrackService.Models.ResponseModel
{
    public class OperationLogsDto
    {
        public string DeviceId { get; set; }
        public float Duration { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
