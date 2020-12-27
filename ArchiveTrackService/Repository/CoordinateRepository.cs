using ArchiveTrackService.Abstraction;
using ArchiveTrackService.DataAccess.Abstraction;
using ArchiveTrackService.Helper.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using ArchiveTrackService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ArchiveTrackService.Repository
{
    public class CoordinateRepository : ICoordinateRepository
    {
        private readonly ICoordinateDataAccessRepository _coordinateDataAccessRepository;
        public CoordinateRepository(ICoordinateDataAccessRepository coordinateDataAccessRepository)
        {
            _coordinateDataAccessRepository = coordinateDataAccessRepository;
        }

        public dynamic GetCoordinates(string coordinateId, string includeType, Pagination pageInfo)
        {
            try
            {
                return _coordinateDataAccessRepository.GetCoordinates(coordinateId, includeType, pageInfo);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex); 
            }
        }

        public dynamic InsertCoordinates(List<CoordinatesModel> coordinates)
        {
            try
            {
                return _coordinateDataAccessRepository.InsertCoordinates(coordinates);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic DeleteCoordinates(string coordinateId)
        {
            try
            {
                return _coordinateDataAccessRepository.DeleteCoordinates(coordinateId);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
