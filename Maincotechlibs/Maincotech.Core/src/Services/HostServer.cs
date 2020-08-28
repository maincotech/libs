namespace Maincotech.Services
{
    using System;

    public class HostServer
    {
        public Guid Id { get; set; }
        public string DnsName { get; set; }
        public int Port { get; set; }
        public string Os { get; set; }
        public string Protocol { get; set; }
        public string Path { get; set; }

        public string Uri { get; set; }
    }
}