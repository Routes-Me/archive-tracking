using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchiveTrackService.Abstraction;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using ArchiveTrackService.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ArchiveTrackService.Controllers
{
    [ApiController]
    [ApiVersion( "1.0" )]
    [Route("v{version:apiVersion}/")]
    public class FeedsController : ControllerBase
    {
        private readonly ICoordinateRepository _coordinateRepository;
        private readonly ArchiveTrackServiceContext _context;
        public FeedsController(ICoordinateRepository coordinateRepository, ArchiveTrackServiceContext context)
        {
            _coordinateRepository = coordinateRepository;
            _context = context;
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

        [HttpPost]
        [Route("vehicles/{vehicleId}/coordinates")]
        public async Task<IActionResult> PostCoordinates(string vehicleId, List<CoordinatesOfVehicleDto> coordinates)
        {
            List<Coordinates> coordinatesList = new List<Coordinates>();
            try
            {
                coordinatesList = _coordinateRepository.InsertCoordinatesForVehicle(vehicleId, coordinates);
                _context.AddRange(coordinatesList);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status202Accepted, CommonMessage.FeedsInsert);
        }
    }
}
