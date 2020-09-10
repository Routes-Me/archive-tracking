using ArchiveTrackService.Models.DBModels;
using System.Collections.Generic;

namespace ArchiveTrackService.Helper.Abstraction
{
    public interface IFeedsIncludedRepository
    {
        dynamic GetVehiclesIncludedData(List<Coordinates> objCoordinateList);
        dynamic GetDevicesIncludedData(List<Coordinates> objCoordinateList);
    }
}
