namespace ArchiveTrackService.Models.ResponseModel
{
    public class CoordinatesOfVehicleDto
    {
        public string CoordinateId { get; set; }
        public string VehicleId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public long Timestamp { get; set; }
    }
}
