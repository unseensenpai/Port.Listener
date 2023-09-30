using AutoMapper;
using DevExpress.Xpo;
using PortListener.Contract;
using PortListener.DAL;
using PortListener.Extensions;
using Serilog;
using System.Net.NetworkInformation;

namespace PortListener.Concrete
{
    public class PortCheckService : IPortCheckService
    {
        public UnitOfWork unitOfWork;
        public IMapper _mapper;
        public PortCheckService(UnitOfWork uow, IMapper mapper)
        {
            _mapper = mapper;
            unitOfWork = uow;
        }
        public async Task<bool> PingIps()
        {
            Log.Information("Portlar alınıyor..");
            try
            {
                Dictionary<string, int> failPairs = new();
                List<PortsDAL> ipList = await unitOfWork.Query<PortsDAL>()
                    .Where(pd =>
                        pd.IsActive)
                    .OrderBy(ob =>
                        ob.IpAddress)
                    .ToListAsync();

                using Ping pingSender = new();

                for (int i = 0; i < 3; i++)
                {
                    foreach (PortsDAL ip in ipList)
                    {
                        int ipErrorCount = ip.ErrorCount;
                        int errorCounter = 0;
                        for (int j = 0; j < 4; j++)
                        {
                            try
                            {
                                PingReply result = await pingSender.SendPingAsync(ip.IpAddress, 4000).ConfigureAwait(true);
                                if (result.Status != IPStatus.Success || result.RoundtripTime > 2000)
                                {
                                    errorCounter++;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"{ip.IpAddress} ipsine ping atılırken hata fırladı. - HATA MESAJI: {ex.Message} - STACK: {ex.StackTrace}");
                                errorCounter = 4;
                                break;
                            }
                        }

                        if (errorCounter == 4)
                        {
                            if (failPairs.Any(x => x.Key == ip.IpAddress))
                            {
                                failPairs[ip.IpAddress] = ipErrorCount + 1;
                            }
                            else
                                failPairs.Add(ip.IpAddress, ipErrorCount + 1);
                        }

                        ip.LastTriggerTime = DateTime.Now;
                        await unitOfWork.SaveAsync(ip);
                    }
                }
                foreach (KeyValuePair<string, int> failPair in failPairs)
                {

                    var port = await unitOfWork.Query<PortsDAL>().FirstOrDefaultAsync(x => x.IpAddress == failPair.Key);
                    if (port.ErrorCount == 4)
                    {
                        port.ErrorCount = 0;
                    }
                    port.ErrorCount += 1;
                    port.TotalErrors += 1;

                    await unitOfWork.SaveAsync(port);
                }
                await unitOfWork.CommitTransactionAsync().ConfigureAwait(true);


                Dictionary<string, int> mailDictionary = DifferenceHelper.DifferenceCalculator(ipList, failPairs);
                if (mailDictionary.Any())
                {
                    Dictionary<string, int> filteredMailDictionary = mailDictionary
                        .Where(x => x.Value % 4 == 0)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    if (filteredMailDictionary.Any())
                    {
                        MailSender.SendPingHTMLMail(mailDictionary);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                unitOfWork.RollbackTransaction();
                Log.Error($"Ping metodunda bir hata meydana geldi. {ex.Message}");
                return false;
            }
        }
    }
}
