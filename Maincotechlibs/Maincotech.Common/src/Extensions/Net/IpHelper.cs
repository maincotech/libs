namespace System.Net
{
    public static class IpHelper
    {
        public static IPAddress[] GetIpAddressList()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        }
    }
}