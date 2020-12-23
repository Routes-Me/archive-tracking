using ArchiveTrackService.Abstraction;
using ArchiveTrackService.Helper.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public dynamic getCoordinates(string vehicleIds, DateTime? startDate, DateTime? endDate, string includeType, Pagination pageInfo)
        {
            try
            {
                int totalCount = 0;
                feedGetResponse response = new feedGetResponse();
                List<Coordinates> objCoordinateList = new List<Coordinates>();
                if (!string.IsNullOrEmpty(vehicleIds))
                {
                    List<Int32?> VehicleIds = new List<Int32?>();
                    string[] VehicleArr = vehicleIds.Split(',');
                    if (VehicleArr.Length > 0)
                    {
                        foreach (var item in VehicleArr)
                        {
                            VehicleIds.Add(Convert.ToInt32(item));
                        }
                    }
                    if (VehicleIds != null)
                    {
                        if (startDate != null && endDate != null)
                        {
                            objCoordinateList = _context.Coordinates.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate && VehicleIds.Contains(x.VehicleId))
                           .OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coordinates.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate && VehicleIds.Contains(x.VehicleId)).ToList().Count();
                        }
                        else
                        {
                            objCoordinateList = (from ac in _context.Coordinates
                                                 where VehicleIds.Contains(Convert.ToInt32(ac.VehicleId))
                                                 select ac
                                           ).OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = (from ac in _context.Coordinates
                                          where VehicleIds.Contains(Convert.ToInt32(ac.VehicleId))
                                          select ac
                                           ).ToList().Count();
                        }
                    }
                }
                else
                {
                    if (startDate != null && endDate != null)
                    {
                        objCoordinateList = _context.Coordinates.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                            .OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Coordinates.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList().Count();
                    }
                    else
                    {
                        objCoordinateList = _context.Coordinates.OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Coordinates.ToList().Count();
                    }
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
                                includeData.vehicles = _feedsIncludedRepository.GetVehiclesIncludedData(objCoordinateList);
                            }
                            else if (item.ToLower() == "device" || item.ToLower() == "devices")
                            {
                                includeData.devices = _feedsIncludedRepository.GetDevicesIncludedData(objCoordinateList);
                            }
                        }
                    }
                }

                if (((JContainer)includeData).Count == 0)
                    includeData = null;

                response.status = true;
                response.message = CommonMessage.FeedRetrived;
                response.pagination = page;
                response.data = objCoordinateList;
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
                List<Coordinates> lstCoordinates = new List<Coordinates>();
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
                    lstCoordinates.Add(objCoordinates);
                }
                _context.Coordinates.AddRange(lstCoordinates);
                _context.SaveChanges();
                return ReturnResponse.SuccessResponse(CommonMessage.FeedInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic DeleteCoordinates(string vehicleIds, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                List<Coordinates> objCoordinateList = new List<Coordinates>();
                List<Coordinates> tempCoordinateList;
                if (!string.IsNullOrEmpty(vehicleIds))
                {
                    string[] VehicleArr = vehicleIds.Split(',');
                    if (VehicleArr.Length > 0)
                    {
                        foreach (var item in VehicleArr)
                        {
                            tempCoordinateList = new List<Coordinates>();
                            if (startDate != null && endDate != null)
                                tempCoordinateList = _context.Coordinates.Where(x => x.VehicleId == Convert.ToInt32(item) && x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();
                            else
                                tempCoordinateList = _context.Coordinates.Where(x => x.VehicleId == Convert.ToInt32(item)).ToList();

                            objCoordinateList.AddRange(tempCoordinateList);
                        }
                    }
                }
                else
                {
                    if (startDate != null && endDate != null)
                        objCoordinateList = _context.Coordinates.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();
                    else
                        objCoordinateList = _context.Coordinates.ToList();
                }
                if (objCoordinateList == null || objCoordinateList.ToList().Count == 0)
                    return ReturnResponse.ErrorResponse(CommonMessage.FeedNotFound, StatusCodes.Status404NotFound);

                if (objCoordinateList != null)
                {
                    _context.Coordinates.RemoveRange(objCoordinateList);
                    _context.SaveChanges();
                }
                return ReturnResponse.SuccessResponse(CommonMessage.FeedDelete, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
    }
}
