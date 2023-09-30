namespace PortListener.Contract
{
    public interface IPortCheckService
    {
        public Task<bool> PingIps();
    }
}
