using System;
using System.Collections.Generic;
using System.Text;

namespace Maincotech.Messaging
{
    public class MessagingOptions
    {
        public ServiceBusProvider ServiceBusProvider { get; set; }
        public string Host { get; set; }

        public string Endpoint { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

    }
}
