using ArchiveTrackService.Abstraction;
using ArchiveTrackService.Helper.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
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
        private readonly archivetrackserviceContext _context;
        private readonly IFeedsIncludedRepository _feedsIncludedRepository;
        public CoordinateRepository(archivetrackserviceContext context, IFeedsIncludedRepository feedsIncludedRepository)
        {
            _context = context;
            _feedsIncludedRepository = feedsIncludedRepository;
        }

        public dynamic GetCoordinates(string id, string includeType, Pagination pageInfo)
        {
            try
            {
                int totalCount = 0;
                feedGetResponse response = new feedGetResponse();
                List<Coordinates> coordinatesList = new List<Coordinates>();
                if (string.IsNullOrEmpty(id))
                {
                    coordinatesList = _context.Coordinates.OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();
                    totalCount = _context.Coordinates.ToList().Count();
                }
                else
                {
                    coordinatesList = _context.Coordinates.Where(x => x.CoordinateId == id).OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();
                    totalCount = _context.Coordinates.Where(x => x.CoordinateId == id).ToList().Count();
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
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertCoordinates(List<Coordinates> coordinates)
        {
            try
            {
                List<Coordinates> coordinatesList = new List<Coordinates>();
                if (coordinates.Count == 0)
                    ReturnResponse.ThrowException(CommonMessage.FeedNotFound, StatusCodes.Status404NotFound);

                foreach (var item in coordinates)
                {
                    Coordinates objCoordinates = new Coordinates();
                    objCoordinates.CoordinateId = item.CoordinateId;
                    objCoordinates.DeviceId = item.DeviceId;
                    objCoordinates.VehicleId = item.VehicleId;
                    objCoordinates.Latitude = item.Latitude;
                    objCoordinates.Longitude = item.Longitude;
                    objCoordinates.CreatedAt = item.ArchivedAt;
                    objCoordinates.ArchivedAt = DateTime.Now;
                    coordinatesList.Add(objCoordinates);
                }
                _context.Coordinates.AddRange(coordinatesList);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.FeedInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic DeleteCoordinates(string id)
        {
            try
            {
                var coordinates = _context.Coordinates.Where(x => x.CoordinateId == id).FirstOrDefault();
                if (coordinates == null)
                    ReturnResponse.ThrowException(CommonMessage.FeedNotFound, StatusCodes.Status404NotFound);

                _context.Coordinates.Remove(coordinates);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.FeedDelete, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
