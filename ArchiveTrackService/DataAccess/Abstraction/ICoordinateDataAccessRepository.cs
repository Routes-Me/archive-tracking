using ArchiveTrackService.Models;
using ArchiveTrackService.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveTrackService.DataAccess.Abstraction
{
    public interface ICoordinateDataAccessRepository
    {
        dynamic GetCoordinates(string coordinateId, string includeType, Pagination pageInfo);
        dynamic InsertCoordinates(List<CoordinatesModel> coordinates);
        dynamic DeleteCoordinates(string coordinateId);
    }
}
