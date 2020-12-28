using System;
using System.Collections.Generic;
using ArchiveTrackService.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using ArchiveTrackService.Models.ResponseModel;
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
        [Route("feeds/{coordinateId?}")]
        public IActionResult Get(string coordinateId, string include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _coordinateRepository.GetCoordinates(coordinateId, include, pageInfo);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpPost]
        [Route("feeds")]
        public IActionResult Post(List<CoordinatesModel> coordinates)
        {
            dynamic response = _coordinateRepository.InsertCoordinates(coordinates);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("feeds/{coordinateId}")]
        public IActionResult Delete(string coordinateId)
        {
            dynamic response = _coordinateRepository.DeleteCoordinates(coordinateId);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
