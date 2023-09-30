using PortListener.DAL;

namespace PortListener.Extensions
{
    public class DifferenceHelper
    {
        public DifferenceHelper()
        {
        }
        public static Dictionary<string, int> DifferenceCalculator(List<PortsDAL> listValue, Dictionary<string, int> failPairs)
        {
            Dictionary<string, int> mailDictionary = new();
            foreach (var item in failPairs)
            {
                if (listValue.Any(x => x.IpAddress == item.Key))
                {
                    PortsDAL portsDAL = listValue.FirstOrDefault(x => x.IpAddress == item.Key);
                    if (portsDAL is not null)
                    {
                        mailDictionary.Add(portsDAL.IpAddress, portsDAL.ErrorCount);
                    }
                };
            }
            return mailDictionary;
        }
    }
}
