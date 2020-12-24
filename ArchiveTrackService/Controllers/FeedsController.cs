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
        [Route("feeds/{id=0}")]
        public IActionResult Get(string id, string include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _coordinateRepository.GetCoordinates(id, include, pageInfo);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpPost]
        [Route("feeds")]
        public IActionResult Post(List<Coordinates> coordinates)
        {
            dynamic response = _coordinateRepository.InsertCoordinates(coordinates);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpDelete]
        [Route("feeds/{id}")]
        public IActionResult Delete(string id)
        {
            dynamic response = _coordinateRepository.DeleteCoordinates(id);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
