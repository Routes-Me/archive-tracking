using Microsoft.AspNetCore.Mvc;

namespace ArchiveTrackService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public string Get()
        {
            return "Archive Tracking Service Start Successfully.";
        }
    }
}
