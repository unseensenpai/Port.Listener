using Microsoft.AspNetCore.Mvc;
using PortListener.Contract;
using System.Reflection.Metadata.Ecma335;

namespace PortListener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PortCheckController
    {
        private readonly IPortCheckService _portCheckService;
        public PortCheckController(IPortCheckService portCheckService)
        {
            _portCheckService = portCheckService;
        }
        [HttpGet]
        public Task<bool> GetPorts() => _portCheckService.PingIps(); 
    }
}
