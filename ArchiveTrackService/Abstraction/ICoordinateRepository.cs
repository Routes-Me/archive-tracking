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
        feedGetResponse getCoordinates(string vehicleIds, DateTime? start, DateTime? end, string include, Pagination pageInfo);
        feedResponse InsertCoordinates(List<Coordinates> model);
        feedResponse DeleteCoordinates(string vehicleIds, DateTime? start, DateTime? end);
    }
}
