using ArchiveTrackService.DataAccess.Abstraction;
using ArchiveTrackService.Helper;
using ArchiveTrackService.Helper.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using ArchiveTrackService.Models.ResponseModel;
using RoutesSecurity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchiveTrackService.DataAccess.Repository
{
    public class CoordinateDataAccessRepository : ICoordinateDataAccessRepository
    {
        private readonly ArchiveTrackServiceContext _context;
        private readonly IFeedsIncludedRepository _feedsIncludedRepository;
        public CoordinateDataAccessRepository(ArchiveTrackServiceContext context, IFeedsIncludedRepository feedsIncludedRepository)
        {
            _context = context;
            _feedsIncludedRepository = feedsIncludedRepository;
        }

        public dynamic DeleteCoordinates(string coordinateId)
        {
            if (string.IsNullOrEmpty(coordinateId))
                Common.ThrowException(CommonMessage.InvalidDataPassed, StatusCodes.Status422UnprocessableEntity);

            Coordinates coordinate = _context.Coordinates.Where(x => x.CoordinateId == Obfuscation.Decode(coordinateId)).FirstOrDefault();
            if (coordinate == null)
                Common.ThrowException(CommonMessage.FeedNotFound, StatusCodes.Status404NotFound);

            _context.Coordinates.Remove(coordinate);
            _context.SaveChanges();
            return ReturnResponse.SuccessResponse(CommonMessage.FeedDelete, false);
        }

        public dynamic GetCoordinates(string coordinateId, string includeType, Pagination pageInfo)
        {
            int totalCount = 0;
            feedGetResponse response = new feedGetResponse();
            List<Coordinates> coordinatesList = new List<Coordinates>();
            if (string.IsNullOrEmpty(coordinateId))
            {
                coordinatesList = _context.Coordinates.OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();
                totalCount = _context.Coordinates.ToList().Count();
            }
            else
            {
                coordinatesList = _context.Coordinates.Where(x => x.CoordinateId == Obfuscation.Decode(coordinateId)).OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();
                totalCount = _context.Coordinates.Where(x => x.CoordinateId == Obfuscation.Decode(coordinateId)).ToList().Count();
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
            response.message = CommonMessage.FeedsRetrived;
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
            return ReturnResponse.SuccessResponse(CommonMessage.FeedsInsert, true);
        }
    }
}
