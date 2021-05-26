using ArchiveTrackService.Abstraction;
using ArchiveTrackService.DataAccess.Abstraction;
using ArchiveTrackService.Helper.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.Common;
using ArchiveTrackService.Models.DBModels;
using ArchiveTrackService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoutesSecurity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RestSharp;

namespace ArchiveTrackService.Repository
{
    public class CoordinateRepository : ICoordinateRepository
    {
        private readonly ArchiveTrackServiceContext _context;
        private readonly ICoordinateDataAccessRepository _coordinateDataAccessRepository;
        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;
        public CoordinateRepository(ArchiveTrackServiceContext context,ICoordinateDataAccessRepository coordinateDataAccessRepository,
                IOptions<AppSettings> appSettings, IOptions<Dependencies> dependencies)
        {
            _context = context;
            _coordinateDataAccessRepository = coordinateDataAccessRepository;
            _appSettings = appSettings.Value;
            _dependencies = dependencies.Value;
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

        public async Task SyncOperationLogs()
        {
            List<OperationLogsDto> operationLogs = new List<OperationLogsDto>();
            List<Coordinates> coordinates = _context.Coordinates.Where(c => c.CreatedAt.Value.Day == DateTime.Now.Day).ToList();

            foreach (var coordinatesGroup in coordinates.GroupBy(g => g.DeviceId))
            {
                operationLogs.Add(new OperationLogsDto
                {
                    DeviceId = Obfuscation.Encode(coordinatesGroup.FirstOrDefault().DeviceId).ToString(),
                    Duration = CalculateRunningTime(coordinatesGroup.ToArray()),
                    Date = DateTime.Now.Date
                });
            }
            await PostAPI(_dependencies.VehicleAnalyticsUrl, operationLogs);
        }

        public dynamic InsertCoordinatesForVehicle(string vehicleId, List<CoordinatesOfVehicleDto> coordinates)
        {
            if (coordinates == null || !coordinates.Any())
                throw new ArgumentNullException(CommonMessage.InvalidDataPassed);

            int vehicleIdDecrypted = Obfuscation.Decode(vehicleId);
            List<Coordinates> coordinatesList = new List<Coordinates>();
            coordinates.ForEach(c => {
                coordinatesList.Add(new Coordinates() {
                    VehicleId = vehicleIdDecrypted,
                    Longitude = c.Longitude,
                    Latitude = c.Latitude,
                    ArchivedAt = UnixTimeStampToDateTime(c.Timestamp.ToString()),
                    CreatedAt = DateTime.Now
                });
            });
            return coordinatesList;
        }

        private DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(unixTimeStamp)).ToLocalTime();
            return dtDateTime;
        }

        private async Task PostAPI(string url, dynamic objectToSend)
        {
            var client = new RestClient(_appSettings.Host + url);
                    var request = new RestRequest(Method.POST);
                    string jsonToSend = JsonConvert.SerializeObject(objectToSend);
                    request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
                    request.RequestFormat = DataFormat.Json;
                    IRestResponse response = client.Execute(request);
            await Task.CompletedTask;
        }

        private float CalculateRunningTime(Coordinates[] deviceCoordinates)
        {
            double runningTime = 0;

            for (int i = 0; i < deviceCoordinates.Length-1; i++)
            {
                TimeSpan? diff = deviceCoordinates[i+1].CreatedAt - deviceCoordinates[i].CreatedAt;
                if (diff.Value.TotalMilliseconds < 600000)
                    runningTime += diff.Value.TotalMilliseconds;
            }
            return (float)runningTime;
        }
    }
}
