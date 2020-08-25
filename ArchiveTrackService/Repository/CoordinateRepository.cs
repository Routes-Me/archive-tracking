using ArchiveTrackService.Abstraction;
using ArchiveTrackService.Helper.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveTrackService.Repository
{
    public class CoordinateRepository : ICoordinateRepository
    {
        private readonly archivetrackserviceContext _context;
        private readonly  IFeedsIncludedRepository _feedsIncludedRepository;
        public CoordinateRepository(archivetrackserviceContext context, IFeedsIncludedRepository feedsIncludedRepository)
        {
            _context = context;
            _feedsIncludedRepository = feedsIncludedRepository;
        }

        public feedGetResponse getCoordinates(string vehicleIds, DateTime? startDate, DateTime? endDate, string includeType, Pagination pageInfo)
        {
            feedGetResponse response = new feedGetResponse();
            int totalCount = 0;
            try
            {
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
                            objCoordinateList = _context.Coordinates.Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate && VehicleIds.Contains(x.VehicleId))
                           .OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                            totalCount = _context.Coordinates.Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate && VehicleIds.Contains(x.VehicleId)).ToList().Count();
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
                        objCoordinateList = _context.Coordinates.Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate)
                            .OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Coordinates.Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate).ToList().Count();
                    }
                    else
                    {
                        objCoordinateList = _context.Coordinates.OrderBy(a => a.CoordinateId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.Coordinates.ToList().Count();
                    }
                }
                if (objCoordinateList == null || objCoordinateList.Count == 0)
                {
                    response.status = false;
                    response.message = "Feeds not found.";
                    response.data = null;

                    response.responseCode = ResponseCode.NotFound;
                    return response;
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
                response.message = "Feeds retrived successfully.";
                response.pagination = page;
                response.data = objCoordinateList;
                response.included = includeData;
                response.responseCode = ResponseCode.Success;
                return response;
            }
            catch (Exception ex)
            {
                response.status = false;
                response.message = "Something went wrong while getting feeds. Error Message - " + ex.Message;
                response.data = null;
                response.responseCode = ResponseCode.InternalServerError;
                return response;
            }
        }

        public feedResponse InsertCoordinates(List<Coordinates> Model)
        {
            feedResponse response = new feedResponse();
            try
            {
                if (Model == null)
                {
                    response.status = false;
                    response.message = "Pass valid data in model.";
                    response.responseCode = ResponseCode.BadRequest;
                    return response;
                }
                _context.Coordinates.AddRange(Model);
                _context.SaveChanges();
                response.status = true;
                response.message = "Coordinates inserted successfully.";
                response.responseCode = ResponseCode.Created;
                return response;
            }
            catch (Exception ex)
            {
                response.status = false;
                response.message = "Something went wrong while inserting feeds. Error Message - " + ex.Message;
                response.responseCode = ResponseCode.InternalServerError;
                return response;
            }
        }

        public feedResponse DeleteCoordinates(string vehicleIds, DateTime? startDate, DateTime? endDate)
        {
            feedResponse response = new feedResponse();
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
                                tempCoordinateList = _context.Coordinates.Where(x => x.VehicleId == Convert.ToInt32(item) && x.Timestamp >= startDate && x.Timestamp <= endDate).ToList();
                            else
                                tempCoordinateList = _context.Coordinates.Where(x => x.VehicleId == Convert.ToInt32(item)).ToList();

                            objCoordinateList.AddRange(tempCoordinateList);
                        }
                    }
                }
                else
                {
                    if (startDate != null && endDate != null)
                        objCoordinateList = _context.Coordinates.Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate).ToList();
                    else
                        objCoordinateList = _context.Coordinates.ToList();
                }
                if (objCoordinateList == null || objCoordinateList.ToList().Count == 0)
                {
                    response.status = false;
                    response.message = "Feeds not found.";
                    response.responseCode = ResponseCode.NotFound;
                    return response;
                }
                if (objCoordinateList != null)
                {
                    _context.Coordinates.RemoveRange(objCoordinateList);
                    _context.SaveChanges();
                }
                response.status = true;
                response.message = "Feeds deleted successfully.";
                response.responseCode = ResponseCode.Success;
                return response;
            }
            catch (Exception ex)
            {
                response.status = false;
                response.message = "Something went wrong while deleting feeds. Error Message - " + ex.Message;
                response.responseCode = ResponseCode.InternalServerError;
                return response;
            }
        }
    }
}
