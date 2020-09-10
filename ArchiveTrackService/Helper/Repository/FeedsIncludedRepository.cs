using ArchiveTrackService.Helper.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ArchiveTrackService.Helper.Repository
{
    public class FeedsIncludedRepository : IFeedsIncludedRepository
    {
        private readonly IOptions<AppSettings> _appSettings;
        public FeedsIncludedRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public dynamic GetDevicesIncludedData(List<Coordinates> objCoordinateList)
        {
            List<DevicesModel> lstDevices = new List<DevicesModel>();
            foreach (var item in objCoordinateList)
            {
                var client = new RestClient(_appSettings.Value.DevicesEndpointUrl + item.DeviceId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var deviceData = JsonConvert.DeserializeObject<DeviceData>(result);
                    lstDevices.AddRange(deviceData.data);
                }
            }
            var deviceList = lstDevices.GroupBy(x => x.DeviceId).Select(a => a.First()).ToList();
            var deviceJson = JsonConvert.SerializeObject(deviceList,
                                   new JsonSerializerSettings
                                   {
                                       NullValueHandling = NullValueHandling.Ignore,
                                   });

            return JArray.Parse(deviceJson);
        }

        public dynamic GetVehiclesIncludedData(List<Coordinates> objCoordinateList)
        {
            List<VehiclesModel> lstVehicle = new List<VehiclesModel>();
            foreach (var item in objCoordinateList)
            {
                var client = new RestClient(_appSettings.Value.VehicleEndpointUrl + item.VehicleId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var vehicleData = JsonConvert.DeserializeObject<VehicleData>(result);
                    lstVehicle.AddRange(vehicleData.data);
                }
            }
            var vehicleList = lstVehicle.GroupBy(x => x.VehicleId).Select(a => a.First()).ToList();
            var vehicleJson = JsonConvert.SerializeObject(vehicleList,
                                   new JsonSerializerSettings
                                   {
                                       NullValueHandling = NullValueHandling.Ignore,
                                   });

            return JArray.Parse(vehicleJson);
        }
    }
}
