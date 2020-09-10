using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveTrackService.Abstraction
{
    public interface ICoordinateRepository
    {
        dynamic getCoordinates(string vehicleIds, DateTime? start, DateTime? end, string include, Pagination pageInfo);
        dynamic InsertCoordinates(List<Coordinates> model);
        dynamic DeleteCoordinates(string vehicleIds, DateTime? start, DateTime? end);
    }
}
