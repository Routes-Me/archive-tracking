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
        dynamic GetCoordinates(string id, string include, Pagination pageInfo);
        dynamic InsertCoordinates(List<Coordinates> coordinates);
        dynamic DeleteCoordinates(string id);
    }
}
