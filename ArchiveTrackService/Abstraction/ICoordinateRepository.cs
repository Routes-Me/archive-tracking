using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using ArchiveTrackService.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveTrackService.Abstraction
{
    public interface ICoordinateRepository
    {
        dynamic GetCoordinates(string coordinateId, string include, Pagination pageInfo);
        dynamic InsertCoordinates(List<CoordinatesModel> coordinates);
        dynamic DeleteCoordinates(string coordinateId);
        Task SyncOperationLogs();
    }
}
