using ArchiveTrackService.DataAccess.Abstraction;
using ArchiveTrackService.Helper;
using ArchiveTrackService.Helper.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using ArchiveTrackService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace ArchiveTrackService.DataAccess.Repository
{
    public class CoordinateDataAccessRepository : ICoordinateDataAccessRepository
    {
        private readonly archivetrackserviceContext _context;
        private readonly IFeedsIncludedRepository _feedsIncludedRepository;
        public CoordinateDataAccessRepository(archivetrackserviceContext context, IFeedsIncludedRepository feedsIncludedRepository)
        {
            _context = context;
            _feedsIncludedRepository = feedsIncludedRepository;
        }

        public dynamic DeleteCoordinates(string coordinateId)
        {
            List<Coordinates> coordinates = new List<Coordinates>();
            if (string.IsNullOrEmpty(coordinateId))
                coordinates = _context.Coordinates.ToList();
            else
                coordinates = _context.Coordinates.Where(x => x.CoordinateId == coordinateId).ToList();

            if (coordinates.Count == 0)
                Common.ThrowException(CommonMessage.FeedNotFound, StatusCodes.Status404NotFound);

            _context.Coordinates.RemoveRange(coordinates);
            _context.SaveChanges();
            return ReturnResponse.SuccessResponse(CommonMessage.FeedDelete, false);
        }

        public dynamic GetCoordinates(string coordinateId, string includeType, Pagination pageInfo)
        {
            int totalCount = 0;
            feedGetResponse response = new feedGetResponse();
            List<Coordinates> coordinatesList = new List<Coordinates>();
            if (string.IsNullOrEmpty(coordinateId) || coordinateId == "0")
            {
                coordinatesList = _context.Coordinates.OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();
                totalCount = _context.Coordinates.ToList().Count();
            }
            else
            {
                coordinatesList = _context.Coordinates.Where(x => x.CoordinateId == coordinateId).OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();
                totalCount = _context.Coordinates.Where(x => x.CoordinateId == coordinateId).ToList().Count();
            }

            var page = new Pagination
            {
                offset = pageInfo.offset,
                limit = pageInfo.limit,
                total = totalCount
            };

            dynamic includeData = new JObject();
            if (!string.IsNullOrEmpty(includeType))
            {
                string[] includeArr = includeType.Split(',');
                if (includeArr.Length > 0)
                {
                    foreach (var item in includeArr)
                    {
                        if (item.ToLower() == "vehicle" || item.ToLower() == "vehicles")
                        {
                            includeData.vehicles = _feedsIncludedRepository.GetVehiclesIncludedData(coordinatesList);
                        }
                        else if (item.ToLower() == "device" || item.ToLower() == "devices")
                        {
                            includeData.devices = _feedsIncludedRepository.GetDevicesIncludedData(coordinatesList);
                        }
                    }
                }
            }

            if (((JContainer)includeData).Count == 0)
                includeData = null;

            response.status = true;
            response.message = CommonMessage.FeedRetrived;
            response.pagination = page;
            response.data = coordinatesList;
            response.included = includeData;
            response.statusCode = StatusCodes.Status200OK;
            return response;
        }

        public dynamic InsertCoordinates(List<CoordinatesModel> coordinates)
        {
            List<Coordinates> coordinatesList = new List<Coordinates>();
            if (coordinates.Count == 0)
                Common.ThrowException(CommonMessage.EmptyModel, StatusCodes.Status422UnprocessableEntity);

            foreach (var item in coordinates)
            {
                Coordinates objCoordinates = new Coordinates();
                objCoordinates.CoordinateId = item.CoordinateId;
                objCoordinates.DeviceId = item.DeviceId;
                objCoordinates.VehicleId = item.VehicleId;
                objCoordinates.Latitude = item.Latitude;
                objCoordinates.Longitude = item.Longitude;
                objCoordinates.CreatedAt = item.Timestamp;
                objCoordinates.ArchivedAt = DateTime.Now;
                coordinatesList.Add(objCoordinates);
            }
            _context.Coordinates.AddRange(coordinatesList);
            _context.SaveChanges();
            return ReturnResponse.SuccessResponse(CommonMessage.FeedInsert, true);
        }
    }
}
