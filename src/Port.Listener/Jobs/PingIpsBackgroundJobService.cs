using PortListener.Contract;
using Serilog;

namespace PortListener.Jobs
{
    public class PingIpsBackgroundJobService
    {
        private readonly IPortCheckService _portCheckService;
        public PingIpsBackgroundJobService(IPortCheckService portCheckService)
        {
            _portCheckService = portCheckService;
        }
        public async Task Execute()
        {
            var result = await _portCheckService.PingIps();
            Log.Information($"{result}");
        }
    }
}
