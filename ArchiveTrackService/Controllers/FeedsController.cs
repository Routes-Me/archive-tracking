using System;
using System.Collections.Generic;
using ArchiveTrackService.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using Microsoft.AspNetCore.Mvc;

namespace ArchiveTrackService.Controllers
{
    [ApiController]
    [Route("api")]
    public class FeedsController : ControllerBase
    {
        private readonly ICoordinateRepository _coordinateRepository;
        public FeedsController(ICoordinateRepository coordinateRepository)
        {
            _coordinateRepository = coordinateRepository;
        }

        [HttpGet]
        [Route("feeds")]
        public IActionResult Get(string vehicleIds, DateTime? start, DateTime? end, string include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _coordinateRepository.getCoordinates(vehicleIds, start, end, include, pageInfo);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpPost]
        [Route("feeds")]
        public IActionResult Post(List<Coordinates> Model)
        {
            dynamic response = _coordinateRepository.InsertCoordinates(Model);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("feeds")]
        public IActionResult Delete(string vehicleIds, DateTime? start, DateTime? end)
        {
            dynamic response = _coordinateRepository.DeleteCoordinates(vehicleIds, start, end);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
