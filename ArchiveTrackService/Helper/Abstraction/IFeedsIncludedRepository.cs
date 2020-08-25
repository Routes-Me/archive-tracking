using ArchiveTrackService.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveTrackService.Helper.Abstraction
{
    public interface IFeedsIncludedRepository
    {
        dynamic GetVehiclesIncludedData(List<Coordinates> objCoordinateList);
        dynamic GetDevicesIncludedData(List<Coordinates> objCoordinateList);
    }
}
